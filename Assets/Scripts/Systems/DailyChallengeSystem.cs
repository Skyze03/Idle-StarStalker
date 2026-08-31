using System;
using UnityEngine;

public class DailyChallengeSystem : MonoBehaviour
{
    public const int MaxDailyAttempts = 3;
    public const int RewardEnergy = 40;
    public const int RewardRunes = 1;

    public event Action<string, string> ResultRequested;

    private DailyChallengeState state;
    private PlayerData player;
    private InventorySystem inventory;
    private BattleSystem battle;
    private BattleState battleState;
    private EnemyData enemy;
    private PanelSwitcher panels;
    private RewardPipeline rewards;
    private bool challengeActive;

    public DailyChallengeState State => state;
    public bool CanStart => state != null && state.remainingAttempts > 0 &&
        battleState != null && !battleState.battleRunning;

    public void Setup(DailyChallengeState challengeState, PlayerData playerData,
        InventorySystem inventorySystem, BattleSystem battleSystem,
        BattleState stateData, EnemyData enemyData, PanelSwitcher panelSwitcher,
        RewardPipeline rewardPipeline = null)
    {
        state = challengeState;
        player = playerData;
        inventory = inventorySystem;
        battle = battleSystem;
        battleState = stateData;
        enemy = enemyData;
        panels = panelSwitcher;
        rewards = rewardPipeline;
        RefreshDailyReset();
        battle.BattleEnded -= OnBattleEnded;
        battle.BattleEnded += OnBattleEnded;
    }

    public void RefreshDailyReset()
    {
        if (state == null) return;
        string today = DateTime.UtcNow.ToString("yyyyMMdd");
        if (state.utcDateKey == today) return;
        state.utcDateKey = today;
        state.remainingAttempts = MaxDailyAttempts;
    }

    public void ConfigureTodayEnemy()
    {
        RefreshDailyReset();
        if (enemy == null || battleState.battleRunning) return;
        int seed = int.Parse(state.utcDateKey) % 997;
        int variant = seed % 5;
        enemy.enemyName = $"Daily Astral Echo {variant + 1}";
        enemy.maxHP = 180 + variant * 35;
        enemy.attack = 20 + variant * 4;
        enemy.defense = 7 + variant * 2;
        enemy.agility = 6 + variant;
        enemy.wisdom = 7 + variant;
        enemy.traits = (EnemyTrait)(1 << (variant % 4));
        enemy.equippedUltimate = UltimateData.GetAll()[variant];
        battle.ResetBattle();
    }

    public bool TryStart()
    {
        RefreshDailyReset();
        if (!CanStart) return false;
        ConfigureTodayEnemy();
        state.remainingAttempts--;
        challengeActive = battle.StartBattle();
        if (!challengeActive) state.remainingAttempts++;
        return challengeActive;
    }

    private void OnBattleEnded(BattleResult result)
    {
        if (!challengeActive) return;
        challengeActive = false;
        if (result == BattleResult.Victory)
        {
            if (rewards != null) rewards.Grant(
                new RewardBundle(RewardEnergy, 0, RewardRunes), "Daily Challenge");
            else { player.energy += RewardEnergy; inventory?.AddRune(RewardRunes); }
            ResultRequested?.Invoke("DAILY VICTORY",
                $"+{RewardEnergy} Energy\n+{RewardRunes} Rune\n\nAttempts remaining: {state.remainingAttempts}/{MaxDailyAttempts}");
        }
        else
        {
            ResultRequested?.Invoke("DAILY DEFEAT",
                $"No reward\n\nAttempts remaining: {state.remainingAttempts}/{MaxDailyAttempts}");
        }
        panels?.ShowDailyChallengePanel();
    }
}
