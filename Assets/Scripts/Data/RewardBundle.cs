using System;

[Serializable]
public class RewardBundle
{
    public int energy;
    public int memoryFragments;
    public int runes;
    public string equipmentTemplateId;
    public string ultimateId;
    public string[] equipmentTemplateIds = Array.Empty<string>();

    public RewardBundle(int energy = 0, int memoryFragments = 0, int runes = 0)
    {
        this.energy = energy;
        this.memoryFragments = memoryFragments;
        this.runes = runes;
    }

    public void Add(RewardBundle other)
    {
        if (other == null) return;

        energy += other.energy;
        memoryFragments += other.memoryFragments;
        runes += other.runes;
        if (!string.IsNullOrEmpty(other.equipmentTemplateId)) equipmentTemplateId = other.equipmentTemplateId;
        if (!string.IsNullOrEmpty(other.ultimateId)) ultimateId = other.ultimateId;
        foreach (string id in other.equipmentTemplateIds ?? Array.Empty<string>()) AddEquipment(id);
    }

    public void AddEquipment(string templateId)
    {
        if (string.IsNullOrEmpty(templateId)) return;
        string[] next = new string[(equipmentTemplateIds?.Length ?? 0) + 1];
        if (equipmentTemplateIds != null) Array.Copy(equipmentTemplateIds, next, equipmentTemplateIds.Length);
        next[next.Length - 1] = templateId;
        equipmentTemplateIds = next;
    }
}
