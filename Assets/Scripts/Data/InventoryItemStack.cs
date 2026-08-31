using System;

[Serializable]
public class InventoryItemStack
{
    public string itemId;
    public int amount;
    public InventoryItemStack(string id, int value) { itemId = id; amount = value; }
}
