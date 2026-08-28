using System;
using UnityEngine.Serialization;

[Serializable]
public class PlayerStats
{
    public int meditationExpBonus = 0;
    public int collectionEnergyBonus = 0;

    public int hp = 0;
    public int attack = 0;
    public int defense = 0;
    [FormerlySerializedAs("speed")]
    public int agility = 0;
    public int wisdom = 0;
}
