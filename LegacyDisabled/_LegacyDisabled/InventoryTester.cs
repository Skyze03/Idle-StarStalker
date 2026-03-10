using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryTester : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 背包系统测试开始 ===");
        
        // 测试1：检查InventoryManager是否存在
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("❌ InventoryManager实例不存在！");
            
            // 尝试创建
            GameObject imObj = new GameObject("InventoryManager");
            imObj.AddComponent<InventoryManager>();
            Debug.Log("已创建InventoryManager实例");
        }
        else
        {
            Debug.Log("✅ InventoryManager实例存在");
        }
        
        // 测试2：检查背包UI
        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null)
        {
            Debug.LogError("❌ 未找到InventoryUI组件");
        }
        else
        {
            Debug.Log("✅ 找到InventoryUI组件");
            Debug.Log($"背包面板: {inventoryUI.inventoryPanel != null}");
            Debug.Log($"符文内容容器: {inventoryUI.runesContent != null}");
            Debug.Log($"符文预制体: {inventoryUI.runeItemPrefab != null}");
        }
        
        // 测试3：添加测试物品
        if (InventoryManager.Instance != null)
        {
            Debug.Log("添加测试符文...");
            for (int i = 0; i < 5; i++)
            {
                Rune testRune = new Rune();
                testRune.color = (RuneColor)Random.Range(0, 3);
                testRune.type = (RuneType)Random.Range(0, 3);
                InventoryManager.Instance.AddRune(testRune);
            }
            Debug.Log($"当前符文数量: {InventoryManager.Instance.runes.Count}");
        }
        
        // 测试4：手动打开背包面板
        if (inventoryUI != null && inventoryUI.inventoryPanel != null)
        {
            inventoryUI.inventoryPanel.SetActive(true);
            inventoryUI.UpdateInventoryUI();
            Debug.Log("已强制打开并刷新背包面板");
        }
        
        Debug.Log("=== 背包系统测试结束 ===");
    }
    
    void Update()
    {
        // 快捷键测试
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }
    
    void ToggleInventory()
    {
        InventoryUI ui = FindObjectOfType<InventoryUI>();
        if (ui != null)
        {
            ui.ToggleInventory();
            Debug.Log("按B键切换背包显示");
        }
    }
}