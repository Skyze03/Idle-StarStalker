using System;
using UnityEngine;

public class MainStageSystem : MonoBehaviour
{
    public event Action<string> ToastRequested;
    public event Action<string, string> ResultRequested;
    public const int TotalStages = 20;
    public const int MaxStamina = 20;
    public const int StaminaCostPerBattle = 1;
    public const int StaminaRecoverySeconds = 5 * 60;
    public const float BattleEquipmentDropChance = 0.20f;
    public const float SweepEquipmentDropChance = 0.10f;

    private PlayerData playerData;
    private InventorySystem inventorySystem;
    private BattleSystem battleSystem;
    private BattleState battleState;
    private EnemyData enemyData;
    private MainStageState stageState;
    private UltimateSystem ultimateSystem;
    private EquipmentSystem equipmentSystem;
    private RewardPipeline rewardPipeline;
    private int activeStage;

    public MainStageState State => stageState;
    public int ActiveStage => activeStage;
    public bool IsPrototypeComplete =>
        stageState != null && stageState.highestClearedStage >= TotalStages;
    public string LastFeedback { get; private set; } =
        "Select a stage to view its enemy and rewards.";
    public bool IsSelectedStageCleared =>
        stageState != null &&
        stageState.selectedStage <= stageState.highestClearedStage;

    public int SecondsUntilNextStamina
    {
        get
        {
            if (stageState == null || stageState.battleStamina >= MaxStamina)
                return 0;

            DateTime lastRefresh = new DateTime(
                stageState.lastStaminaRefreshUtcTicks,
                DateTimeKind.Utc
            );
            double elapsed = Math.Max(0d, (DateTime.UtcNow - lastRefresh).TotalSeconds);
            int remaining = StaminaRecoverySeconds -
                (int)(elapsed % StaminaRecoverySeconds);
            return Math.Max(1, remaining);
        }
    }

    public void Setup(
        PlayerData playerData,
        InventorySystem inventorySystem,
        BattleSystem battleSystem,
        BattleState battleState,
        EnemyData enemyData,
        MainStageState stageState,
        UltimateSystem ultimateSystem,
        EquipmentSystem equipmentSystem,
        RewardPipeline rewardPipeline = null)
    {
        this.playerData = playerData;
        this.inventorySystem = inventorySystem;
        this.battleSystem = battleSystem;
        this.battleState = battleState;
        this.enemyData = enemyData;
        this.stageState = stageState;
        this.ultimateSystem = ultimateSystem;
        this.equipmentSystem = equipmentSystem;
        this.rewardPipeline = rewardPipeline;

        this.stageState.Normalize();

        battleSystem.BattleEnded -= OnBattleEnded;
        battleSystem.BattleEnded += OnBattleEnded;

        RefreshStamina();
        ConfigureEnemy(stageState.selectedStage);
    }

    private void OnDestroy()
    {
        if (battleSystem != null)
        {
            battleSystem.BattleEnded -= OnBattleEnded;
        }
    }

    public void Tick()
    {
        RefreshStamina();
    }

    public bool SelectStage(int stageNumber)
    {
        if (stageState == null || battleState == null || battleState.battleRunning)
        {
            return false;
        }

        if (stageNumber < 1 ||
            stageNumber > TotalStages ||
            stageNumber > stageState.highestUnlockedStage)
        {
            return false;
        }

        stageState.selectedStage = stageNumber;
        ConfigureEnemy(stageNumber);
        LastFeedback = IsSelectedStageCleared
            ? $"Stage {stageNumber} cleared. Replay or sweep available."
            : $"Stage {stageNumber} ready for first clear.";
        return true;
    }

    public bool TryStartSelectedStage()
    {
        if (!CanSpendStamina() || battleSystem == null)
        {
            return false;
        }

        activeStage = stageState.selectedStage;
        ConfigureEnemy(activeStage);
        SpendStamina();
        LastFeedback = $"Stage {activeStage} battle in progress.";

        if (!battleSystem.StartBattle())
        {
            RefundStamina();
            activeStage = 0;
            return false;
        }

        Debug.Log(
            $"Started Stage {activeStage}. " +
            $"Stamina: {stageState.battleStamina}/{MaxStamina}"
        );
        return true;
    }

    public bool CanStartSelectedStage()
    {
        return stageState != null &&
            battleState != null &&
            !battleState.battleRunning &&
            stageState.selectedStage <= stageState.highestUnlockedStage &&
            stageState.battleStamina >= StaminaCostPerBattle;
    }

    public bool CanSweepSelectedStage()
    {
        return CanStartSelectedStage() && IsSelectedStageCleared;
    }

