using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;

public class CharacterClickHandler : MonoBehaviour, 
    IPointerClickHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler
{
    [Header("必填组件")]
    public MeditationSystem meditationSystem;  // 冥想系统
    
    [Header("点击设置")]
    public float clickCooldown = 1f;          // 点击冷却时间
    public float alphaThreshold = 0.1f;       // Alpha检测阈值
    
    [Header("视觉反馈")]
    public Color hoverColor = new Color(1.1f, 1.1f, 1.1f, 1f);   // 悬停颜色
    public Color clickColor = new Color(1.2f, 1.2f, 1.2f, 1f);    // 点击颜色
    public float feedbackDuration = 0.2f;     // 反馈持续时间
    
    // 内部变量
    private Image characterImage;
    private Color originalColor;
    private bool canClick = true;
    private Coroutine feedbackCoroutine;
    
    void Start()
    {
        characterImage = GetComponent<Image>();
        originalColor = characterImage.color;
        
        // ========== 关键修改：改进的自动查找 ==========
        if (meditationSystem == null)
        {
            // 方法1：先按名称查找GameSystems
            GameObject gameSystems = GameObject.Find("GameSystems");
            if (gameSystems != null)
            {
                meditationSystem = gameSystems.GetComponentInChildren<MeditationSystem>();
                Debug.Log($"通过GameSystems找到了MeditationSystem: {meditationSystem != null}");
            }
            
            // 方法2：如果没找到，全局查找
            if (meditationSystem == null)
            {
                meditationSystem = FindObjectOfType<MeditationSystem>(true); // true表示包括未激活的
                Debug.Log($"全局查找MeditationSystem: {meditationSystem != null}");
            }
            
            // 方法3：如果还是没找到，尝试通过GameManager获取
            if (meditationSystem == null && GameManager.Instance != null)
            {
                // 如果GameManager有引用
                meditationSystem = GameManager.Instance.GetComponentInChildren<MeditationSystem>();
                Debug.Log($"通过GameManager查找: {meditationSystem != null}");
            }
        }
        
        if (meditationSystem == null)
        {
            Debug.LogError("❌ 未找到MeditationSystem！请手动在Inspector中拖拽赋值。");
            Debug.Log("检查位置：GameSystems对象应该包含MeditationSystem脚本");
        }
        else
        {
            Debug.Log($"✅ MeditationSystem连接成功: {meditationSystem.gameObject.name}");
        }
        
        // 确保Alpha检测启用
        EnsureAlphaHitTest();
    }

    void FindMeditationSystem()
    {
        // 如果已经在Inspector中设置了，直接使用
        if (meditationSystem != null)
        {
            Debug.Log($"✅ 使用Inspector中设置的MeditationSystem: {meditationSystem.gameObject.name}");
            return;
        }
        
        Debug.Log("开始自动查找MeditationSystem...");
        
        // 方法列表（按优先级）
        System.Func<MeditationSystem>[] findMethods = new System.Func<MeditationSystem>[]
        {
            // 方法1：通过GameManager（如果你有）
            () => GameManager.Instance?.GetComponentInChildren<MeditationSystem>(),
            
            // 方法2：查找名为"GameSystems"的对象
            () => {
                GameObject go = GameObject.Find("GameSystems");
                return go?.GetComponentInChildren<MeditationSystem>();
            },
            
            // 方法3：查找包含"System"的对象
            () => {
                MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();
                foreach (MonoBehaviour obj in allObjects)
                {
                    if (obj.name.Contains("System") && obj is MeditationSystem ms)
                        return ms;
                }
                return null;
            },
            
            // 方法4：全局查找
            () => FindObjectOfType<MeditationSystem>(true),
            
            // 方法5：通过类型名查找
            () => {
                GameObject[] allGOs = FindObjectsOfType<GameObject>();
                foreach (GameObject go in allGOs)
                {
                    MeditationSystem ms = go.GetComponent<MeditationSystem>();
                    if (ms != null) return ms;
                }
                return null;
            }
        };
        
        // 尝试所有方法
        foreach (var method in findMethods)
        {
            meditationSystem = method();
            if (meditationSystem != null)
            {
                Debug.Log($"✅ 找到MeditationSystem: {meditationSystem.gameObject.name} (方法: {method.Method.Name})");
                return;
            }
        }
        
        // 如果都没找到
        Debug.LogError("❌ 未找到MeditationSystem！原因可能是：");
        Debug.LogError("1. MeditationSystem脚本未添加到场景中的任何对象");
        Debug.LogError("2. MeditationSystem对象被禁用");
        Debug.LogError("3. 脚本名称不匹配（检查是否叫MeditationSystem）");
        
        // 创建临时占位（仅用于测试）
        CreateTestMeditationSystem();
    }

    void CreateTestMeditationSystem()
    {
        Debug.LogWarning("创建测试用的MeditationSystem...");
        
        GameObject testObj = new GameObject("Test_MeditationSystem");
        meditationSystem = testObj.AddComponent<MeditationSystem>();
        
        // 尝试复制现有设置
        MeditationSystem existing = FindObjectOfType<MeditationSystem>(true);
        if (existing != null)
        {
            // 复制一些设置
            Debug.Log("复制了现有的MeditationSystem设置");
        }
        
        Debug.LogWarning("注意：这是测试用的临时MeditationSystem，请尽快设置正确的引用");
    }

    void EnsureAlphaHitTest()
    {
        if (characterImage == null) return;
        
        characterImage.raycastTarget = true;
        
        // 设置Alpha阈值
        try
        {
            characterImage.alphaHitTestMinimumThreshold = alphaThreshold;
            Debug.Log($"✅ Alpha检测设置成功: 阈值={alphaThreshold}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Alpha检测设置失败: {e.Message}");
        }
    }
    
    
    // ========== 点击事件 ==========
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canClick) 
        {
            Debug.Log("点击冷却中...");
            return;
        }
        
        if (meditationSystem == null)
        {
            Debug.LogWarning("点击了角色，但MeditationSystem未连接");
            return;
        }
        
        Debug.Log("🎯 角色被点击！开始冥想...");
        
        // 开始冥想
        StartCoroutine(ClickRoutine());
    }
    
    IEnumerator ClickRoutine()
    {
        // 进入冷却
        canClick = false;
        
        // 视觉反馈
        StartClickFeedback();
        
        // 等待反馈动画
        yield return new WaitForSeconds(feedbackDuration);
        
        // 触发冥想
        if (meditationSystem != null)
        {
            meditationSystem.StartMeditation();
            Debug.Log("✅ 冥想已开始");
        }
        
        // 冷却时间
        yield return new WaitForSeconds(clickCooldown);
        canClick = true;
        
        Debug.Log("🔄 可以再次点击角色");
    }
    
    // ========== 视觉反馈 ==========
    void StartClickFeedback()
    {
        if (characterImage == null) return;
        
        // 停止之前的反馈
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);
        
        feedbackCoroutine = StartCoroutine(ClickFeedbackCoroutine());
    }
    
    IEnumerator ClickFeedbackCoroutine()
    {
        if (characterImage == null) yield break;
        
        float elapsed = 0f;
        
        // 点击瞬间效果
        characterImage.color = clickColor;
        
        // 渐变恢复
        while (elapsed < feedbackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / feedbackDuration;
            
            characterImage.color = Color.Lerp(
                clickColor, 
                originalColor, 
                t
            );
            
            yield return null;
        }
        
        // 确保恢复
        characterImage.color = originalColor;
    }
    
    // ========== 悬停效果 ==========
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (characterImage != null && canClick)
        {
            characterImage.color = hoverColor;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (characterImage != null)
        {
            characterImage.color = originalColor;
        }
    }
    
    // ========== 调试方法 ==========
    void Update()
{
    // 使用新Input System（Unity Input System Package）
    Keyboard keyboard = Keyboard.current;
    
    // 检查键盘是否可用
    if (keyboard == null)
    {
        // 可能是移动设备或编辑器未聚焦
        // 可以在这里添加触摸屏检测
        #if UNITY_EDITOR
        // 编辑器中使用鼠标左键模拟
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && canClick)
        {
            Debug.Log("鼠标左键模拟点击角色");
            StartCoroutine(ClickRoutine());
        }
        #endif
        return;
    }
    
    // 空格键模拟点击（调试功能）
    if (keyboard.spaceKey.wasPressedThisFrame && canClick)
    {
        Debug.Log("空格键模拟点击角色");
        StartCoroutine(ClickRoutine());
    }
    
    // 可选：添加其他调试快捷键
    if (keyboard.f1Key.wasPressedThisFrame)
    {
        Debug.Log("F1键：显示调试信息");
        // 可以在这里添加调试功能
    }
}
    
    void OnDrawGizmosSelected()
    {
        if (characterImage == null) return;
        
        // 在Scene视图中显示点击区域
        RectTransform rt = characterImage.rectTransform;
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        
        // 绘制绿色边框
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
        }
        
        // 显示信息
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(
            corners[1], 
            $"角色点击区域\n" +
            $"Alpha阈值: {alphaThreshold}\n" +
            $"冷却: {(canClick ? "就绪" : "等待")}"
        );
        #endif
    }
}