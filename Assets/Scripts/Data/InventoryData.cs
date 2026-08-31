using System;

[Serializable]
public class InventoryData
{
    public int memoryFragmentCount = 0;
    public int runeCount = 0;
    public int starDustCount = 0;
    public InventoryItemStack[] itemStacks = Array.Empty<InventoryItemStack>();
    public EquipmentInstance[] equipmentInstances = Array.Empty<EquipmentInstance>();
    public EquipmentLoadoutEntry[] equippedInstances = Array.Empty<EquipmentLoadoutEntry>();
}
