using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.InputSystem; 

public class DebugPanel : MonoBehaviour
{
    // UI组件引用
    public Button debugInfoButton;
    public Button testUpgradeButton;
    public Button clearLogButton;
    public Button testMeditateButton;
    public Button testCollectButton;
    
    void Start()
    {
        // 绑定按钮事件
        if (debugInfoButton != null)
            debugInfoButton.onClick.AddListener(ShowDebugInfo);
        
        if (testUpgradeButton != null)
            testUpgradeButton.onClick.AddListener(TestQuickUpgrade);
        
        if (clearLogButton != null)
            clearLogButton.onClick.AddListener(ClearConsole);
        
        if (testMeditateButton != null)
            testMeditateButton.onClick.AddListener(TestMeditate);
        
        if (testCollectButton != null)
            testCollectButton.onClick.AddListener(TestCollect);
        
        Debug.Log("调试面板初始化完成");
    }
    
    void ShowDebugInfo()
    {
        Debug.Log("点击：显示状态按钮");
        
        // 先确保GameManager存在
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager未找到！");
            return;
        }
        
        PlayerData data = GameManager.Instance.playerData;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== 玩家状态 ===");
        sb.AppendLine($"等级: Lv{data.level}");
        sb.AppendLine($"经验: {data.exp}/{data.GetRequiredExp()}");
        sb.AppendLine($"能量: {data.energy}");
        
        Debug.Log(sb.ToString());
    }
    
    void TestQuickUpgrade()
    {
        Debug.Log("点击：快速升级按钮");
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager未找到！");
            return;
        }
        
        PlayerData data = GameManager.Instance.playerData;
        int oldLevel = data.level;
        
        // 直接添加足够升级的经验
        int needed = data.GetRequiredExp() - data.exp;
        if (needed > 0)
        {
            data.exp += needed;
        }
        
        // 调用升级检查（如果PlayerData有这个方法）
        Debug.Log($"尝试升级：当前等级{oldLevel}，经验{data.exp}");
    }
    
    void TestMeditate()
    {
        Debug.Log("点击：测试冥想按钮");
        Debug.Log("(实际功能需要MeditationSystem)");
    }
    
    void TestCollect()
    {
        Debug.Log("点击：测试采集按钮");
        Debug.Log("(实际功能需要CollectionSystem)");
    }
    
    void ClearConsole()
    {
        Debug.Log("点击：清空控制台按钮");
        Debug.Log("=== 清空分隔线 ===");
    }
    
    // 添加快捷键支持 - 使用Input System
    void Update()
    {
        // 获取当前键盘
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return; // 确保键盘存在
        
        // F1: 显示调试信息
        if (keyboard.f1Key.wasPressedThisFrame)
        {
            ShowDebugInfo();
        }
        
        // F2: 快速升级
        if (keyboard.f2Key.wasPressedThisFrame)
        {
            TestQuickUpgrade();
        }
        
        // F3: 清空控制台
        if (keyboard.f3Key.wasPressedThisFrame)
        {
            ClearConsole();
        }
        
        // F4: 测试冥想
        if (keyboard.f4Key.wasPressedThisFrame)
        {
            TestMeditate();
        }
        
        // F5: 测试采集
        if (keyboard.f5Key.wasPressedThisFrame)
        {
            TestCollect();
        }
        
        // 添加一些额外的调试快捷键
        // R: 重置游戏数据
        if (keyboard.rKey.wasPressedThisFrame)
        {
            GameManager.Instance.DebugResetGame();
            DebugLogger.Log("调试", "游戏数据已重置 (按R键)");
        }
        
        // E: 增加能量
        if (keyboard.eKey.wasPressedThisFrame)
        {
            GameManager.Instance.playerData.energy += 100;
            DebugLogger.Log("调试", "增加100能量 (按E键)");
        }
    }
}