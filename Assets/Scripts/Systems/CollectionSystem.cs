using System;
using UnityEngine;

public class CollectionSystem : MonoBehaviour
{
    public event Action<string> FeedbackRequested;
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
        bool runeDropped = TryDropRune();

        FeedbackRequested?.Invoke(
            $"Collected: +{totalEnergyGain} Energy" +
            (runeDropped ? "  +1 Rune" : string.Empty)
        );

        Debug.Log("Collected once. Current Energy = " + playerData.energy);
    }

    private bool TryDropRune()
    {
        if (inventorySystem == null)
        {
            return false;
        }

        float roll = UnityEngine.Random.value;

        if (roll <= runeDropChance)
        {
            inventorySystem.AddRune(1);
            return true;
        }

        return false;
    }
}
