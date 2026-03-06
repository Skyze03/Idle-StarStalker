using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    // 单例
    public static GameInputManager Instance;
    
    // 输入Action
    private PlayerInputActions playerInputActions;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupInputActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void SetupInputActions()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        
        // 绑定调试快捷键
        playerInputActions.Debug.AddTestItems.performed += ctx => OnAddTestItems();
        playerInputActions.Debug.ClearInventory.performed += ctx => OnClearInventory();
        playerInputActions.Debug.RefreshUI.performed += ctx => OnRefreshUI();
        playerInputActions.Debug.ToggleInventory.performed += ctx => OnToggleInventory();
    }
    
    // 调试功能回调
    void OnAddTestItems()
    {
        var inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI != null) inventoryUI.AddTestItems();
    }
    
    void OnClearInventory()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.ClearInventory();
    }
    
    void OnRefreshUI()
    {
        var inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI != null) inventoryUI.UpdateInventoryUI();
    }
    
    void OnToggleInventory()
    {
        var inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI != null) inventoryUI.ToggleInventory();
    }
    
    // 获取当前键盘（方便其他脚本使用）
    public Keyboard GetKeyboard()
    {
        return Keyboard.current;
    }
    
    void OnDestroy()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Disable();
            playerInputActions.Dispose();
        }
    }
}