using System;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public event Action<string> FeedbackRequested;
    private PlayerData playerData;

    public void Setup(PlayerData data)
    {
        playerData = data;
    }

    public bool TryUpgradePart(BodyPartType partType)
    {
        if (playerData == null)
        {
            Debug.LogWarning("UpgradeSystem: playerData is null.");
            return false;
        }

        if (!playerData.CanUpgradePart(partType))
        {
            Debug.Log(
                partType + " cannot exceed Player Level " + playerData.level
            );
            FeedbackRequested?.Invoke(
                $"{partType}: Player Lv.{playerData.GetPartLevel(partType) + 1} required"
            );
            return false;
        }

        int cost = playerData.GetPartUpgradeCost(partType);

        if (playerData.energy < cost)
        {
            Debug.Log("Not enough energy to upgrade " + partType);
            FeedbackRequested?.Invoke($"{partType}: not enough Energy ({cost} required)");
            return false;
        }

        playerData.energy -= cost;
        playerData.UpgradePart(partType);

        Debug.Log(partType + " upgraded. New Level = " + playerData.GetPartLevel(partType));
        FeedbackRequested?.Invoke(
            $"{partType} upgraded to Lv.{playerData.GetPartLevel(partType)}  -{cost} Energy"
        );
        return true;
    }
}