    public bool TrySweepHighestClearedStage()
    {
        if (stageState == null || stageState.highestClearedStage <= 0)
        {
            Debug.Log("No cleared stage is available to sweep.");
            return false;
        }

        return TrySweepStage(stageState.highestClearedStage);
    }

    public bool TrySweepStage(int stageNumber)
    {
        if (stageState == null || battleState == null || battleState.battleRunning)
        {
            return false;
        }

        if (stageNumber < 1 || stageNumber > stageState.highestClearedStage)
        {
            SetFeedback("Only cleared stages can be swept.");
            return false;
        }

        if (!CanSpendStamina())
        {
            return false;
        }

        SpendStamina();
        RewardBundle reward = GetNormalReward(stageNumber);
        string sweepDrop = RollEquipmentDrop(stageNumber, SweepEquipmentDropChance);
        if (!string.IsNullOrEmpty(sweepDrop)) reward.AddEquipment(sweepDrop);
        GrantReward(reward, $"Stage {stageNumber} sweep");
        LastFeedback = $"Sweep complete: {FormatReward(reward)}";
        ToastRequested?.Invoke($"Stage {stageNumber} Sweep\n{FormatReward(reward)}");
        return true;
    }

    public void RefreshAfterLoad()
    {
        if (stageState == null) return;

        stageState.Normalize();
        RefreshStamina();
        ConfigureEnemy(stageState.selectedStage);
    }

    private void OnBattleEnded(BattleResult result)
    {
        int completedStage = activeStage;
        activeStage = 0;

        if (result != BattleResult.Victory || completedStage <= 0)
        {
            if (result == BattleResult.Defeat && completedStage > 0)
            {
                LastFeedback =
                    $"Stage {completedStage} defeated. No rewards received.";
                ResultRequested?.Invoke(
                    "DEFEAT",
                    $"Stage {completedStage}\nNo rewards received.\n\nStrengthen or adjust your build, then try again."
                );
            }
            return;
        }

        RewardBundle totalReward = GetNormalReward(completedStage);
        string normalDrop = RollEquipmentDrop(completedStage, BattleEquipmentDropChance);
        if (!string.IsNullOrEmpty(normalDrop)) totalReward.AddEquipment(normalDrop);
        bool firstClear = completedStage > stageState.highestClearedStage;

        if (firstClear)
        {
            stageState.highestClearedStage = completedStage;
            stageState.highestUnlockedStage = Math.Min(
                TotalStages,
                completedStage + 1
            );
            totalReward.Add(GetFirstClearReward(completedStage));
            string unlockedUltimate = GetUltimateUnlockForStage(completedStage);
            if (!string.IsNullOrEmpty(unlockedUltimate)) totalReward.ultimateId = unlockedUltimate;
            string equipmentDrop = GetEquipmentUnlockForStage(completedStage);
            if (!string.IsNullOrEmpty(equipmentDrop)) totalReward.AddEquipment(equipmentDrop);

            if (stageState.highestUnlockedStage > completedStage)
            {
                stageState.selectedStage = stageState.highestUnlockedStage;
            }
        }

        GrantReward(
            totalReward,
            firstClear
                ? $"Stage {completedStage} first clear"
                : $"Stage {completedStage} clear"
        );

        LastFeedback = firstClear
            ? $"First clear! {FormatReward(totalReward)} " +
              GetUltimateUnlockFeedback(completedStage) +
              GetEquipmentUnlockFeedback(completedStage) +
              (completedStage < TotalStages
                  ? $"Stage {completedStage + 1} unlocked."
                  : "All prototype stages cleared!")
            : $"Victory! {FormatReward(totalReward)}";

        if (IsPrototypeComplete)
        {
            stageState.selectedStage = TotalStages;
            LastFeedback =
                $"Main Story Prototype Complete! Stage {TotalStages} remains " +
                $"available for replay and sweep. {FormatReward(totalReward)}";
        }

        string resultBody = $"Stage {completedStage} cleared\n{FormatReward(totalReward)}";
        if (firstClear)
        {
            string ultimateUnlock = GetUltimateUnlockForStage(completedStage);
            string equipmentUnlock = GetEquipmentUnlockForStage(completedStage);
            if (!string.IsNullOrEmpty(ultimateUnlock))
                resultBody += $"\nUltimate unlocked: {UltimateData.GetById(ultimateUnlock).ultimateName}";
            EquipmentData equipment = EquipmentData.GetById(equipmentUnlock);
            if (equipment != null) resultBody += $"\nEquipment obtained: {equipment.itemName}";
            resultBody += completedStage < TotalStages
                ? $"\nStage {completedStage + 1} unlocked"
                : "\nMain Story prototype complete!";
        }
        if (!string.IsNullOrEmpty(normalDrop))
            resultBody += $"\nRandom drop: {EquipmentData.GetById(normalDrop).itemName}";
        ResultRequested?.Invoke(firstClear ? "FIRST CLEAR" : "VICTORY", resultBody);
    }

