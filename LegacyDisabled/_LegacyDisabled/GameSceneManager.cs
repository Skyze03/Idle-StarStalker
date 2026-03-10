using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour  // 类名改为GameSceneManager
{
    public GameObject meditationPanel;
    public GameObject collectionPanel;
    
    void Start()
    {
        // 确保开始时两个面板都存在
        if (meditationPanel == null)
            meditationPanel = GameObject.Find("MeditationPanel");
    
        if (collectionPanel == null)
            collectionPanel = GameObject.Find("CollectionPanel");
    
        // 显示冥想场景，隐藏采集场景
        ShowMeditationScene();

    }
    
    public void ShowMeditationScene()
    {
        Debug.Log("切换到冥想场景");
        
        if (meditationPanel == null)
        {
            Debug.LogError("MeditationPanel未分配！");
            meditationPanel = GameObject.Find("MeditationPanel");
        }
        
        if (collectionPanel == null)
        {
            Debug.LogError("CollectionPanel未分配！");
            collectionPanel = GameObject.Find("CollectionPanel");
        }
        
        if (meditationPanel != null) 
        {
            meditationPanel.SetActive(true);
            Debug.Log("显示冥想面板");
        }
        
        if (collectionPanel != null) 
        {
            collectionPanel.SetActive(false);
            Debug.Log("隐藏采集面板");
        }
    }
    
    public void ShowCollectionScene()
    {
        meditationPanel.SetActive(false);
        collectionPanel.SetActive(true);
    }
}