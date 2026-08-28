using System;

public enum BattleResult
{
    None,
    Victory,
    Defeat
}

[Serializable]
public class BattleState
{
    // Current HP values only belong to the current battle.
    // They must not overwrite the player's permanent maximum HP.
    public int playerCurrentHP;
    public int enemyCurrentHP;

    // Timers used by the first automatic battle prototype.
    public float playerAttackTimer;
    public float enemyAttackTimer;

    // Current battle status.
    public bool battleRunning;
    public BattleResult battleResult;

    // Prevents victory rewards from being granted more than once.
    public bool rewardGranted;

    public BattleState()
    {
        Reset();
    }

    public void Reset()
    {
        playerCurrentHP = 0;
        enemyCurrentHP = 0;

        playerAttackTimer = 0f;
        enemyAttackTimer = 0f;

        battleRunning = false;
        battleResult = BattleResult.None;
        rewardGranted = false;
    }
}