    private bool CanSpendStamina()
    {
        if (stageState == null || battleState == null || battleState.battleRunning)
        {
            return false;
        }

        RefreshStamina();

        if (stageState.battleStamina < StaminaCostPerBattle)
        {
            SetFeedback(
                $"Not enough stamina. Next point in {SecondsUntilNextStamina}s."
            );
            return false;
        }

        return true;
    }

    private void SpendStamina()
    {
        bool wasFull = stageState.battleStamina >= MaxStamina;
        stageState.battleStamina -= StaminaCostPerBattle;

        if (wasFull)
        {
            stageState.lastStaminaRefreshUtcTicks = DateTime.UtcNow.Ticks;
        }
    }

    private void RefundStamina()
    {
        stageState.battleStamina = Math.Min(
            MaxStamina,
            stageState.battleStamina + StaminaCostPerBattle
        );
    }

    private void RefreshStamina()
    {
        if (stageState == null) return;

        DateTime now = DateTime.UtcNow;

        if (stageState.battleStamina >= MaxStamina)
        {
            stageState.battleStamina = MaxStamina;
            stageState.lastStaminaRefreshUtcTicks = now.Ticks;
            return;
        }

        DateTime lastRefresh = new DateTime(
            stageState.lastStaminaRefreshUtcTicks,
            DateTimeKind.Utc
        );
        if (lastRefresh > now)
        {
            stageState.lastStaminaRefreshUtcTicks = now.Ticks;
            return;
        }

        double elapsedSeconds = Math.Max(0d, (now - lastRefresh).TotalSeconds);
        int recovered = (int)(elapsedSeconds / StaminaRecoverySeconds);

        if (recovered <= 0) return;

        stageState.battleStamina = Math.Min(
            MaxStamina,
            stageState.battleStamina + recovered
        );

        if (stageState.battleStamina >= MaxStamina)
        {
            stageState.lastStaminaRefreshUtcTicks = now.Ticks;
        }
        else
        {
            stageState.lastStaminaRefreshUtcTicks = lastRefresh
                .AddSeconds(recovered * StaminaRecoverySeconds)
                .Ticks;
        }
    }

    private void ConfigureEnemy(int stageNumber)
    {
        if (enemyData == null) return;

        int stageIndex = Math.Max(0, stageNumber - 1);

        enemyData.traits = GetEnemyTraits(stageNumber);
        enemyData.enemyName =
            $"Stage {stageNumber} {EnemyTraitUtility.GetDisplayName(enemyData.traits)} Shade";
        enemyData.maxHP = ScaleStat(60f, 1.18f, stageIndex);
        enemyData.attack = ScaleStat(8f, 1.12f, stageIndex);
        enemyData.defense = ScaleStat(2f, 1.10f, stageIndex);
        enemyData.agility = 4 + stageIndex / 5;
        enemyData.wisdom = 5 + stageIndex / 5;
        ApplyEnemyTraits(enemyData);
        enemyData.equippedUltimate = UltimateData.GetById(GetEnemyUltimateId(stageNumber));

        if (battleState != null && !battleState.battleRunning)
        {
            battleSystem.ResetBattle();
        }
    }

    private int ScaleStat(float baseValue, float growth, int stageIndex)
    {
        return Mathf.Max(
            1,
            Mathf.RoundToInt(baseValue * Mathf.Pow(growth, stageIndex))
        );
    }

    public RewardBundle GetNormalReward(int stageNumber)
    {
        return new RewardBundle(
            energy: 10 + 3 * Math.Max(0, stageNumber - 1)
        );
    }

    public RewardBundle GetFirstClearReward(int stageNumber)
    {
        RewardBundle reward = new RewardBundle(
            energy: 50 + 10 * Math.Max(0, stageNumber - 1)
        );

        switch (stageNumber)
        {
            case 5:
                reward.memoryFragments = 1;
                reward.runes = 1;
                break;
            case 10:
                reward.memoryFragments = 2;
                reward.runes = 2;
                break;
            case 15:
                reward.memoryFragments = 3;
                reward.runes = 3;
                break;
            case 20:
                reward.memoryFragments = 5;
                reward.runes = 5;
                break;
        }

        return reward;
    }

    public string GetUltimateUnlockForStage(int stageNumber)
    {
        switch (stageNumber)
        {
            case 5: return "iron_retaliation";
            case 10: return "rapid_nova";
            case 15: return "meteor_flurry";
            case 20: return "swift_ascension";
            default: return string.Empty;
        }
    }

