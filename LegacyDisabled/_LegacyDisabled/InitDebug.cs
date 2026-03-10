using UnityEngine;

public class InitDebug : MonoBehaviour
{
    void Start()
    {
        // 确保GameManager存在
        if (GameManager.Instance == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }
        
        // 输出欢迎信息
        DebugLogger.Log("系统", "====================================");
        DebugLogger.Log("系统", "     星语者 - 调试模式启动");
        DebugLogger.Log("系统", "====================================");
        DebugLogger.Log("系统", "快捷键：");
        DebugLogger.Log("系统", "  F1 - 显示状态");
        DebugLogger.Log("系统", "  F2 - 快速升级");
        DebugLogger.Log("系统", "  F3 - 清空控制台");
        DebugLogger.Log("系统", "  F4 - 测试冥想");
        DebugLogger.Log("系统", "  F5 - 测试采集");
        DebugLogger.Log("系统", "====================================");
    }
}