using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    private PlayerData playerData;

    public void Setup(PlayerData data)
    {
        playerData = data;
    }

    public bool TryUpgradeBody()
    {
        if (playerData == null)
        {
            Debug.LogWarning("UpgradeSystem: playerData is null.");
            return false;
        }

        int cost = playerData.GetBodyUpgradeCost();

        if (playerData.energy < cost)
        {
            Debug.Log("Not enough energy to upgrade body.");
            return false;
        }

        playerData.energy -= cost;
        playerData.bodyLevel++;

        Debug.Log("Body upgraded. New Body Level = " + playerData.bodyLevel);
        return true;
    }
}