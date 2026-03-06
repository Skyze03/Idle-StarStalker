using UnityEngine;
// 在 Item.cs 中完善以下类
[System.Serializable]
public class Item
{
    public string id;           // 唯一ID，如 "rune_red_a"
    public string name;         // 显示名称，如 "红色A符文"
    public string description;  // 描述
    public ItemType itemType;   // 物品类型（新增枚举）
    public Sprite icon;         // 图标（后期使用）
    public int stackCount = 1;  // 堆叠数量
}

// 物品类型枚举（新增）
public enum ItemType
{
    Rune,
    MeditationFragment,
    MemoryFragment
}

// 符文类型枚举（保持原有）
public enum RuneColor { Red, Yellow, Blue }
public enum RuneType { A, B, C }

[System.Serializable]
public class Rune : Item
{
    public RuneColor color;
    public RuneType type;
    
    public Rune()
    {
        itemType = ItemType.Rune;
    }
    
    public string GetCode()
    {
        return $"{color.ToString().Substring(0,1)}{type}"; // 如 "RA", "YB"
    }
}

// 碎片类型（保持原有）
public enum FragmentType { Meditation, Memory }

[System.Serializable]
public class Fragment : Item
{
    public FragmentType fragmentType;
    
    public Fragment()
    {
        itemType = fragmentType == FragmentType.Meditation ? 
            ItemType.MeditationFragment : ItemType.MemoryFragment;
    }
}