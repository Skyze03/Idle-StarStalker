using UnityEngine;

public class RewardPipeline : MonoBehaviour
{
    private PlayerData player;
    private InventorySystem inventory;
    private EquipmentSystem equipment;
    private UltimateSystem ultimates;

    public void Setup(PlayerData data, InventorySystem inventorySystem,
        EquipmentSystem equipmentSystem, UltimateSystem ultimateSystem)
    {
        player = data; inventory = inventorySystem;
        equipment = equipmentSystem; ultimates = ultimateSystem;
    }

    public void Grant(RewardBundle reward, string source)
    {
        if (reward == null || player == null) return;
        player.energy += reward.energy;
        if (reward.memoryFragments > 0) inventory?.AddMemoryFragment(reward.memoryFragments);
        if (reward.runes > 0) inventory?.AddRune(reward.runes);
        if (!string.IsNullOrEmpty(reward.equipmentTemplateId))
            equipment?.GrantInstance(reward.equipmentTemplateId);
        foreach (string templateId in reward.equipmentTemplateIds ?? System.Array.Empty<string>())
            equipment?.GrantInstance(templateId);
        if (!string.IsNullOrEmpty(reward.ultimateId)) ultimates?.Unlock(reward.ultimateId);
        Debug.Log($"RewardPipeline [{source}]: {reward.energy} Energy");
    }
}
