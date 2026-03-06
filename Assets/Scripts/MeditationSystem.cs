using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MeditationSystem : MonoBehaviour
{
    public Button meditateButton;
    //public TMP_Text levelText;
    //public TMP_Text expText;
    public Slider expSlider;
    public TMPro.TextMeshProUGUI levelText;
    public TMPro.TextMeshProUGUI expText;
    // 新增：自动冥想相关
    public Button autoMeditateButton;
    public Text autoButtonText;
    public GameObject autoMeditateIndicator;  // 自动冥想指示灯（可选）
    
    private bool isMeditating = false;
    private bool autoMeditateEnabled = false;
    private Coroutine autoMeditateCoroutine;
    
    void Start()
    {
        // 隐藏采集面板
        GameObject.Find("CollectionPanel")?.SetActive(false);
        
        // 原有：手动冥想按钮
        meditateButton.onClick.AddListener(StartMeditation);
        
        // 新增：自动冥想按钮初始化
        if (autoMeditateButton != null)
        {
            autoMeditateButton.onClick.AddListener(ToggleAutoMeditate);
            autoMeditateButton.interactable = false;  // 默认不可用
            
            // 检查是否已经达到5级
            CheckAutoMeditateUnlock();
        }
        
        // 隐藏指示灯（如果存在）
        if (autoMeditateIndicator != null)
            autoMeditateIndicator.SetActive(false);
        
        UpdateUI();
    }
    
    // 新增：检查是否解锁自动冥想
    void CheckAutoMeditateUnlock()
    {
        if (GameManager.Instance.playerData.level >= 5)
        {
            autoMeditateButton.interactable = true;
            DebugLog("系统", "已达到5级，解锁自动冥想功能！");
        }
    }
    
    // 原有：开始手动冥想
    public void StartMeditation()
    {
        if (isMeditating) return;
        StartCoroutine(MeditateCoroutine());
    }
    
    // 原有：冥想协程（基本保持不变）
    IEnumerator MeditateCoroutine()
    {
        isMeditating = true;
        meditateButton.interactable = false;
        
        DebugLogButtonClick("冥想按钮");
        DebugLog("冥想", "开始冥想...读条1秒");
        
        // 显示读条（1秒）
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            // 这里可以更新读条UI
            yield return null;
        }
        
        // 获得经验和检查升级（使用新方法，复用逻辑）
        PerformMeditation();
        
        meditateButton.interactable = true;
        isMeditating = false;
        
        DebugLog("冥想", "冥想完成，按钮恢复可用");
    }
    
    // 新增：执行一次冥想的核心逻辑（可被手动和自动调用）
    void PerformMeditation()
    {
        // 获得经验
        int expGained = 20;
        int oldExp = GameManager.Instance.playerData.exp;
        GameManager.Instance.playerData.exp += expGained;
        
        DebugLogDataChange("经验值", oldExp, GameManager.Instance.playerData.exp);
        DebugLog("冥想", $"获得 {expGained} 点经验");
        
        // 检查升级
        int oldLevel = GameManager.Instance.playerData.level;
        CheckLevelUp();
        
        if (GameManager.Instance.playerData.level > oldLevel)
        {
            DebugLog("升级", $"恭喜！升级到 {GameManager.Instance.playerData.level} 级！");
            
            // 检查是否解锁自动冥想
            if (GameManager.Instance.playerData.level >= 5 && autoMeditateButton != null)
            {
                autoMeditateButton.interactable = true;
            }
        }
        
        // 碎片掉落概率（10%基础 + 每级1%）
        float fragmentChance = 0.1f + (GameManager.Instance.playerData.level * 0.01f);
        bool gotFragment = Random.value < fragmentChance;
        
        DebugLogProbability("记忆碎片掉落", fragmentChance, gotFragment);
        
        if (gotFragment)
        {
            Fragment fragment = new Fragment();
            fragment.fragmentType = FragmentType.Memory; // 或Meditation，根据你的设计
            
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddFragment(fragment);
            }
        }
        
        UpdateUI();
    }
    
    // 新增：自动冥想开关
    void ToggleAutoMeditate()
    {
        autoMeditateEnabled = !autoMeditateEnabled;
        
        if (autoMeditateEnabled)
        {
            DebugLogButtonClick("开启自动冥想");
            DebugLog("冥想", "自动冥想已开启");
            
            // 更新按钮文本
            if (autoButtonText != null)
                autoButtonText.text = "停止自动";
            
            // 显示指示灯
            if (autoMeditateIndicator != null)
                autoMeditateIndicator.SetActive(true);
            
            // 开始自动冥想协程
            autoMeditateCoroutine = StartCoroutine(AutoMeditateCoroutine());
        }
        else
        {
            DebugLogButtonClick("关闭自动冥想");
            DebugLog("冥想", "自动冥想已关闭");
            
            // 更新按钮文本
            if (autoButtonText != null)
                autoButtonText.text = "自动冥想";
            
            // 隐藏指示灯
            if (autoMeditateIndicator != null)
                autoMeditateIndicator.SetActive(false);
            
            // 停止协程
            if (autoMeditateCoroutine != null)
            {
                StopCoroutine(autoMeditateCoroutine);
                autoMeditateCoroutine = null;
            }
        }
    }
    
    // 新增：自动冥想协程
    IEnumerator AutoMeditateCoroutine()
    {
        DebugLog("冥想", "自动冥想循环开始");
        
        while (autoMeditateEnabled)
        {
            // 等待1秒（模拟读条时间）
            yield return new WaitForSeconds(1f);
            
            // 执行一次冥想（跳过动画）
            if (!isMeditating)  // 确保手动冥想不在进行中
            {
                PerformMeditation();
                DebugLog("冥想", "自动冥想完成一次循环");
            }
            
            // 短暂延迟，防止过载
            yield return null;
        }
        
        DebugLog("冥想", "自动冥想循环结束");
    }
    
    // 原有：检查升级（基本保持不变）
    void CheckLevelUp()
    {
        PlayerData data = GameManager.Instance.playerData;
        
        while (data.exp >= data.GetRequiredExp())
        {
            int required = data.GetRequiredExp();
            int oldLevel = data.level;
            
            data.exp -= required;
            data.level++;
            
            // 计算属性变化前的值
            int oldHP = data.MaxHP;
            int oldAttack = data.Attack;
            
            data.CalculateStats();
            
            // 记录属性变化
            DebugLogDataChange("生命值", oldHP, data.MaxHP);
            DebugLogDataChange("攻击力", oldAttack, data.Attack);
            DebugLogDataChange("力量", 0, data.Strength);
            DebugLogDataChange("敏捷", 0, data.Agility);
            DebugLogDataChange("智慧", 0, data.Intelligence);
            
            // 5级解锁自动冥想
            if (data.level == 5)
            {
                DebugLog("系统", "★★★★★ 解锁自动冥想功能！ ★★★★★");
            }
            
            DebugLog("系统", $"升级详情: Lv{oldLevel}→Lv{data.level}, 消耗{required}经验");
            DebugLog("系统", $"下一级需要: {data.GetRequiredExp()}经验");
        }
    }
    
    // 原有：更新UI
    void UpdateUI()
    {
        if (GameManager.Instance == null) 
        {
            Debug.LogError("GameManager为空！");
            return;
        }
        
        PlayerData data = GameManager.Instance.playerData;
        
        // 强制设置明显值用于测试
        if (levelText != null)
        {
            levelText.text = $"等级: {data.level}";
            levelText.color = Color.yellow;  // 黄色，明显可见
            Debug.Log($"设置等级文本: {levelText.text}");
        }
        else
        {
            Debug.LogError("LevelText未连接！");
        }
        
        if (expText != null)
        {
            expText.text = $"经验: {data.exp}/{data.GetRequiredExp()}";
            expText.color = Color.cyan;  // 青色，明显可见
            Debug.Log($"设置经验文本: {expText.text}");
        }
        else
        {
            Debug.LogError("ExpText未连接！");
        }
        
        if (expSlider != null)
        {
            float expPercent = (float)data.exp / data.GetRequiredExp();
            expSlider.value = expPercent;
            
            UpdateExpBarColor(expPercent); // 添加这行
            
            Debug.Log($"设置经验条: {expPercent:P0}");
        }
        else
        {
            Debug.LogError("ExpSlider未连接！");
        }
    }
    
    void UpdateExpBarColor(float percent)
    {
        var fill = expSlider.fillRect?.GetComponent<UnityEngine.UI.Image>();
        if (fill == null) return;
        
        if (percent < 0.3f)
            fill.color = Color.red * 0.8f;          // 暗红色
        else if (percent < 0.7f)
            fill.color = Color.yellow * 0.7f;       // 暗黄色
        else
            fill.color = Color.green * 0.6f;        // 暗绿色
    }
    
    // 新增：DebugLogger的包装方法（保持你现有的反射方式）
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
    
    // 新增：清理协程
    void OnDestroy()
    {
        if (autoMeditateCoroutine != null)
        {
            StopCoroutine(autoMeditateCoroutine);
        }
    }
    
    // 新增：在Update中检查解锁状态（可选）
    void Update()
    {
        // 可以每帧检查是否达到5级（简单方式）
        if (autoMeditateButton != null && !autoMeditateButton.interactable)
        {
            if (GameManager.Instance.playerData.level >= 5)
            {
                autoMeditateButton.interactable = true;
            }
        }
    }
}