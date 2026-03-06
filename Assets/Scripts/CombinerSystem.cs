using UnityEngine;

public class CombinerSystem : MonoBehaviour
{
    // 简单占位类，先让编译通过
    public void SelectRune(Rune rune)
    {
        Debug.Log($"组合器收到符文: {rune?.GetCode()}");
    }
}