using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private PlayerData playerData;
    private BattleState battleState;
    private EnemyData enemyData;

    public BattleState BattleState => battleState;
    public EnemyData EnemyData => enemyData;

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

    public void StartBattle()
    {
        if (playerData == null || battleState == null || enemyData == null)
        {
            Debug.LogError("BattleSystem is not properly set up.");
            return;
        }

        // Make sure permanent player stats are up to date.
        playerData.CalculateStats();

        battleState.playerCurrentHP = playerData.stats.hp;
        battleState.enemyCurrentHP = enemyData.maxHP;

        battleState.playerAttackTimer = 0f;
        battleState.enemyAttackTimer = 0f;

        battleState.battleRunning = true;
        battleState.battleResult = BattleResult.None;
        battleState.rewardGranted = false;

        Debug.Log(
            $"Battle started: Player HP {battleState.playerCurrentHP} " +
            $"vs {enemyData.enemyName} HP {battleState.enemyCurrentHP}"
        );
    }

    public void Tick(float deltaTime)
    {
        if (battleState == null || !battleState.battleRunning)
        {
            return;
        }

        battleState.playerAttackTimer += deltaTime;
        battleState.enemyAttackTimer += deltaTime;

        float playerAttackInterval = GetAttackInterval(playerData.stats.speed);
        float enemyAttackInterval = GetAttackInterval(enemyData.speed);

        // Player attacks first if both become ready on the same frame.
        if (battleState.playerAttackTimer >= playerAttackInterval)
        {
            battleState.playerAttackTimer -= playerAttackInterval;

            PlayerAttack();

            if (!battleState.battleRunning)
            {
                return;
            }
        }

        if (battleState.enemyAttackTimer >= enemyAttackInterval)
        {
            battleState.enemyAttackTimer -= enemyAttackInterval;

            EnemyAttack();
        }
    }

    private void PlayerAttack()
    {
        int damage = CalculateDamage(
            playerData.stats.attack,
            enemyData.defense
        );

        battleState.enemyCurrentHP -= damage;

        if (battleState.enemyCurrentHP < 0)
        {
            battleState.enemyCurrentHP = 0;
        }

        Debug.Log(
            $"Player dealt {damage} damage. " +
            $"{enemyData.enemyName} HP: {battleState.enemyCurrentHP}/{enemyData.maxHP}"
        );

        CheckBattleEnd();
    }

    private void EnemyAttack()
    {
        int damage = CalculateDamage(
            enemyData.attack,
            playerData.stats.defense
        );

        battleState.playerCurrentHP -= damage;

        if (battleState.playerCurrentHP < 0)
        {
            battleState.playerCurrentHP = 0;
        }

        Debug.Log(
            $"{enemyData.enemyName} dealt {damage} damage. " +
            $"Player HP: {battleState.playerCurrentHP}/{playerData.stats.hp}"
        );

        CheckBattleEnd();
    }

    private int CalculateDamage(int attackerAttack, int defenderDefense)
    {
        return Mathf.Max(1, attackerAttack - defenderDefense);
    }

    private float GetAttackInterval(int speed)
    {
        return 3f / Mathf.Max(1f, speed * 0.2f);
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

            RewardVictory();

            Debug.Log("Battle result: Victory");
            return;
        }

        if (battleState.playerCurrentHP <= 0)
        {
            battleState.battleRunning = false;
            battleState.battleResult = BattleResult.Defeat;

            Debug.Log("Battle result: Defeat");
        }
    }

    private void RewardVictory()
    {
        if (battleState.rewardGranted)
        {
            return;
        }

        playerData.energy += enemyData.energyReward;
        battleState.rewardGranted = true;

        Debug.Log(
            $"Victory reward: +{enemyData.energyReward} Energy. " +
            $"Current Energy: {playerData.energy}"
        );
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