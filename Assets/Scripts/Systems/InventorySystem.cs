using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private InventoryData inventoryData;

    public void Setup(InventoryData data)
    {
        inventoryData = data;
    }

    public void AddMemoryFragment(int amount)
    {
        if (inventoryData == null)
        {
            Debug.LogWarning("InventorySystem: inventoryData is null.");
            return;
        }

        inventoryData.memoryFragmentCount += amount;
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
        Debug.Log("Added Rune. Total = " + inventoryData.runeCount);
    }
}