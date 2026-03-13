using System;

[Serializable]
public class PlayerData
{
    public int level = 1;
    public int exp = 0;
    public int energy = 0;

    public int headLevel = 1;
    public int armsLevel = 1;
    public int legsLevel = 1;
    public int chestLevel = 1;
    public int feetLevel = 1;
    public int weaponLevel = 1;

    public PlayerStats stats = new PlayerStats();

    public void CalculateStats()
    {
        if (stats == null)
        {
            stats = new PlayerStats();
        }

        // 已接入现有系统的属性
        stats.meditationExpBonus = headLevel - 1;
        stats.collectionEnergyBonus = armsLevel - 1;

        // 先算出来，后面战斗系统再接
        stats.hp = 100 + (legsLevel - 1) * 20;
        stats.attack = 10 + (weaponLevel - 1) * 5;
        stats.defense = 5 + (chestLevel - 1) * 3;
        stats.speed = 5 + (feetLevel - 1) * 2;

    }

    public int GetRequiredExp()
    {
        return level * 100;
    }

    public int GetPartLevel(BodyPartType partType)
    {
        switch (partType)
        {
            case BodyPartType.Head:
                return headLevel;

            case BodyPartType.Arms:
                return armsLevel;

            case BodyPartType.Legs:
                return legsLevel;

            case BodyPartType.Chest:
                return chestLevel;

            case BodyPartType.Feet:
                return feetLevel;

            case BodyPartType.Weapon:
                return weaponLevel;

            default:
                return 1;
        }
    }

    public int GetPartUpgradeCost(BodyPartType partType)
    {
        int currentLevel = GetPartLevel(partType);
        return currentLevel * 10;
    }

    public void UpgradePart(BodyPartType partType)
    {
        switch (partType)
        {
            case BodyPartType.Head:
                headLevel++;
                break;

            case BodyPartType.Arms:
                armsLevel++;
                break;

            case BodyPartType.Legs:
                legsLevel++;
                break;

            case BodyPartType.Chest:
                chestLevel++;
                break;

            case BodyPartType.Feet:
                feetLevel++;
                break;

            case BodyPartType.Weapon:
                weaponLevel++;
                break;
        }

        CalculateStats();
    }
}