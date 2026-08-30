using System;
using System.Collections.Generic;

public enum UltimateEffectType
{
    DamageMultiplier,
    DefenseScaling,
    RageRefund,
    MultiHit,
    AgilityBuff
}

[Serializable]
public class UltimateData
{
    public string id;
    public string ultimateName;
    public string description;
    public UltimateEffectType effectType;
    public float power;
    public int hitCount;

    public UltimateData(string id, string ultimateName, string description,
        UltimateEffectType effectType, float power, int hitCount = 1)
    {
        this.id = id;
        this.ultimateName = ultimateName;
        this.description = description;
        this.effectType = effectType;
        this.power = power;
        this.hitCount = hitCount;
    }

    public static UltimateData CreateStarBurst() => GetById("star_burst");

    public static UltimateData GetById(string id)
    {
        foreach (UltimateData ultimate in GetAll())
            if (ultimate.id == id) return ultimate;
        return CreateCatalog()[0];
    }

    public static List<UltimateData> GetAll() => CreateCatalog();

    private static List<UltimateData> CreateCatalog()
    {
        return new List<UltimateData>
        {
            new UltimateData("star_burst", "Star Burst",
                "Deals 250% normal damage.", UltimateEffectType.DamageMultiplier, 2.5f),
            new UltimateData("iron_retaliation", "Iron Retaliation",
                "Deals damage based on Attack plus 180% Defense.", UltimateEffectType.DefenseScaling, 1.8f),
            new UltimateData("rapid_nova", "Rapid Nova",
                "Deals 180% normal damage and keeps 40 Rage.", UltimateEffectType.RageRefund, 1.8f),
            new UltimateData("meteor_flurry", "Meteor Flurry",
                "Strikes 3 times for 90% normal damage per hit.", UltimateEffectType.MultiHit, 0.9f, 3),
            new UltimateData("swift_ascension", "Swift Ascension",
                "Deals no damage. +50% Agility for the next 3 actions.", UltimateEffectType.AgilityBuff, 0.5f)
        };
    }
}
