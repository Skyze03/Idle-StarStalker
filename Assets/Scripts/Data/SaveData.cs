using System;

[Serializable]
public class SaveData
{
    public int level;
    public int exp;
    public int energy;

    public int headLevel;
    public int armsLevel;
    public int legsLevel;
    public int chestLevel;
    public int feetLevel;
    public int weaponLevel;

    public bool autoMeditationUnlocked;
    public bool autoMeditationEnabled;
    public float autoMeditationTimer;

    public int memoryFragmentCount;
    public int runeCount;

    public int meditationExpBonus;
    public int collectionEnergyBonus;

    public bool mainStageInitialized;
    public int selectedStage;
    public int highestUnlockedStage;
    public int highestClearedStage;
    public int battleStamina;
    public long lastStaminaRefreshUtcTicks;
    public string[] unlockedUltimateIds;
    public string equippedUltimateId;
    public string[] ownedEquipmentIds;
    public string equippedHeadItemId;
    public string equippedChestItemId;
    public string equippedArmsItemId;
    public string equippedLegsItemId;
    public string equippedFeetItemId;
    public string equippedWeaponItemId;
    public string equippedAccessoryItemId;
    public bool equipmentInitialized;
}
