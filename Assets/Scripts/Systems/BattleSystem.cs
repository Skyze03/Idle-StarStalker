using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public const float ActionThreshold = 100f;
    public const float RageThreshold = 100f;

    // Keeps the initial Action Bar prototype close to the V1 attack timing.
    private const float ActionGainPerAgilityPerSecond = ActionThreshold / 15f;
    private const float RagePerNormalAttack = 10f;
    private const float RagePerHitTaken = 15f;
    private const float RageGainPerWisdomPoint = 0.02f;

    private PlayerData playerData;
    private BattleState battleState;
    private EnemyData enemyData;
    private EquipmentSystem equipmentSystem;
    private CombatBuildSnapshot playerBuild;
    private CombatBuildSnapshot enemyBuild;
    private readonly Queue<string> combatLog = new Queue<string>();

    public BattleState BattleState => battleState;
    public EnemyData EnemyData => enemyData;
    public CombatBuildSnapshot PlayerBuild => playerBuild;
    public CombatBuildSnapshot EnemyBuild => enemyBuild;
    public EquipmentSystem EquipmentSystem => equipmentSystem;
    public string CombatLogText => string.Join("\n", combatLog);
    public event Action<BattleResult> BattleEnded;

    public CombatBuildSnapshot CreatePlayerBuildPreview()
    {
        if (playerData == null) return null;
        playerData.CalculateStats();
        playerData.RefreshUltimate();
        return CombatBuildSnapshot.FromPlayer(playerData, equipmentSystem);
    }

    public void Setup(
        PlayerData playerData,
        BattleState battleState,
        EnemyData enemyData,
        EquipmentSystem equipmentSystem)
    {
        this.playerData = playerData;
        this.battleState = battleState;
        this.enemyData = enemyData;
        this.equipmentSystem = equipmentSystem;

        ResetBattle();
    }

    public bool StartBattle()
    {
        if (playerData == null || battleState == null || enemyData == null)
        {
            Debug.LogError("BattleSystem is not properly set up.");
            return false;
        }

        if (battleState.battleRunning)
        {
            Debug.LogWarning("A battle is already running.");
            return false;
        }

        // Make sure permanent player stats are up to date.
        playerData.CalculateStats();
        playerData.RefreshUltimate();
        playerBuild = CombatBuildSnapshot.FromPlayer(playerData, equipmentSystem);
        enemyBuild = CombatBuildSnapshot.FromEnemy(enemyData);
        combatLog.Clear();

        battleState.playerCurrentHP = playerBuild.maxHP;
        battleState.enemyCurrentHP = enemyBuild.maxHP;

        battleState.playerActionValue = 0f;
        battleState.enemyActionValue = 0f;
        battleState.playerRage = 0f;
        battleState.enemyRage = 0f;
        battleState.playerAgilityBuffActions = 0;
        battleState.enemyAgilityBuffActions = 0;

        battleState.battleRunning = true;
        battleState.battleResult = BattleResult.None;
        battleState.rewardGranted = false;

        Record($"Build locked: {playerBuild.ultimate.ultimateName} vs " +
            $"{enemyBuild.ultimate.ultimateName}");
        return true;
    }

    public void Tick(float deltaTime)
    {
        if (battleState == null || !battleState.battleRunning)
        {
            return;
        }

        battleState.playerActionValue += GetActionGain(
            GetEffectiveAgility(
                playerBuild.agility,
                battleState.playerAgilityBuffActions,
                playerBuild.ultimate
            ),
            deltaTime
        );
        battleState.enemyActionValue += GetActionGain(
            GetEffectiveAgility(
                enemyBuild.agility,
                battleState.enemyAgilityBuffActions,
                enemyBuild.ultimate
            ),
            deltaTime
        );

        // Player attacks first if both become ready on the same frame.
        while (battleState.playerActionValue >= ActionThreshold)
        {
            battleState.playerActionValue -= ActionThreshold;

            PlayerAct();

            if (!battleState.battleRunning)
            {
                return;
            }
        }

        while (battleState.enemyActionValue >= ActionThreshold)
        {
            battleState.enemyActionValue -= ActionThreshold;

            EnemyAct();

            if (!battleState.battleRunning)
            {
                return;
            }
        }
    }

    private void PlayerAct()
    {
        UltimateData ultimate = playerBuild.ultimate;
        bool useUltimate = CanUseUltimate(battleState.playerRage, ultimate);
        bool consumeExistingBuff = battleState.playerAgilityBuffActions > 0;

        int damage = CalculateDamage(
            playerBuild.attack,
            enemyBuild.defense
        );

        if (useUltimate)
        {
            int hitCount;
            damage = ResolveUltimate(
                damage, playerBuild.defense, ultimate,
                ref battleState.playerAgilityBuffActions, out hitCount
            );
            battleState.playerRage = ultimate.effectType == UltimateEffectType.RageRefund
                ? 40f
                : 0f;

            if (damage > 0)
            {
                battleState.enemyRage = AddRage(
                    battleState.enemyRage,
                    RagePerHitTaken * hitCount,
                    enemyBuild.wisdom
                );
            }
        }

        battleState.enemyCurrentHP -= damage;

        if (battleState.enemyCurrentHP < 0)
        {
            battleState.enemyCurrentHP = 0;
        }

        if (useUltimate)
        {
            Record(ultimate.effectType == UltimateEffectType.AgilityBuff
                ? $"Player used {ultimate.ultimateName}: Agility boosted for 3 actions."
                : $"Player used {ultimate.ultimateName}: {damage} damage.");
        }
        else
        {
            battleState.playerRage = AddRage(
                battleState.playerRage,
                RagePerNormalAttack,
                playerBuild.wisdom
            );
            battleState.playerRage = AddRage(
                battleState.playerRage,
                playerBuild.bonusRageOnAttack,
                0
            );

            Record($"Player attacked: {damage} damage.");
        }

        if (!useUltimate)
            battleState.enemyRage = AddRage(
                battleState.enemyRage, RagePerHitTaken, enemyBuild.wisdom
            );

        if (damage > 0)
            battleState.enemyRage = AddRage(
                battleState.enemyRage, enemyBuild.bonusRageOnHit, 0
            );

        if (consumeExistingBuff &&
            (!useUltimate || ultimate.effectType != UltimateEffectType.AgilityBuff))
            battleState.playerAgilityBuffActions--;

        CheckBattleEnd();
    }

    private void EnemyAct()
    {
        UltimateData ultimate = enemyBuild.ultimate;
        bool useUltimate = CanUseUltimate(battleState.enemyRage, ultimate);
        bool consumeExistingBuff = battleState.enemyAgilityBuffActions > 0;

        int damage = CalculateDamage(
            enemyBuild.attack,
            playerBuild.defense
        );

        if (useUltimate)
        {
            int hitCount;
            damage = ResolveUltimate(
                damage, enemyBuild.defense, ultimate,
                ref battleState.enemyAgilityBuffActions, out hitCount
            );
            battleState.enemyRage = ultimate.effectType == UltimateEffectType.RageRefund
                ? 40f
                : 0f;

            if (damage > 0)
            {
                battleState.playerRage = AddRage(
                    battleState.playerRage,
                    RagePerHitTaken * hitCount,
                    playerBuild.wisdom
                );
            }
        }

        battleState.playerCurrentHP -= damage;

        if (battleState.playerCurrentHP < 0)
        {
            battleState.playerCurrentHP = 0;
        }

        if (useUltimate)
        {
            Record(ultimate.effectType == UltimateEffectType.AgilityBuff
                ? $"Enemy used {ultimate.ultimateName}: Agility boosted for 3 actions."
                : $"Enemy used {ultimate.ultimateName}: {damage} damage.");
        }
        else
        {
            battleState.enemyRage = AddRage(
                battleState.enemyRage,
                RagePerNormalAttack,
                enemyBuild.wisdom
            );
            battleState.enemyRage = AddRage(
                battleState.enemyRage,
                enemyBuild.bonusRageOnAttack,
                0
            );

            Record($"Enemy attacked: {damage} damage.");
        }

        if (!useUltimate)
            battleState.playerRage = AddRage(
                battleState.playerRage, RagePerHitTaken, playerBuild.wisdom
            );

        if (damage > 0)
            battleState.playerRage = AddRage(
                battleState.playerRage, playerBuild.bonusRageOnHit, 0
            );

        if (consumeExistingBuff &&
            (!useUltimate || ultimate.effectType != UltimateEffectType.AgilityBuff))
            battleState.enemyAgilityBuffActions--;

        CheckBattleEnd();
    }

    private int CalculateDamage(int attackerAttack, int defenderDefense)
    {
        return Mathf.Max(1, attackerAttack - defenderDefense);
    }

    private int ResolveUltimate(
        int normalDamage,
        int attackerDefense,
        UltimateData ultimate,
        ref int agilityBuffActions,
        out int hitCount)
    {
        hitCount = 1;
        switch (ultimate.effectType)
        {
            case UltimateEffectType.DefenseScaling:
                return Mathf.Max(1, normalDamage +
                    Mathf.RoundToInt(attackerDefense * ultimate.power));
            case UltimateEffectType.MultiHit:
                hitCount = Mathf.Max(1, ultimate.hitCount);
                return Mathf.Max(1,
                    Mathf.RoundToInt(normalDamage * ultimate.power) * hitCount);
            case UltimateEffectType.AgilityBuff:
                agilityBuffActions = 3;
                return 0;
            default:
                return Mathf.Max(1, Mathf.RoundToInt(normalDamage * ultimate.power));
        }
    }

    private int GetEffectiveAgility(
        int baseAgility,
        int buffActions,
        UltimateData equippedUltimate)
    {
        if (buffActions <= 0) return baseAgility;
        float bonus = equippedUltimate != null &&
            equippedUltimate.effectType == UltimateEffectType.AgilityBuff
                ? equippedUltimate.power
                : 0.5f;
        return Mathf.Max(1, Mathf.RoundToInt(baseAgility * (1f + bonus)));
    }

    private bool CanUseUltimate(float rage, UltimateData ultimate)
    {
        return ultimate != null && rage >= RageThreshold;
    }

    private float AddRage(float currentRage, float baseGain, int wisdom)
    {
        float wisdomMultiplier = 1f +
            Mathf.Max(0, wisdom) * RageGainPerWisdomPoint;

        return Mathf.Min(
            RageThreshold,
            currentRage + baseGain * wisdomMultiplier
        );
    }

    private float GetActionGain(int agility, float deltaTime)
    {
        return Mathf.Max(1, agility) *
            ActionGainPerAgilityPerSecond *
            Mathf.Max(0f, deltaTime);
    }

    private void CheckBattleEnd()
    {
        if (!battleState.battleRunning)
        {
            return;
        }

        if (battleState.enemyCurrentHP <= 0)
        {
            battleState.battleRunning = false;
            battleState.battleResult = BattleResult.Victory;

            Debug.Log("Battle result: Victory");
            Record("Battle ended: Victory.");
            BattleEnded?.Invoke(BattleResult.Victory);
            return;
        }

        if (battleState.playerCurrentHP <= 0)
        {
            battleState.battleRunning = false;
            battleState.battleResult = BattleResult.Defeat;

            Debug.Log("Battle result: Defeat");
            Record("Battle ended: Defeat.");
            BattleEnded?.Invoke(BattleResult.Defeat);
        }
    }

    public void ResetBattle()
    {
        if (battleState == null)
        {
            return;
        }

        battleState.Reset();

        if (playerData != null)
        {
            playerData.CalculateStats();
            battleState.playerCurrentHP = playerData.stats.hp;
        }

        if (enemyData != null)
        {
            battleState.enemyCurrentHP = enemyData.maxHP;
        }
    }

    private void Record(string message)
    {
        if (combatLog.Count >= 2) combatLog.Dequeue();
        combatLog.Enqueue(message);
        Debug.Log(message);
    }
}
