using System;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSystem : MonoBehaviour
{
    private PlayerData playerData;
    private BattleState battleState;

    public IReadOnlyList<UltimateData> Catalog => UltimateData.GetAll();

    public void Setup(PlayerData data, BattleState state)
    {
        playerData = data;
        battleState = state;
        NormalizePlayerUltimates();
    }

    public bool IsUnlocked(string ultimateId)
    {
        if (playerData?.unlockedUltimateIds == null) return false;
        return Array.IndexOf(playerData.unlockedUltimateIds, ultimateId) >= 0;
    }

    public bool Unlock(string ultimateId)
    {
        if (playerData == null || IsUnlocked(ultimateId)) return false;
        List<string> unlocked = new List<string>(playerData.unlockedUltimateIds);
        unlocked.Add(ultimateId);
        playerData.unlockedUltimateIds = unlocked.ToArray();
        Debug.Log($"Ultimate unlocked: {UltimateData.GetById(ultimateId).ultimateName}");
        return true;
    }

    public bool Equip(string ultimateId)
    {
        if (playerData == null || !IsUnlocked(ultimateId) ||
            (battleState != null && battleState.battleRunning)) return false;
        playerData.equippedUltimateId = ultimateId;
        playerData.RefreshUltimate();
        Debug.Log($"Ultimate equipped: {playerData.equippedUltimate.ultimateName}");
        return true;
    }

    public void NormalizePlayerUltimates()
    {
        if (playerData == null) return;
        if (playerData.unlockedUltimateIds == null || playerData.unlockedUltimateIds.Length == 0)
            playerData.unlockedUltimateIds = new[] { "star_burst" };
        if (!IsUnlocked(playerData.equippedUltimateId))
            playerData.equippedUltimateId = playerData.unlockedUltimateIds[0];
        playerData.RefreshUltimate();
    }
}
