using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolbarManager : MonoBehaviour
{
    // 单例模式
    public static ToolbarManager Instance;
    
    // 按钮引用
    public Button inventoryButton;
    public Button combinerButton;
    public Button upgradeButton;
    public Button battleButton;
    
    // 对应的功能面板
    public GameObject inventoryPanel;
    public GameObject combinerPanel;
    public GameObject upgradePanel;
    public GameObject battlePanel;
    
    // 当前打开的面板
    private GameObject currentOpenPanel;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景保持
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // 绑定按钮事件
        if (inventoryButton != null)
            inventoryButton.onClick.AddListener(() => TogglePanel(inventoryPanel));
        
        if (combinerButton != null)
            combinerButton.onClick.AddListener(() => TogglePanel(combinerPanel));
        
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(() => TogglePanel(upgradePanel));
        
        if (battleButton != null)
            battleButton.onClick.AddListener(() => TogglePanel(battlePanel));
        
        // 初始化：隐藏所有功能面板
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        
        // 使用DebugLogger（如果存在）
        if (System.Type.GetType("DebugLogger") != null)
        {
            System.Reflection.MethodInfo logMethod = System.Type.GetType("DebugLogger")
                .GetMethod("Log", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (logMethod != null)
                logMethod.Invoke(null, new object[] { "系统", "底部工具栏初始化完成", true });
        }
        else
        {
            Debug.Log("底部工具栏初始化完成");
        }
        // 测试按钮（开发用）
        if (Input.GetKey(KeyCode.LeftControl)) // 按住Ctrl点击背包按钮
        {
            inventoryButton.onClick.AddListener(() => {
                var inventoryUI = FindObjectOfType<InventoryUI>();
                if (inventoryUI != null) inventoryUI.AddTestItems();
            });
        }
    }
    
    void TogglePanel(GameObject panel)
    {
        if (panel == null) return;
        
        // 如果点击的是已打开的面板，则关闭它
        if (currentOpenPanel == panel)
        {
            panel.SetActive(false);
            currentOpenPanel = null;
            Debug.Log($"关闭 {panel.name}");
        }
        else
        {
            // 关闭当前打开的面板
            if (currentOpenPanel != null)
            {
                currentOpenPanel.SetActive(false);
            }
            
            // 打开新面板
            panel.SetActive(true);
            currentOpenPanel = panel;
            Debug.Log($"打开 {panel.name}");
            
            // 如果是背包面板，尝试刷新
            if (panel == inventoryPanel)
            {
                // 方法1：直接查找InventoryUI组件
                InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
                if (inventoryUI != null)
                {
                    inventoryUI.UpdateInventoryUI();
                }
                else
                {
                    // 方法2：或者查找任何有UpdateInventoryUI方法的组件
                    MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
                    foreach (MonoBehaviour mb in allMonoBehaviours)
                    {
                        System.Type type = mb.GetType();
                        System.Reflection.MethodInfo method = type.GetMethod("UpdateInventoryUI");
                        if (method != null)
                        {
                            method.Invoke(mb, null);
                            break;
                        }
                    }
                }
            }
            else if (panel == upgradePanel)
            {
                // 升级面板的刷新（等创建了UpgradeSystem再实现）
                // FindObjectOfType<UpgradeSystem>()?.UpdateUI();
            }
        }
    }
    
    // 外部调用：强制关闭所有面板
    public void CloseAllPanels()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        currentOpenPanel = null;
    }
    
    // 检查是否有面板打开
    public bool IsAnyPanelOpen()
    {
        return currentOpenPanel != null;
    }
}