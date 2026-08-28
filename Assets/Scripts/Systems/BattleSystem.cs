using System;
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

    public BattleState BattleState => battleState;
    public EnemyData EnemyData => enemyData;
    public event Action<BattleResult> BattleEnded;

    public void Setup(
        PlayerData playerData,
        BattleState battleState,
        EnemyData enemyData)
    {
        this.playerData = playerData;
        this.battleState = battleState;
        this.enemyData = enemyData;

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

        battleState.playerCurrentHP = playerData.stats.hp;
        battleState.enemyCurrentHP = enemyData.maxHP;

        battleState.playerActionValue = 0f;
        battleState.enemyActionValue = 0f;
        battleState.playerRage = 0f;
        battleState.enemyRage = 0f;

        battleState.battleRunning = true;
        battleState.battleResult = BattleResult.None;
        battleState.rewardGranted = false;

        Debug.Log(
            $"Battle started: Player HP {battleState.playerCurrentHP} " +
            $"vs {enemyData.enemyName} HP {battleState.enemyCurrentHP}"
        );
        return true;
    }

    public void Tick(float deltaTime)
    {
        if (battleState == null || !battleState.battleRunning)
        {
            return;
        }

        battleState.playerActionValue += GetActionGain(
            playerData.stats.agility,
            deltaTime
        );
        battleState.enemyActionValue += GetActionGain(
            enemyData.agility,
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
        UltimateData ultimate = playerData.equippedUltimate;
        bool useUltimate = CanUseUltimate(battleState.playerRage, ultimate);

        int damage = CalculateDamage(
            playerData.stats.attack,
            enemyData.defense
        );

        if (useUltimate)
        {
            damage = CalculateUltimateDamage(damage, ultimate);
            battleState.playerRage = 0f;
        }

        battleState.enemyCurrentHP -= damage;

        if (battleState.enemyCurrentHP < 0)
        {
            battleState.enemyCurrentHP = 0;
        }

        if (useUltimate)
        {
            Debug.Log(
                $"Player used {ultimate.ultimateName} for {damage} damage. " +
                $"{enemyData.enemyName} HP: {battleState.enemyCurrentHP}/{enemyData.maxHP}"
            );
        }
        else
        {
            battleState.playerRage = AddRage(
                battleState.playerRage,
                RagePerNormalAttack,
                playerData.stats.wisdom
            );

            Debug.Log(
                $"Player dealt {damage} damage. " +
                $"{enemyData.enemyName} HP: {battleState.enemyCurrentHP}/{enemyData.maxHP}"
            );
        }

        battleState.enemyRage = AddRage(
            battleState.enemyRage,
            RagePerHitTaken,
            enemyData.wisdom
        );

        CheckBattleEnd();
    }

    private void EnemyAct()
    {
        UltimateData ultimate = enemyData.equippedUltimate;
        bool useUltimate = CanUseUltimate(battleState.enemyRage, ultimate);

        int damage = CalculateDamage(
            enemyData.attack,
            playerData.stats.defense
        );

        if (useUltimate)
        {
            damage = CalculateUltimateDamage(damage, ultimate);
            battleState.enemyRage = 0f;
        }

        battleState.playerCurrentHP -= damage;

        if (battleState.playerCurrentHP < 0)
        {
            battleState.playerCurrentHP = 0;
        }

        if (useUltimate)
        {
            Debug.Log(
                $"{enemyData.enemyName} used {ultimate.ultimateName} for {damage} damage. " +
                $"Player HP: {battleState.playerCurrentHP}/{playerData.stats.hp}"
            );
        }
        else
        {
            battleState.enemyRage = AddRage(
                battleState.enemyRage,
                RagePerNormalAttack,
                enemyData.wisdom
            );

            Debug.Log(
                $"{enemyData.enemyName} dealt {damage} damage. " +
                $"Player HP: {battleState.playerCurrentHP}/{playerData.stats.hp}"
            );
        }

        battleState.playerRage = AddRage(
            battleState.playerRage,
            RagePerHitTaken,
            playerData.stats.wisdom
        );

        CheckBattleEnd();
    }

    private int CalculateDamage(int attackerAttack, int defenderDefense)
    {
        return Mathf.Max(1, attackerAttack - defenderDefense);
    }

    private int CalculateUltimateDamage(int normalDamage, UltimateData ultimate)
    {
        return Mathf.Max(
            1,
            Mathf.RoundToInt(normalDamage * ultimate.damageMultiplier)
        );
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
            BattleEnded?.Invoke(BattleResult.Victory);
            return;
        }

        if (battleState.playerCurrentHP <= 0)
        {
            battleState.battleRunning = false;
            battleState.battleResult = BattleResult.Defeat;

            Debug.Log("Battle result: Defeat");
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
}
