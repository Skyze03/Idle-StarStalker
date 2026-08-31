using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    public const int MaxEquipmentLevel = 10;
    public const int DismantleStarDustReward = 10;
    public event Action<string> FeedbackRequested;
    private PlayerData playerData;
    private BattleState battleState;
    private InventorySystem inventorySystem;
    public int OwnedCount => inventorySystem?.EquipmentInstances.Count ??
        playerData?.ownedEquipmentIds?.Length ?? 0;

    public void Setup(PlayerData data, BattleState state, InventorySystem inventory = null)
    {
        playerData = data;
        battleState = state;
        inventorySystem = inventory;
        Normalize(0);
    }

    public EquipmentData GetEquipped(EquipmentSlot slot)
    {
        return EquipmentData.GetById(GetEquippedId(slot));
    }

    public EquipmentInstance GetEquippedInstance(EquipmentSlot slot)
    {
        EquipmentLoadoutEntry entry = inventorySystem?.Data?.equippedInstances == null
            ? null : Array.Find(inventorySystem.Data.equippedInstances, x => x.slot == slot);
        EquipmentInstance instance = entry == null ? null :
            System.Linq.Enumerable.FirstOrDefault(inventorySystem.EquipmentInstances,
                x => x.instanceId == entry.instanceId);
        if (instance != null) return instance;
        string templateId = GetEquippedId(slot);
        return System.Linq.Enumerable.FirstOrDefault(
            inventorySystem?.EquipmentInstances ?? Array.Empty<EquipmentInstance>(),
            x => x.templateId == templateId);
    }

    public EquipmentStatBlock GetEffectiveStats(EquipmentInstance instance)
    {
        EquipmentData item = EquipmentData.GetById(instance?.templateId);
        if (item == null) return new EquipmentStatBlock();
        float multiplier = 1f + 0.1f * Mathf.Max(0, instance.level - 1);
        return new EquipmentStatBlock
        {
            hp = Mathf.RoundToInt(item.hp * multiplier), attack = Mathf.RoundToInt(item.attack * multiplier),
            defense = Mathf.RoundToInt(item.defense * multiplier), agility = Mathf.RoundToInt(item.agility * multiplier),
            wisdom = Mathf.RoundToInt(item.wisdom * multiplier),
            rageOnAttack = Mathf.Round(item.rageOnAttack * multiplier * 10f) / 10f,
            rageOnHit = Mathf.Round(item.rageOnHit * multiplier * 10f) / 10f
        };
    }

    public int GetUpgradeCost(EquipmentInstance instance) =>
        instance == null || instance.level >= MaxEquipmentLevel ? 0 : instance.level * 50;

    public int GetUpgradeStarDustCost(EquipmentInstance instance) =>
        instance == null || instance.level >= MaxEquipmentLevel ? 0 : instance.level * 5;

    public bool TryUpgrade(string instanceId)
    {
        if (battleState != null && battleState.battleRunning) return false;
        EquipmentInstance instance = System.Linq.Enumerable.FirstOrDefault(
            inventorySystem.EquipmentInstances, x => x.instanceId == instanceId);
        if (instance == null || instance.level >= MaxEquipmentLevel) return false;
        int energyCost = GetUpgradeCost(instance);
        int dustCost = GetUpgradeStarDustCost(instance);
        if (playerData.energy < energyCost)
        {
            FeedbackRequested?.Invoke($"Not enough Energy ({energyCost} required)");
            return false;
        }
        if (inventorySystem == null || inventorySystem.StarDust < dustCost)
        {
            FeedbackRequested?.Invoke($"Not enough Star Dust ({dustCost} required)");
            return false;
        }

        playerData.energy -= energyCost;
        inventorySystem.SpendStarDust(dustCost);
        instance.level++;
        FeedbackRequested?.Invoke(
            $"{EquipmentData.GetById(instance.templateId).itemName} upgraded to Lv.{instance.level}  " +
            $"-{energyCost} Energy  -{dustCost} Star Dust");
        return true;
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
        if (inventorySystem?.Data != null)
        {
            List<EquipmentLoadoutEntry> loadout = new List<EquipmentLoadoutEntry>(
                inventorySystem.Data.equippedInstances);
            loadout.RemoveAll(x => x.slot == slot);
            EquipmentInstance selected = string.IsNullOrEmpty(itemId) ? null :
                System.Linq.Enumerable.FirstOrDefault(inventorySystem.EquipmentInstances,
                    x => x.templateId == itemId);
            if (selected != null) loadout.Add(new EquipmentLoadoutEntry
                { slot = slot, instanceId = selected.instanceId });
            inventorySystem.Data.equippedInstances = loadout.ToArray();
        }
        EquipmentData equipped = EquipmentData.GetById(itemId);
        FeedbackRequested?.Invoke(equipped == null
            ? $"{slot} unequipped"
            : $"Equipped {equipped.itemName} to {slot}");
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

        if (inventorySystem != null)
        {
            foreach (string legacyId in playerData.ownedEquipmentIds)
            {
                bool migrated = System.Linq.Enumerable.Any(
                    inventorySystem.EquipmentInstances, x => x.templateId == legacyId);
                if (!migrated) inventorySystem.AddEquipment(legacyId);
            }
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
        (inventorySystem != null &&
            System.Linq.Enumerable.Any(inventorySystem.EquipmentInstances, x => x.templateId == id)) ||
        (playerData?.ownedEquipmentIds != null && Array.IndexOf(playerData.ownedEquipmentIds, id) >= 0);

    public bool Unlock(string itemId)
    {
        EquipmentData item = EquipmentData.GetById(itemId);
        if (playerData == null || item == null || IsOwned(itemId)) return false;
        List<string> owned = new List<string>(playerData.ownedEquipmentIds);
        owned.Add(itemId);
        playerData.ownedEquipmentIds = owned.ToArray();
        inventorySystem?.AddEquipment(itemId);
        Debug.Log($"Equipment obtained: {item.itemName}");
        return true;
    }

    public EquipmentInstance GrantInstance(string templateId)
    {
        EquipmentData item = EquipmentData.GetById(templateId);
        if (item == null || inventorySystem == null) return null;
        EquipmentInstance instance = inventorySystem.AddEquipment(templateId);
        if (!IsLegacyOwned(templateId))
        {
            List<string> owned = new List<string>(playerData.ownedEquipmentIds);
            owned.Add(templateId); playerData.ownedEquipmentIds = owned.ToArray();
        }
        return instance;
    }

    public List<EquipmentInstance> GetInstancesForSlot(EquipmentSlot slot)
    {
        List<EquipmentInstance> result = new List<EquipmentInstance>();
        foreach (EquipmentInstance instance in inventorySystem.EquipmentInstances)
        {
            EquipmentData item = EquipmentData.GetById(instance.templateId);
            if (item != null && item.slot == slot) result.Add(instance);
        }
        result.Sort((a, b) => {
            int locked = b.locked.CompareTo(a.locked);
            return locked != 0 ? locked : string.Compare(a.templateId, b.templateId, StringComparison.Ordinal);
        });
        return result;
    }

    public bool ToggleLock(string instanceId)
    {
        EquipmentInstance instance = System.Linq.Enumerable.FirstOrDefault(
            inventorySystem.EquipmentInstances, x => x.instanceId == instanceId);
        if (instance == null) return false;
        instance.locked = !instance.locked;
        FeedbackRequested?.Invoke(instance.locked ? "Equipment locked" : "Equipment unlocked");
        return true;
    }

    public bool IsEquippedInstance(string instanceId)
    {
        if (string.IsNullOrEmpty(instanceId) || inventorySystem?.Data?.equippedInstances == null)
            return false;
        if (Array.Exists(inventorySystem.Data.equippedInstances,
            entry => entry.instanceId == instanceId)) return true;

        // Old saves may contain only the equipped template id. Protect the
        // instance that currently represents that legacy equipped item too.
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            if (GetEquippedInstance(slot)?.instanceId == instanceId) return true;
        return false;
    }

    public bool TryDismantle(string instanceId)
    {
        if (battleState != null && battleState.battleRunning)
        {
            FeedbackRequested?.Invoke("Equipment cannot be dismantled during battle");
            return false;
        }

        EquipmentInstance instance = System.Linq.Enumerable.FirstOrDefault(
            inventorySystem?.EquipmentInstances ?? Array.Empty<EquipmentInstance>(),
            item => item.instanceId == instanceId);
        if (instance == null) return false;
        if (instance.locked)
        {
            FeedbackRequested?.Invoke("Unlock this equipment before dismantling");
            return false;
        }
        if (IsEquippedInstance(instanceId))
        {
            FeedbackRequested?.Invoke("Equipped equipment cannot be dismantled");
            return false;
        }

        string templateId = instance.templateId;
        string itemName = EquipmentData.GetById(templateId)?.itemName ?? "Equipment";
        if (!inventorySystem.RemoveEquipment(instanceId)) return false;
        inventorySystem.AddStarDust(DismantleStarDustReward);

        bool hasAnother = System.Linq.Enumerable.Any(
            inventorySystem.EquipmentInstances, item => item.templateId == templateId);
        if (!hasAnother && playerData?.ownedEquipmentIds != null)
        {
            List<string> owned = new List<string>(playerData.ownedEquipmentIds);
            owned.RemoveAll(id => id == templateId);
            playerData.ownedEquipmentIds = owned.ToArray();
        }

        FeedbackRequested?.Invoke(
            $"Dismantled {itemName}  +{DismantleStarDustReward} Star Dust");
        return true;
    }

    public bool EquipInstance(EquipmentSlot slot, string instanceId)
    {
        EquipmentInstance instance = string.IsNullOrEmpty(instanceId) ? null :
            System.Linq.Enumerable.FirstOrDefault(inventorySystem.EquipmentInstances,
                x => x.instanceId == instanceId);
        if (instance != null && EquipmentData.GetById(instance.templateId)?.slot != slot) return false;
        if (!Equip(slot, instance?.templateId ?? string.Empty)) return false;
        List<EquipmentLoadoutEntry> entries = new List<EquipmentLoadoutEntry>(inventorySystem.Data.equippedInstances);
        entries.RemoveAll(x => x.slot == slot);
        if (instance != null) entries.Add(new EquipmentLoadoutEntry { slot = slot, instanceId = instanceId });
        inventorySystem.Data.equippedInstances = entries.ToArray();
        return true;
    }

    private bool IsLegacyOwned(string id) => playerData?.ownedEquipmentIds != null &&
        Array.IndexOf(playerData.ownedEquipmentIds, id) >= 0;

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
