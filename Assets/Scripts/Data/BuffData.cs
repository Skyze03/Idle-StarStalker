using System;

[Serializable]
public class BuffData
{
    public int meditationExpBonus = 0;
    public int collectionEnergyBonus = 0;

    public string GetCurrentBuffDescription()
    {
        if (meditationExpBonus <= 0 && collectionEnergyBonus <= 0)
        {
            return "Current Buff: None";
        }

        string description = "Current Buff: ";

        if (meditationExpBonus > 0)
        {
            description += "Meditation EXP +" + meditationExpBonus;
        }

        if (collectionEnergyBonus > 0)
        {
            if (meditationExpBonus > 0)
            {
                description += " | ";
            }

            description += "Collection Energy +" + collectionEnergyBonus;
        }

        return description;
    }
}