using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public PlayerData playerData;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeGame()
    {
        if (playerData == null)
            playerData = new PlayerData();
        
        playerData.CalculateStats();
        Debug.Log("游戏初始化完成！");
    }
    
    // 保存/加载功能（后续实现）
    public void SaveGame() { }
    public void LoadGame() { }



    // 添加这个便捷访问属性
    public static PlayerData PlayerData 
    { 
        get 
        { 
            if (Instance == null) return null;
            return Instance.playerData; 
        } 
    }
    
    // 添加调试用的重置方法
    public void DebugResetGame()
    {
        playerData = new PlayerData();
        playerData.CalculateStats();
        DebugLogger.Log("调试", "游戏数据已重置");
    }
}