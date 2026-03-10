using UnityEngine;

public class CollectionSystem : MonoBehaviour
{
    private PlayerData playerData;
    private InventorySystem inventorySystem;

    [SerializeField] private int energyPerCollection = 5;
    [SerializeField] private float runeDropChance = 0.4f;

    public void Setup(PlayerData data, InventorySystem inventory)
    {
        playerData = data;
        inventorySystem = inventory;
    }

    public void CollectOnce()
    {
        if (playerData == null)
        {
            Debug.LogWarning("CollectionSystem: playerData is null.");
            return;
        }

        playerData.energy += energyPerCollection;
        TryDropRune();

        Debug.Log("Collected once. Current Energy = " + playerData.energy);
    }

    private void TryDropRune()
    {
        if (inventorySystem == null)
        {
            return;
        }

        float roll = Random.value;

        if (roll <= runeDropChance)
        {
            inventorySystem.AddRune(1);
        }
    }
}