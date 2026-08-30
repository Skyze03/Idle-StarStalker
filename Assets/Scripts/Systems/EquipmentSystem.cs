using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    private PlayerData playerData;
    private BattleState battleState;
    public int OwnedCount => playerData?.ownedEquipmentIds?.Length ?? 0;

    public void Setup(PlayerData data, BattleState state)
    {
        playerData = data;
        battleState = state;
        Normalize(0);
    }

    public EquipmentData GetEquipped(EquipmentSlot slot)
    {
        return EquipmentData.GetById(GetEquippedId(slot));
    }

    public List<EquipmentData> GetOwnedForSlot(EquipmentSlot slot)
    {
        List<EquipmentData> result = new List<EquipmentData>();
        foreach (EquipmentData item in EquipmentData.GetAll())
            if (item.slot == slot && IsOwned(item.id)) result.Add(item);
        return result;
    }

    public bool Equip(EquipmentSlot slot, string itemId)
    {
        if (playerData == null || (battleState != null && battleState.battleRunning))
            return false;

        if (!string.IsNullOrEmpty(itemId))
        {
            EquipmentData item = EquipmentData.GetById(itemId);
            if (item == null || item.slot != slot || !IsOwned(itemId)) return false;
        }

        SetEquippedId(slot, itemId ?? string.Empty);
        return true;
    }

    public bool Cycle(EquipmentSlot slot)
    {
        if (battleState != null && battleState.battleRunning) return false;
        List<EquipmentData> choices = GetOwnedForSlot(slot);
        string current = GetEquippedId(slot);
        int index = choices.FindIndex(item => item.id == current);
        int nextIndex = index + 1;
        return Equip(slot, nextIndex >= choices.Count ? string.Empty : choices[nextIndex].id);
    }

    public IEnumerable<EquipmentData> GetAllEquipped()
    {
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            EquipmentData item = GetEquipped(slot);
            if (item != null) yield return item;
        }
    }

    public void Normalize(int highestClearedStage)
    {
        if (playerData == null) return;
        if (playerData.ownedEquipmentIds == null)
            playerData.ownedEquipmentIds = Array.Empty<string>();

        for (int stage = 1; stage <= highestClearedStage; stage++)
        {
            string itemId = GetStageEquipmentId(stage);
            if (!string.IsNullOrEmpty(itemId)) Unlock(itemId);
        }

        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            string id = GetEquippedId(slot);
            EquipmentData item = EquipmentData.GetById(id);
            if (item == null || item.slot != slot || !IsOwned(id))
                SetEquippedId(slot, string.Empty);
        }
    }

    public bool IsOwned(string id) =>
        playerData?.ownedEquipmentIds != null &&
        Array.IndexOf(playerData.ownedEquipmentIds, id) >= 0;

    public bool Unlock(string itemId)
    {
        EquipmentData item = EquipmentData.GetById(itemId);
        if (playerData == null || item == null || IsOwned(itemId)) return false;
        List<string> owned = new List<string>(playerData.ownedEquipmentIds);
        owned.Add(itemId);
        playerData.ownedEquipmentIds = owned.ToArray();
        Debug.Log($"Equipment obtained: {item.itemName}");
        return true;
    }

    public static string GetStageEquipmentId(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1: return "star_blade";
            case 2: return "seer_circlet";
            case 3: return "iron_carapace";
            case 4: return "windstep_boots";
            case 5: return "fury_bracers";
            case 6: return "guardian_leggings";
            case 7: return "thorn_sigil";
            case 8: return "war_visor";
            case 9: return "vital_shell";
            case 10: return "quickfang";
            default: return string.Empty;
        }
    }

    private string GetEquippedId(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Head: return playerData.equippedHeadItemId;
            case EquipmentSlot.Chest: return playerData.equippedChestItemId;
            case EquipmentSlot.Arms: return playerData.equippedArmsItemId;
            case EquipmentSlot.Legs: return playerData.equippedLegsItemId;
            case EquipmentSlot.Feet: return playerData.equippedFeetItemId;
            case EquipmentSlot.Weapon: return playerData.equippedWeaponItemId;
            case EquipmentSlot.Accessory: return playerData.equippedAccessoryItemId;
            default: return string.Empty;
        }
    }

    private void SetEquippedId(EquipmentSlot slot, string id)
    {
        switch (slot)
        {
            case EquipmentSlot.Head: playerData.equippedHeadItemId = id; break;
            case EquipmentSlot.Chest: playerData.equippedChestItemId = id; break;
            case EquipmentSlot.Arms: playerData.equippedArmsItemId = id; break;
            case EquipmentSlot.Legs: playerData.equippedLegsItemId = id; break;
            case EquipmentSlot.Feet: playerData.equippedFeetItemId = id; break;
            case EquipmentSlot.Weapon: playerData.equippedWeaponItemId = id; break;
            case EquipmentSlot.Accessory: playerData.equippedAccessoryItemId = id; break;
        }
    }
}
