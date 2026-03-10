using UnityEngine;
[System.Serializable]
public class BodyPart
{
    public string partName;
    public int level = 0;
    public string[] affectedStats; // 影响的属性
    
    // 升级消耗公式：基础消耗 * (1.2^当前等级)
    public int GetUpgradeCost()
    {
        int baseCost = 50;
        return Mathf.RoundToInt(baseCost * Mathf.Pow(1.2f, level));
    }
    
    // 获取某属性的加成值
    public int GetStatValue(string statName)
    {
        foreach (string stat in affectedStats)
        {
            if (stat == statName)
            {
                // 每级提供5点属性
                return level * 5;
            }
        }
        return 0;
    }
    
    public BodyPart(string name, string[] stats)
    {
        partName = name;
        affectedStats = stats;
    }
}