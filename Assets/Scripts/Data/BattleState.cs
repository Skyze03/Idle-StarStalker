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

    // Action values fill during battle. An action occurs at 100.
    public float playerActionValue;
    public float enemyActionValue;

    // Rage is shared by the player/enemy combat model and fuels ultimates.
    public float playerRage;
    public float enemyRage;

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

        playerActionValue = 0f;
        enemyActionValue = 0f;
        playerRage = 0f;
        enemyRage = 0f;

        battleRunning = false;
        battleResult = BattleResult.None;
        rewardGranted = false;
    }
}
