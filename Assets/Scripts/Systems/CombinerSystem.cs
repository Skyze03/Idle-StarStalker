using System;
using UnityEngine;

public class CombinerSystem : MonoBehaviour
{
    public event Action<string> FeedbackRequested;
    private InventoryData inventoryData;
    private BuffData buffData;

    [SerializeField] private int meditationBuffFragmentCost = 1;
    [SerializeField] private int collectionBuffRuneCost = 1;

    [SerializeField] private int meditationBuffValue = 5;
    [SerializeField] private int collectionBuffValue = 3;

    public void Setup(InventoryData inventory, BuffData buff)
    {
        inventoryData = inventory;
        buffData = buff;
    }

    public bool CreateMeditationBuff()
    {
        if (inventoryData == null || buffData == null)
        {
            Debug.LogWarning("CombinerSystem: inventoryData or buffData is null.");
            return false;
        }

        if (inventoryData.memoryFragmentCount < meditationBuffFragmentCost)
        {
            Debug.Log("Not enough Memory Fragments.");
            FeedbackRequested?.Invoke("Not enough Memory Fragments");
            return false;
        }

        inventoryData.memoryFragmentCount -= meditationBuffFragmentCost;
        buffData.meditationExpBonus = meditationBuffValue;

        Debug.Log("Meditation Buff created. EXP bonus = " + buffData.meditationExpBonus);
        FeedbackRequested?.Invoke(
            $"Meditation Buff created  EXP +{buffData.meditationExpBonus}"
        );
        return true;
    }

    public bool CreateCollectionBuff()
    {
        if (inventoryData == null || buffData == null)
        {
            Debug.LogWarning("CombinerSystem: inventoryData or buffData is null.");
            return false;
        }

        if (inventoryData.runeCount < collectionBuffRuneCost)
        {
            Debug.Log("Not enough Runes.");
            FeedbackRequested?.Invoke("Not enough Runes");
            return false;
        }

        inventoryData.runeCount -= collectionBuffRuneCost;
        buffData.collectionEnergyBonus = collectionBuffValue;

        Debug.Log("Collection Buff created. Energy bonus = " + buffData.collectionEnergyBonus);
        FeedbackRequested?.Invoke(
            $"Collection Buff created  Energy +{buffData.collectionEnergyBonus}"
        );
        return true;
    }
}
