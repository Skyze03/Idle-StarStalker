using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
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

        int cost = playerData.GetPartUpgradeCost(partType);

        if (playerData.energy < cost)
        {
            Debug.Log("Not enough energy to upgrade " + partType);
            return false;
        }

        playerData.energy -= cost;
        playerData.UpgradePart(partType);

        Debug.Log(partType + " upgraded. New Level = " + playerData.GetPartLevel(partType));
        return true;
    }
}