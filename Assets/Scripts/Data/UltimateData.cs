using System;

[Serializable]
public class UltimateData
{
    public string ultimateName;
    public float damageMultiplier;

    public UltimateData(string ultimateName, float damageMultiplier)
    {
        this.ultimateName = ultimateName;
        this.damageMultiplier = damageMultiplier;
    }

    public static UltimateData CreateStarBurst()
    {
        return new UltimateData("Star Burst", 2.5f);
    }
}
