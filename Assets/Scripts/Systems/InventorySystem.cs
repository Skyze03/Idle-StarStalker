using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private InventoryData inventoryData;

    public void Setup(InventoryData data)
    {
        inventoryData = data;
        Normalize();
    }

    public void AddMemoryFragment(int amount)
    {
        if (inventoryData == null)
        {
            Debug.LogWarning("InventorySystem: inventoryData is null.");
            return;
        }

        inventoryData.memoryFragmentCount += amount;
        SetStack("memory_fragment", inventoryData.memoryFragmentCount);
        Debug.Log("Added Memory Fragment. Total = " + inventoryData.memoryFragmentCount);
    }

    public void AddRune(int amount)
    {
        if (inventoryData == null)
        {
            Debug.LogWarning("InventorySystem: inventoryData is null.");
            return;
        }

        inventoryData.runeCount += amount;
        SetStack("rune", inventoryData.runeCount);
        Debug.Log("Added Rune. Total = " + inventoryData.runeCount);
    }

    public int StarDust => inventoryData?.starDustCount ?? 0;

    public void AddStarDust(int amount)
    {
        if (inventoryData == null || amount <= 0) return;
        inventoryData.starDustCount += amount;
        SetStack("star_dust", inventoryData.starDustCount);
    }

    public bool SpendStarDust(int amount)
    {
        if (inventoryData == null || amount < 0 || inventoryData.starDustCount < amount) return false;
        inventoryData.starDustCount -= amount;
        SetStack("star_dust", inventoryData.starDustCount);
        return true;
    }

    public bool RemoveEquipment(string instanceId)
    {
        if (inventoryData == null) return false;
        List<EquipmentInstance> items = new List<EquipmentInstance>(inventoryData.equipmentInstances);
        int removed = items.RemoveAll(x => x.instanceId == instanceId);
        if (removed == 0) return false;
        inventoryData.equipmentInstances = items.ToArray();
        return true;
    }

    public int GetAmount(string itemId)
    {
        if (inventoryData?.itemStacks == null) return 0;
        InventoryItemStack stack = Array.Find(inventoryData.itemStacks, x => x.itemId == itemId);
        return stack?.amount ?? 0;
    }

    public EquipmentInstance AddEquipment(string templateId)
    {
        if (EquipmentData.GetById(templateId) == null || inventoryData == null) return null;
        List<EquipmentInstance> items = new List<EquipmentInstance>(inventoryData.equipmentInstances);
        EquipmentInstance instance = new EquipmentInstance(templateId);
        items.Add(instance);
        inventoryData.equipmentInstances = items.ToArray();
        return instance;
    }

    public IReadOnlyList<EquipmentInstance> EquipmentInstances =>
        inventoryData?.equipmentInstances ?? Array.Empty<EquipmentInstance>();
    public InventoryData Data => inventoryData;

    private void Normalize()
    {
        if (inventoryData.itemStacks == null) inventoryData.itemStacks = Array.Empty<InventoryItemStack>();
        if (inventoryData.equipmentInstances == null) inventoryData.equipmentInstances = Array.Empty<EquipmentInstance>();
        if (inventoryData.equippedInstances == null) inventoryData.equippedInstances = Array.Empty<EquipmentLoadoutEntry>();
        SetStack("memory_fragment", inventoryData.memoryFragmentCount);
        SetStack("rune", inventoryData.runeCount);
        SetStack("star_dust", inventoryData.starDustCount);
    }

    private void SetStack(string id, int amount)
    {
        List<InventoryItemStack> stacks = new List<InventoryItemStack>(inventoryData.itemStacks);
        InventoryItemStack stack = stacks.Find(x => x.itemId == id);
        if (stack == null) stacks.Add(new InventoryItemStack(id, amount));
        else stack.amount = amount;
        inventoryData.itemStacks = stacks.ToArray();
    }
}
