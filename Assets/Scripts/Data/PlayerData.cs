using System;

[Serializable]
public class PlayerData
{
    public int level = 1;
    public int exp = 0;
    public int energy = 0;

    public int bodyLevel = 1;

    public void CalculateStats()
    {
        // 现在先留空，后面再扩展真正属性计算
    }

    public int GetRequiredExp()
    {
        return level * 100;
    }

    public int GetBodyUpgradeCost()
    {
        return bodyLevel * 10;
    }
}