using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
public class InventoryUI : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject inventoryPanel;
    public Transform runesContent;      // 符文容器的Content对象
    public Transform fragmentsContent;  // 碎片容器的Content对象
    public Button closeButton;
    
    [Header("预制体")]
    public GameObject runeItemPrefab;
    public GameObject fragmentItemPrefab;
    
    [Header("统计文本")]
    public TextMeshProUGUI runesSummaryText;
    public TextMeshProUGUI fragmentsSummaryText;
    
    // 当前显示的物品列表
    private List<GameObject> currentRuneItems = new List<GameObject>();
    private List<GameObject> currentFragmentItems = new List<GameObject>();
    
    void Start()
    {
        // 绑定关闭按钮
        if (closeButton != null)
            closeButton.onClick.AddListener(() => inventoryPanel.SetActive(false));
        
        // 订阅背包变化事件
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateInventoryUI;
        }
        
        // 初始隐藏面板
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        
        DebugLogger.Log("UI", "背包UI控制器初始化完成");
    }
    
    void OnDestroy()
    {
        // 取消事件订阅
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateInventoryUI;
        }
    }
    
    // 打开/关闭背包
    public void ToggleInventory()
    {
        bool isActive = inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!isActive);
        
        if (!isActive)
        {
            UpdateInventoryUI(); // 打开时刷新
        }
    }
    
    // 更新整个背包UI
    public void UpdateInventoryUI()
    {
        if (!inventoryPanel.activeSelf) return;
        
        ClearCurrentItems();
        UpdateRunesDisplay();
        UpdateFragmentsDisplay();
        UpdateSummaryText();
        
        DebugLogger.Log("背包", "背包UI已刷新");
    }
    
    // 更新符文显示
    void UpdateRunesDisplay()
    {
        if (InventoryManager.Instance == null || runeItemPrefab == null || runesContent == null)
            return;
        
        // 获取按类型统计的符文
        var runeCounts = InventoryManager.Instance.GetRuneCountByType();
        
        foreach (var kvp in runeCounts)
        {
            // 创建符文物品UI
            GameObject runeItem = Instantiate(runeItemPrefab, runesContent);
            currentRuneItems.Add(runeItem);
            
            // 获取组件
            Image icon = runeItem.GetComponentInChildren<Image>();
            TextMeshProUGUI[] texts = runeItem.GetComponentsInChildren<TextMeshProUGUI>();
            
            // 设置显示信息
            if (texts.Length > 0) texts[0].text = kvp.Key; // 符文代码
            if (texts.Length > 1) texts[1].text = $"×{kvp.Value}"; // 数量
            
            // 设置颜色（简化版，后期可以换成图标）
            if (icon != null)
            {
                // 根据符文代码设置颜色
                if (kvp.Key.StartsWith("R")) icon.color = Color.red;
                else if (kvp.Key.StartsWith("Y")) icon.color = Color.yellow;
                else if (kvp.Key.StartsWith("B")) icon.color = Color.blue;
            }
            
            // 添加点击事件（用于组合器选择）
            Button btn = runeItem.GetComponent<Button>();
            if (btn != null)
            {
                // 这里需要找到对应的符文对象（简化处理）
                // 实际应该存储符文ID或引用
                btn.onClick.AddListener(() => OnRuneClicked(kvp.Key));
            }
        }
    }
    
    // 更新碎片显示
    void UpdateFragmentsDisplay()
    {
        if (InventoryManager.Instance == null || fragmentItemPrefab == null || fragmentsContent == null)
            return;
        
        // 显示冥想碎片
        if (InventoryManager.Instance.meditationFragments.Count > 0)
        {
            GameObject fragItem = Instantiate(fragmentItemPrefab, fragmentsContent);
            currentFragmentItems.Add(fragItem);
            
            TextMeshProUGUI[] texts = fragItem.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0) texts[0].text = "冥想碎片";
            if (texts.Length > 1) texts[1].text = $"×{InventoryManager.Instance.meditationFragments.Count}";
        }
        
        // 显示记忆碎片
        if (InventoryManager.Instance.memoryFragments.Count > 0)
        {
            GameObject fragItem = Instantiate(fragmentItemPrefab, fragmentsContent);
            currentFragmentItems.Add(fragItem);
            
            TextMeshProUGUI[] texts = fragItem.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0) texts[0].text = "记忆碎片";
            if (texts.Length > 1) texts[1].text = $"×{InventoryManager.Instance.memoryFragments.Count}";
        }
    }
    
    // 更新摘要文本
    void UpdateSummaryText()
    {
        if (InventoryManager.Instance == null) return;
        
        if (runesSummaryText != null)
        {
            int totalRunes = InventoryManager.Instance.GetTotalRuneCount();
            runesSummaryText.text = $"符文: {totalRunes}/50";
        }
        
        if (fragmentsSummaryText != null)
        {
            int totalFrags = InventoryManager.Instance.meditationFragments.Count + 
                            InventoryManager.Instance.memoryFragments.Count;
            fragmentsSummaryText.text = $"碎片: {totalFrags}/100";
        }
    }
    
    // 清理当前显示的物品
    void ClearCurrentItems()
    {
        foreach (var item in currentRuneItems) Destroy(item);
        foreach (var item in currentFragmentItems) Destroy(item);
        
        currentRuneItems.Clear();
        currentFragmentItems.Clear();
    }
    
    // 符文被点击（用于组合器选择）
    void OnRuneClicked(string runeCode)
    {
        DebugLogger.Log("背包", $"选中符文: {runeCode}");
        
        // 通知组合器系统（后续实现）
        var combiner = FindObjectOfType<CombinerSystem>();
        if (combiner != null)
        {
            // 这里需要传递符文对象，简化处理
            Rune rune = InventoryManager.Instance.FindRune(runeCode);
            if (rune != null)
            {
                combiner.SelectRune(rune);
            }
        }
    }
    
    // 调试：添加测试物品
    public void AddTestItems()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddTestItems();
        }
    }


    void Update()
    {
        // 获取当前键盘状态
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        // F8: 添加测试物品
        if (keyboard.f8Key.wasPressedThisFrame)
        {
            AddTestItems();
        }
        
        // F9: 清空背包
        if (keyboard.f9Key.wasPressedThisFrame)
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ClearInventory();
                UpdateInventoryUI(); // 刷新显示
            }
        }
        
        // F10: 刷新背包显示
        if (keyboard.f10Key.wasPressedThisFrame)
        {
            UpdateInventoryUI();
        }
        
        // 添加其他调试快捷键（可选）
        // I键：切换背包显示
        if (keyboard.iKey.wasPressedThisFrame && keyboard.ctrlKey.isPressed)
        {
            ToggleInventory();
        }
    }
}