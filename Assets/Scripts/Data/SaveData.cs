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
}