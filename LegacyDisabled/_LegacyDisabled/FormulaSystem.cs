using UnityEngine;
using System.Text;
public static class FormulaSystem
{
    // 行动条速度公式：基础速度 + 敏捷 × 系数
    public static float GetActionSpeed(int agility)
    {
        float baseSpeed = 1.0f;
        float agilityFactor = 0.02f;
        return baseSpeed + (agility * agilityFactor);
    }
    
    // 攻击获得怒气公式：伤害 × 系数 + 固定值
    public static float GetRageFromDamage(float damage)
    {
        // 伤害的5%转化为怒气
        return damage * 0.05f + 5f;
    }
    
    // 计算伤害公式：攻击力 × 力量系数
    public static float CalculateDamage(int attack, int strength)
    {
        float strengthMultiplier = 1.0f + (strength * 0.01f);
        return attack * strengthMultiplier;
    }
    
    // 升级消耗公式（用于身体部位）
    public static int GetUpgradeCost(int baseCost, int currentLevel)
    {
        // 公式：基础消耗 × (1.15^当前等级)
        return Mathf.RoundToInt(baseCost * Mathf.Pow(1.15f, currentLevel));
    }





    // 调试方法：显示所有公式计算结果
    public static void DebugAllFormulas(int level, int agility, int attack, int strength)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== 公式系统调试 ===");
        sb.AppendLine($"等级 {level} → 升级需要经验: {GetRequiredExp(level)}");
        sb.AppendLine($"敏捷 {agility} → 行动速度: {GetActionSpeed(agility):F2}");
        sb.AppendLine($"攻击 {attack}, 力量 {strength} → 伤害: {CalculateDamage(attack, strength):F0}");
        
        // 测试不同等级的部位升级消耗
        sb.AppendLine("\n部位升级消耗:");
        for (int i = 1; i <= 5; i++)
        {
            sb.AppendLine($"  等级{i}→{i+1}: {GetUpgradeCost(50, i)}能量");
        }
        
        DebugLogger.Log("系统", sb.ToString());
    }
    
    // 方便调用的升级经验公式
    public static int GetRequiredExp(int level)
    {
        return Mathf.RoundToInt(100 * Mathf.Pow(1.5f, level - 1));
    }
}