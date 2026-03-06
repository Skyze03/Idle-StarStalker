using UnityEngine;
using System.Text;
using System.Collections.Generic;

public static class DebugLogger
{
    // 颜色定义
    private static Dictionary<string, string> colorMap = new Dictionary<string, string>()
    {
        {"冥想", "#4ECDC4"},    // 青绿色
        {"采集", "#FF6B6B"},    // 红色
        {"升级", "#FFD166"},    // 黄色
        {"战斗", "#06D6A0"},    // 绿色
        {"系统", "#118AB2"},    // 蓝色
        {"错误", "#EF476F"},    // 粉红色
        {"物品", "#9B5DE5"}     // 紫色
    };
    
    // 主日志方法
    public static void Log(string category, string message, bool showTime = true)
    {
        string color = GetColor(category);
        string timeStamp = showTime ? $"[{System.DateTime.Now:HH:mm:ss}] " : "";
        string formattedMessage = $"{timeStamp}<color={color}>[{category}]</color> {message}";
        
        Debug.Log(formattedMessage);
    }
    
    // 数据变化日志
    public static void LogDataChange(string item, int oldValue, int newValue)
    {
        int diff = newValue - oldValue;
        string change = diff > 0 ? $"+{diff}" : $"{diff}";
        Log("系统", $"{item}: {oldValue} → {newValue} ({change})");
    }
    
    // 概率事件日志
    public static void LogProbability(string eventName, float probability, bool success)
    {
        string result = success ? "成功" : "失败";
        string color = success ? "green" : "gray";
        Log("系统", $"{eventName}: {probability*100}%概率 → <color={color}>{result}</color>");
    }
    
    // 按钮点击日志
    public static void LogButtonClick(string buttonName)
    {
        Log("系统", $"点击按钮: {buttonName}");
    }
    
    private static string GetColor(string category)
    {
        return colorMap.ContainsKey(category) ? colorMap[category] : "#FFFFFF";
    }
}