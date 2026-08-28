using System;
using UnityEngine;

public class MainStageSystem : MonoBehaviour
{
    public const int TotalStages = 20;
    public const int MaxStamina = 20;
    public const int StaminaCostPerBattle = 1;
    public const int StaminaRecoverySeconds = 5 * 60;

    private PlayerData playerData;
    private InventorySystem inventorySystem;
    private BattleSystem battleSystem;
    private BattleState battleState;
    private EnemyData enemyData;
    private MainStageState stageState;
    private int activeStage;

    public MainStageState State => stageState;
    public int ActiveStage => activeStage;
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
        MainStageState stageState)
    {
        this.playerData = playerData;
        this.inventorySystem = inventorySystem;
        this.battleSystem = battleSystem;
        this.battleState = battleState;
        this.enemyData = enemyData;
        this.stageState = stageState;

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
            Debug.Log("Only cleared stages can be swept.");
            return false;
        }

        if (!CanSpendStamina())
        {
            return false;
        }

        SpendStamina();
        RewardBundle reward = GetNormalReward(stageNumber);
        GrantReward(reward, $"Stage {stageNumber} sweep");
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
            return;
        }

        RewardBundle totalReward = GetNormalReward(completedStage);
        bool firstClear = completedStage > stageState.highestClearedStage;

        if (firstClear)
        {
            stageState.highestClearedStage = completedStage;
            stageState.highestUnlockedStage = Math.Min(
                TotalStages,
                completedStage + 1
            );
            totalReward.Add(GetFirstClearReward(completedStage));

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
            Debug.Log("Not enough battle stamina.");
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

        enemyData.enemyName = $"Stage {stageNumber} Shade";
        enemyData.maxHP = ScaleStat(60f, 1.18f, stageIndex);
        enemyData.attack = ScaleStat(8f, 1.12f, stageIndex);
        enemyData.defense = ScaleStat(2f, 1.10f, stageIndex);
        enemyData.agility = 4 + stageIndex / 5;
        enemyData.wisdom = 5 + stageIndex / 5;
        enemyData.equippedUltimate = UltimateData.CreateStarBurst();

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

    private RewardBundle GetNormalReward(int stageNumber)
    {
        return new RewardBundle(
            energy: 10 + 3 * Math.Max(0, stageNumber - 1)
        );
    }

    private RewardBundle GetFirstClearReward(int stageNumber)
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

    private void GrantReward(RewardBundle reward, string source)
    {
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
}
