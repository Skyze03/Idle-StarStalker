using UnityEngine;

public class SimpleDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("SimpleDebug脚本已启动！");
    }
    
    public void TestButton()
    {
        Debug.Log("按钮被点击了！");
    }
}