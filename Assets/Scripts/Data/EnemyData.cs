using System;

[Serializable]
public class EnemyData
{
    public string enemyName;

    public int maxHP;
    public int attack;
    public int defense;
    public int speed;

    public int energyReward;

    public EnemyData(
        string enemyName,
        int maxHP,
        int attack,
        int defense,
        int speed,
        int energyReward)
    {
        this.enemyName = enemyName;
        this.maxHP = maxHP;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.energyReward = energyReward;
    }
}