using UnityEngine;

public class CollectionSystem : MonoBehaviour
{
    private PlayerData playerData;
    private InventorySystem inventorySystem;
    private BuffData buffData;

    [SerializeField] private int energyPerCollection = 5;
    [SerializeField] private float runeDropChance = 0.4f;

    public void Setup(PlayerData data, InventorySystem inventory, BuffData buff)
    {
        playerData = data;
        inventorySystem = inventory;
        buffData = buff;
    }

    public void CollectOnce()
    {
        if (playerData == null)
        {
            Debug.LogWarning("CollectionSystem: playerData is null.");
            return;
        }

        int totalEnergyGain = energyPerCollection;

        if (playerData != null && playerData.stats != null)
        {
            totalEnergyGain += playerData.stats.collectionEnergyBonus;
        }

        if (buffData != null)
        {
            totalEnergyGain += buffData.collectionEnergyBonus;
        }

        playerData.energy += totalEnergyGain;
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