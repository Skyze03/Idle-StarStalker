using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CollectionSystem : MonoBehaviour
{
    public Button collectButton;
    public TMP_Text energyText;
    public Slider progressBar;  // 新增：进度条
    
    private bool isCollecting = false;
    
    void Start()
    {
        collectButton.onClick.AddListener(StartCollecting);
        
        // 隐藏进度条（如果存在）
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
            
        UpdateUI();
    }
    
    // 修改：改为协程方式
    void StartCollecting()
    {
        if (isCollecting) return;
        StartCoroutine(CollectEnergyCoroutine());
    }
    
    IEnumerator CollectEnergyCoroutine()
    {
        isCollecting = true;
        collectButton.interactable = false;
        
        DebugLogButtonClick("采集按钮");
        DebugLog("采集", "开始采集能量...读条1秒");
        
        // 显示进度条
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f;
        }
        
        // 1秒读条
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            
            // 更新进度条
            if (progressBar != null)
            {
                progressBar.value = timer / 1f;
            }
            
            yield return null;
        }
        
        // 采集完成，获得奖励
        CollectEnergyReward();
        
        // 隐藏进度条
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
        
        collectButton.interactable = true;
        isCollecting = false;
        
        DebugLog("采集", "采集完成");
    }
    
    void CollectEnergyReward()
    {
        int energyGained = 10;
        int oldEnergy = GameManager.Instance.playerData.energy;
        GameManager.Instance.playerData.energy += energyGained;
        
        DebugLogDataChange("能量", oldEnergy, GameManager.Instance.playerData.energy);
        DebugLog("采集", $"获得 {energyGained} 点能量");
        
        // 符文掉落概率20%
        bool gotRune = Random.value < 0.2f;
        DebugLogProbability("符文掉落", 0.2f, gotRune);
        
        if (gotRune)
        {
            Rune newRune = GenerateRandomRune();
    
            // 添加到背包管理器
            if (InventoryManager.Instance != null)
            {
                bool added = InventoryManager.Instance.AddRune(newRune);
                if (added)
                {
                    DebugLogger.Log("物品", $"获得符文: {newRune.name}");
                }
            }
            else
            {
                DebugLogger.Log("错误", "InventoryManager未找到！");
            }
        }
        
        UpdateUI();
    }
    
    Rune GenerateRandomRune()
    {
        Rune rune = new Rune();
        rune.color = (RuneColor)Random.Range(0, 3);
        rune.type = (RuneType)Random.Range(0, 3);
        rune.name = $"{rune.color}{rune.type}符文";
        
        DebugLog("系统", $"生成符文详情: 颜色={rune.color}, 类型={rune.type}, 编码={rune.GetCode()}");
        
        return rune;
    }
    
    void UpdateUI()
    {
        energyText.text = $"能量: {GameManager.Instance.playerData.energy}";
    }
    
    // DebugLogger包装方法（与MeditationSystem相同）
    void DebugLog(string category, string message, bool showTime = true)
    {
        if (System.Type.GetType("DebugLogger") != null)
        {
            System.Reflection.MethodInfo method = System.Type.GetType("DebugLogger")
                .GetMethod("Log", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (method != null)
                method.Invoke(null, new object[] { category, message, showTime });
        }
        else
        {
            Debug.Log($"[{category}] {message}");
        }
    }
    
    void DebugLogButtonClick(string buttonName)
    {
        if (System.Type.GetType("DebugLogger") != null)
        {
            System.Reflection.MethodInfo method = System.Type.GetType("DebugLogger")
                .GetMethod("LogButtonClick", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (method != null)
                method.Invoke(null, new object[] { buttonName });
        }
    }
    
    void DebugLogDataChange(string item, int oldValue, int newValue)
    {
        if (System.Type.GetType("DebugLogger") != null)
        {
            System.Reflection.MethodInfo method = System.Type.GetType("DebugLogger")
                .GetMethod("LogDataChange", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (method != null)
                method.Invoke(null, new object[] { item, oldValue, newValue });
        }
        else
        {
            int diff = newValue - oldValue;
            Debug.Log($"{item}: {oldValue} → {newValue} ({(diff > 0 ? "+" : "")}{diff})");
        }
    }
    
    void DebugLogProbability(string eventName, float probability, bool success)
    {
        if (System.Type.GetType("DebugLogger") != null)
        {
            System.Reflection.MethodInfo method = System.Type.GetType("DebugLogger")
                .GetMethod("LogProbability", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (method != null)
                method.Invoke(null, new object[] { eventName, probability, success });
        }
        else
        {
            Debug.Log($"{eventName}: {probability*100}%概率 → {(success ? "成功" : "失败")}");
        }
    }
}