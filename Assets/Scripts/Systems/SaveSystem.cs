using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    public void SaveGame(PlayerData playerData, MeditationState meditationState, InventoryData inventoryData, BuffData buffData)
    {
        if (playerData == null || meditationState == null || inventoryData == null || buffData == null)
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

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game saved to: " + saveFilePath);
    }

    public bool LoadGame(PlayerData playerData, MeditationState meditationState, InventoryData inventoryData, BuffData buffData)
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No save file found.");
            return false;
        }

        if (playerData == null || meditationState == null || inventoryData == null || buffData == null)
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

        playerData.CalculateStats();

        meditationState.autoMeditationUnlocked = saveData.autoMeditationUnlocked;
        meditationState.autoMeditationEnabled = saveData.autoMeditationEnabled;
        meditationState.autoMeditationTimer = saveData.autoMeditationTimer;

        inventoryData.memoryFragmentCount = saveData.memoryFragmentCount;
        inventoryData.runeCount = saveData.runeCount;

        buffData.meditationExpBonus = saveData.meditationExpBonus;
        buffData.collectionEnergyBonus = saveData.collectionEnergyBonus;

        Debug.Log("Game loaded from: " + saveFilePath);
        return true;
    }
}