    public string GetEnemyUltimateId(int stageNumber)
    {
        if (stageNumber >= 20) return "swift_ascension";
        if (stageNumber >= 15) return "meteor_flurry";
        if (stageNumber >= 10) return "rapid_nova";
        if (stageNumber >= 5) return "iron_retaliation";
        return "star_burst";
    }

    public string GetEquipmentUnlockForStage(int stageNumber)
    {
        return EquipmentSystem.GetStageEquipmentId(stageNumber);
    }

    public string RollEquipmentDrop(int stageNumber, float chance, float? forcedRoll = null)
    {
        float roll = forcedRoll ?? UnityEngine.Random.value;
        if (roll >= chance) return string.Empty;
        int poolSize = Math.Min(10, Math.Max(1, stageNumber));
        int index = forcedRoll.HasValue
            ? Math.Min(poolSize - 1, Mathf.FloorToInt((forcedRoll.Value / Math.Max(0.0001f, chance)) * poolSize))
            : UnityEngine.Random.Range(0, poolSize);
        return EquipmentSystem.GetStageEquipmentId(index + 1);
    }

    private string GetEquipmentUnlockFeedback(int stageNumber)
    {
        EquipmentData item = EquipmentData.GetById(
            GetEquipmentUnlockForStage(stageNumber)
        );
        return item == null ? string.Empty : $"Obtained {item.itemName}! ";
    }

    public EnemyTrait GetEnemyTraits(int stageNumber)
    {
        switch (stageNumber)
        {
            case 5: return EnemyTrait.Frenzy | EnemyTrait.Bulwark;
            case 10: return EnemyTrait.Swift | EnemyTrait.Sage;
            case 15: return EnemyTrait.Frenzy | EnemyTrait.Swift;
            case 20: return EnemyTrait.Bulwark | EnemyTrait.Sage;
        }

        switch ((stageNumber - 1) % 4)
        {
            case 1: return EnemyTrait.Frenzy;
            case 2: return EnemyTrait.Bulwark;
            case 3: return EnemyTrait.Swift;
            default: return stageNumber >= 6 ? EnemyTrait.Sage : EnemyTrait.None;
        }
    }

    private void ApplyEnemyTraits(EnemyData enemy)
    {
        if ((enemy.traits & EnemyTrait.Frenzy) != 0)
            enemy.attack = Mathf.Max(1, Mathf.CeilToInt(enemy.attack * 1.15f));
        if ((enemy.traits & EnemyTrait.Bulwark) != 0)
            enemy.defense = Mathf.Max(1, Mathf.CeilToInt(enemy.defense * 1.25f));
        if ((enemy.traits & EnemyTrait.Swift) != 0)
            enemy.agility = Mathf.Max(1, Mathf.CeilToInt(enemy.agility * 1.25f));
        if ((enemy.traits & EnemyTrait.Sage) != 0)
            enemy.wisdom = Mathf.Max(1, Mathf.CeilToInt(enemy.wisdom * 1.30f));
    }

    private string GetUltimateUnlockFeedback(int stageNumber)
    {
        string id = GetUltimateUnlockForStage(stageNumber);
        return string.IsNullOrEmpty(id)
            ? string.Empty
            : $"Unlocked {UltimateData.GetById(id).ultimateName}! ";
    }

    private void GrantReward(RewardBundle reward, string source)
    {
        if (rewardPipeline != null) { rewardPipeline.Grant(reward, source); return; }
        if (reward == null || playerData == null) return;

        playerData.energy += reward.energy;

        if (inventorySystem != null)
        {
            if (reward.memoryFragments > 0)
                inventorySystem.AddMemoryFragment(reward.memoryFragments);
            if (reward.runes > 0)
                inventorySystem.AddRune(reward.runes);
        }

        Debug.Log(
            $"{source} reward: +{reward.energy} Energy, " +
            $"+{reward.memoryFragments} Memory Fragments, +{reward.runes} Runes."
        );
    }

    private void SetFeedback(string message)
    {
        LastFeedback = message;
        Debug.Log(message);
    }

    private string FormatReward(RewardBundle reward)
    {
        if (reward == null) return "No reward.";

        string result = $"+{reward.energy} Energy";
        if (reward.memoryFragments > 0)
            result += $", +{reward.memoryFragments} Memory Fragments";
        if (reward.runes > 0)
            result += $", +{reward.runes} Runes";
        foreach (string equipmentId in reward.equipmentTemplateIds ?? Array.Empty<string>())
        {
            EquipmentData equipment = EquipmentData.GetById(equipmentId);
            if (equipment != null) result += $", {equipment.itemName}";
        }
        return result + ".";
    }
}
