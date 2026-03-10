using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ForceAlphaHitTest : MonoBehaviour
{
    [Range(0.01f, 0.9f)]
    public float alphaThreshold = 0.1f;
    private Image targetImage;
    
    void Start()
    {
        targetImage = GetComponent<Image>();
        ApplyAlphaHitTest();
    }
    
    void ApplyAlphaHitTest()
    {
        if (targetImage == null) return;
        
        // 方法1：直接设置
        targetImage.alphaHitTestMinimumThreshold = alphaThreshold;
        
        Debug.Log($"【Alpha检测】已为 {gameObject.name} 设置阈值: {alphaThreshold}");
        
        // 验证设置
        StartCoroutine(VerifyAfterDelay());
    }
    
    System.Collections.IEnumerator VerifyAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log($"【Alpha检测验证】{gameObject.name} 当前阈值: {targetImage.alphaHitTestMinimumThreshold}");
    }
    
    // 编辑器调试
    #if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && targetImage == null)
            targetImage = GetComponent<Image>();
        
        if (targetImage != null)
            targetImage.alphaHitTestMinimumThreshold = alphaThreshold;
    }
    #endif
}