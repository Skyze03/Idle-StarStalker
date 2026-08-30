using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    public void SaveGame(PlayerData playerData, MeditationState meditationState, InventoryData inventoryData, BuffData buffData, MainStageState mainStageState)
    {
        if (playerData == null || meditationState == null || inventoryData == null || buffData == null || mainStageState == null)
        {
            Debug.LogWarning("SaveSystem: One or more data objects are null.");
            return;
        }

        SaveData saveData = new SaveData();

        saveData.level = playerData.level;
        saveData.exp = playerData.exp;
        saveData.energy = playerData.energy;

        saveData.headLevel = playerData.headLevel;
        saveData.armsLevel = playerData.armsLevel;
        saveData.legsLevel = playerData.legsLevel;
        saveData.chestLevel = playerData.chestLevel;
        saveData.feetLevel = playerData.feetLevel;
        saveData.weaponLevel = playerData.weaponLevel;

        saveData.autoMeditationUnlocked = meditationState.autoMeditationUnlocked;
        saveData.autoMeditationEnabled = meditationState.autoMeditationEnabled;
        saveData.autoMeditationTimer = meditationState.autoMeditationTimer;

        saveData.memoryFragmentCount = inventoryData.memoryFragmentCount;
        saveData.runeCount = inventoryData.runeCount;

        saveData.meditationExpBonus = buffData.meditationExpBonus;
        saveData.collectionEnergyBonus = buffData.collectionEnergyBonus;

        saveData.mainStageInitialized = true;
        saveData.selectedStage = mainStageState.selectedStage;
        saveData.highestUnlockedStage = mainStageState.highestUnlockedStage;
        saveData.highestClearedStage = mainStageState.highestClearedStage;
        saveData.battleStamina = mainStageState.battleStamina;
        saveData.lastStaminaRefreshUtcTicks = mainStageState.lastStaminaRefreshUtcTicks;
        saveData.unlockedUltimateIds = playerData.unlockedUltimateIds;
        saveData.equippedUltimateId = playerData.equippedUltimateId;
        saveData.ownedEquipmentIds = playerData.ownedEquipmentIds;
        saveData.equippedHeadItemId = playerData.equippedHeadItemId;
        saveData.equippedChestItemId = playerData.equippedChestItemId;
        saveData.equippedArmsItemId = playerData.equippedArmsItemId;
        saveData.equippedLegsItemId = playerData.equippedLegsItemId;
        saveData.equippedFeetItemId = playerData.equippedFeetItemId;
        saveData.equippedWeaponItemId = playerData.equippedWeaponItemId;
        saveData.equippedAccessoryItemId = playerData.equippedAccessoryItemId;
        saveData.equipmentInitialized = true;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game saved to: " + saveFilePath);
    }

    public bool LoadGame(PlayerData playerData, MeditationState meditationState, InventoryData inventoryData, BuffData buffData, MainStageState mainStageState)
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No save file found.");
            return false;
        }

        if (playerData == null || meditationState == null || inventoryData == null || buffData == null || mainStageState == null)
        {
            Debug.LogWarning("SaveSystem: One or more data objects are null.");
            return false;
        }

        string json = File.ReadAllText(saveFilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        playerData.level = saveData.level;
        playerData.exp = saveData.exp;
        playerData.energy = saveData.energy;

        playerData.headLevel = saveData.headLevel;
        playerData.armsLevel = saveData.armsLevel;
        playerData.legsLevel = saveData.legsLevel;
        playerData.chestLevel = saveData.chestLevel;
        playerData.feetLevel = saveData.feetLevel;
        playerData.weaponLevel = saveData.weaponLevel;

        playerData.unlockedUltimateIds = saveData.unlockedUltimateIds;
        playerData.equippedUltimateId = saveData.equippedUltimateId;
        if (playerData.unlockedUltimateIds == null ||
            playerData.unlockedUltimateIds.Length == 0)
        {
            playerData.unlockedUltimateIds = new[] { "star_burst" };
        }
        playerData.RefreshUltimate();
        playerData.ownedEquipmentIds = saveData.ownedEquipmentIds;
        playerData.equippedHeadItemId = saveData.equippedHeadItemId;
        playerData.equippedChestItemId = saveData.equippedChestItemId;
        playerData.equippedArmsItemId = saveData.equippedArmsItemId;
        playerData.equippedLegsItemId = saveData.equippedLegsItemId;
        playerData.equippedFeetItemId = saveData.equippedFeetItemId;
        playerData.equippedWeaponItemId = saveData.equippedWeaponItemId;
        playerData.equippedAccessoryItemId = saveData.equippedAccessoryItemId;

        if (!saveData.equipmentInitialized)
        {
            var legacyOwned = new System.Collections.Generic.List<string>();
            for (int stage = 1; stage <= saveData.highestClearedStage; stage++)
            {
                string itemId = EquipmentSystem.GetStageEquipmentId(stage);
                if (!string.IsNullOrEmpty(itemId) && !legacyOwned.Contains(itemId))
                    legacyOwned.Add(itemId);
            }
            playerData.ownedEquipmentIds = legacyOwned.ToArray();
        }

        playerData.CalculateStats();

        meditationState.autoMeditationUnlocked = saveData.autoMeditationUnlocked;
        meditationState.autoMeditationEnabled = saveData.autoMeditationEnabled;
        meditationState.autoMeditationTimer = saveData.autoMeditationTimer;

        inventoryData.memoryFragmentCount = saveData.memoryFragmentCount;
        inventoryData.runeCount = saveData.runeCount;

        buffData.meditationExpBonus = saveData.meditationExpBonus;
        buffData.collectionEnergyBonus = saveData.collectionEnergyBonus;

        if (saveData.mainStageInitialized)
        {
            mainStageState.selectedStage = saveData.selectedStage;
            mainStageState.highestUnlockedStage = saveData.highestUnlockedStage;
            mainStageState.highestClearedStage = saveData.highestClearedStage;
            mainStageState.battleStamina = saveData.battleStamina;
            mainStageState.lastStaminaRefreshUtcTicks = saveData.lastStaminaRefreshUtcTicks;
            mainStageState.Normalize();
        }
        else
        {
            mainStageState.ResetProgress();
        }

        Debug.Log("Game loaded from: " + saveFilePath);
        return true;
    }
}
