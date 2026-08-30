using System;
using UnityEngine.Serialization;

[Serializable]
public class EnemyData
{
    public string enemyName;

    public int maxHP;
    public int attack;
    public int defense;
    [FormerlySerializedAs("speed")]
    public int agility;
    public int wisdom;

    public UltimateData equippedUltimate;
    public EnemyTrait traits;

    public EnemyData(
        string enemyName,
        int maxHP,
        int attack,
        int defense,
        int agility,
        int wisdom,
        UltimateData equippedUltimate)
    {
        this.enemyName = enemyName;
        this.maxHP = maxHP;
        this.attack = attack;
        this.defense = defense;
        this.agility = agility;
        this.wisdom = wisdom;
        this.equippedUltimate = equippedUltimate;
    }
}
