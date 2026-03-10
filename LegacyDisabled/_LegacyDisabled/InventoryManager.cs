using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    // 单例模式
    public static InventoryManager Instance { get; private set; }
    
    // 物品列表（按类型分开便于管理）
    public List<Rune> runes = new List<Rune>();
    public List<Fragment> meditationFragments = new List<Fragment>();
    public List<Fragment> memoryFragments = new List<Fragment>();
    
    // 背包容量限制（可选）
    public int maxRunes = 50;
    public int maxFragments = 100;
    
    // 事件：当背包内容变化时通知UI更新
    public System.Action OnInventoryChanged;
    
    void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景保存
            Debug.Log("背包管理器初始化");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // ========== 添加物品的方法 ==========
    public bool AddRune(Rune rune)
    {
        // 检查容量
        if (runes.Count >= maxRunes)
        {
            DebugLogger.Log("背包", "符文背包已满！");
            return false;
        }
        
        // 设置ID和名称
        rune.id = $"rune_{rune.color}_{rune.type}".ToLower();
        rune.name = $"{rune.color}{rune.type}符文";
        
        runes.Add(rune);
        DebugLogger.Log("背包", $"获得符文: {rune.GetCode()}");
        
        // 触发更新事件
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    public bool AddFragment(Fragment fragment)
    {
        // 根据类型添加到不同列表
        bool success = false;
        
        if (fragment.fragmentType == FragmentType.Meditation)
        {
            if (meditationFragments.Count < maxFragments)
            {
                fragment.id = $"frag_med_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
                fragment.name = "冥想碎片";
                meditationFragments.Add(fragment);
                success = true;
                DebugLogger.Log("背包", "获得冥想碎片");
            }
        }
        else
        {
            if (memoryFragments.Count < maxFragments)
            {
                fragment.id = $"frag_mem_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
                fragment.name = "记忆碎片";
                memoryFragments.Add(fragment);
                success = true;
                DebugLogger.Log("背包", "获得记忆碎片");
            }
        }
        
        if (success) OnInventoryChanged?.Invoke();
        return success;
    }
    
    // ========== 查询物品的方法 ==========
    public List<Rune> GetRunesByColor(RuneColor color)
    {
        return runes.Where(r => r.color == color).ToList();
    }
    
    public List<Rune> GetRunesByType(RuneType type)
    {
        return runes.Where(r => r.type == type).ToList();
    }
    
    public Rune FindRune(string code)
    {
        return runes.FirstOrDefault(r => r.GetCode() == code);
    }
    
    // ========== 移除物品的方法 ==========
    public bool RemoveRune(Rune rune)
    {
        bool removed = runes.Remove(rune);
        if (removed) OnInventoryChanged?.Invoke();
        return removed;
    }
    
    public bool RemoveRuneByCode(string code)
    {
        var rune = FindRune(code);
        if (rune != null) return RemoveRune(rune);
        return false;
    }
    
    // ========== 统计信息 ==========
    public int GetTotalRuneCount()
    {
        return runes.Count;
    }
    
    public Dictionary<string, int> GetRuneCountByType()
    {
        var counts = new Dictionary<string, int>();
        
        foreach (var rune in runes)
        {
            string code = rune.GetCode();
            if (counts.ContainsKey(code))
                counts[code]++;
            else
                counts[code] = 1;
        }
        
        return counts;
    }
    
    // ========== 清空背包（调试用） ==========
    public void ClearInventory()
    {
        runes.Clear();
        meditationFragments.Clear();
        memoryFragments.Clear();
        OnInventoryChanged?.Invoke();
        DebugLogger.Log("背包", "背包已清空");
    }
    
    // ========== 测试数据（开发时使用） ==========
    public void AddTestItems()
    {
        DebugLogger.Log("测试", "添加测试物品...");
        
        // 添加一些符文
        string[] colors = { "Red", "Yellow", "Blue" };
        string[] types = { "A", "B", "C" };
        
        for (int i = 0; i < 10; i++)
        {
            Rune rune = new Rune();
            rune.color = (RuneColor)Random.Range(0, 3);
            rune.type = (RuneType)Random.Range(0, 3);
            AddRune(rune);
        }
        
        // 添加一些碎片
        for (int i = 0; i < 3; i++)
        {
            Fragment frag = new Fragment();
            frag.fragmentType = FragmentType.Meditation;
            AddFragment(frag);
        }
        
        for (int i = 0; i < 2; i++)
        {
            Fragment frag = new Fragment();
            frag.fragmentType = FragmentType.Memory;
            AddFragment(frag);
        }
        
        DebugLogger.Log("测试", $"添加了10个符文，5个碎片");
    }
}