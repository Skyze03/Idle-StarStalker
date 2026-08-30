using System;
using System.Collections.Generic;

[Flags]
public enum EnemyTrait
{
    None = 0,
    Frenzy = 1 << 0,
    Bulwark = 1 << 1,
    Swift = 1 << 2,
    Sage = 1 << 3
}

public static class EnemyTraitUtility
{
    public static string GetDisplayName(EnemyTrait traits)
    {
        if (traits == EnemyTrait.None) return "Balanced";
        List<string> names = new List<string>();
        if ((traits & EnemyTrait.Frenzy) != 0) names.Add("Frenzy");
        if ((traits & EnemyTrait.Bulwark) != 0) names.Add("Bulwark");
        if ((traits & EnemyTrait.Swift) != 0) names.Add("Swift");
        if ((traits & EnemyTrait.Sage) != 0) names.Add("Sage");
        return string.Join(" + ", names);
    }

    public static string GetDescription(EnemyTrait traits)
    {
        if (traits == EnemyTrait.None) return "No special passive.";
        List<string> effects = new List<string>();
        if ((traits & EnemyTrait.Frenzy) != 0)
            effects.Add("ATK +15%; normal attacks gain +5 Rage");
        if ((traits & EnemyTrait.Bulwark) != 0)
            effects.Add("DEF +25%; gains +8 Rage when damaged");
        if ((traits & EnemyTrait.Swift) != 0)
            effects.Add("AGI +25%");
        if ((traits & EnemyTrait.Sage) != 0)
            effects.Add("WIS +30%");
        return string.Join("; ", effects) + ".";
    }
}
