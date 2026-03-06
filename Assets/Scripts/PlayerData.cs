using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // 基础属性
    public int level = 1;
    public int exp = 0;
    public int energy = 0; // 采集获得的能量
    
    // 计算下一级所需经验（指数曲线）
    public int GetRequiredExp()
    {
        // 公式：100 * (1.5^(level-1))
        return Mathf.RoundToInt(100 * Mathf.Pow(1.5f, level - 1));
    }
    
    // 属性值（由装备部位计算得出）
    public int MaxHP { get; set; }
    public int Attack { get; set; }
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Agility { get; set; }
    
    // 部位数据
    public Dictionary<string, BodyPart> bodyParts = new Dictionary<string, BodyPart>()
    {
        {"Head", new BodyPart("头部", new string[]{"MaxHP", "Intelligence"})},
        {"Hand", new BodyPart("手部", new string[]{"Attack", "Intelligence"})},
        {"Leg", new BodyPart("腿部", new string[]{"Attack", "Agility"})},
        {"Chest", new BodyPart("胸甲", new string[]{"MaxHP", "Strength"})},
        {"Foot", new BodyPart("脚部", new string[]{"MaxHP", "Agility"})},
        {"Weapon", new BodyPart("武器", new string[]{"Attack", "Strength"})}
    };
    
    // 计算总属性
    public void CalculateStats()
    {
        MaxHP = 0; Attack = 0; Strength = 0; Intelligence = 0; Agility = 0;
        
        foreach (var part in bodyParts.Values)
        {
            MaxHP += part.GetStatValue("MaxHP");
            Attack += part.GetStatValue("Attack");
            Strength += part.GetStatValue("Strength");
            Intelligence += part.GetStatValue("Intelligence");
            Agility += part.GetStatValue("Agility");
        }
        
        // 基础值
        MaxHP += 100 + (level * 20);
        Attack += 10 + (level * 2);
    }
}