using System;
using System.Collections.Generic;

[Serializable]
public class EquipmentData
{
    public string id;
    public string itemName;
    public EquipmentSlot slot;
    public string description;
    public int hp;
    public int attack;
    public int defense;
    public int agility;
    public int wisdom;
    public float rageOnAttack;
    public float rageOnHit;

    public EquipmentData(string id, string itemName, EquipmentSlot slot,
        string description, int hp = 0, int attack = 0, int defense = 0,
        int agility = 0, int wisdom = 0, float rageOnAttack = 0f,
        float rageOnHit = 0f)
    {
        this.id = id;
        this.itemName = itemName;
        this.slot = slot;
        this.description = description;
        this.hp = hp;
        this.attack = attack;
        this.defense = defense;
        this.agility = agility;
        this.wisdom = wisdom;
        this.rageOnAttack = rageOnAttack;
        this.rageOnHit = rageOnHit;
    }

    public static EquipmentData GetById(string id)
    {
        foreach (EquipmentData item in GetAll())
            if (item.id == id) return item;
        return null;
    }

    public static List<EquipmentData> GetAll()
    {
        return new List<EquipmentData>
        {
            new EquipmentData("seer_circlet", "Seer Circlet", EquipmentSlot.Head,
                "+4 Wisdom", wisdom: 4),
            new EquipmentData("war_visor", "War Visor", EquipmentSlot.Head,
                "+3 Attack", attack: 3),
            new EquipmentData("iron_carapace", "Iron Carapace", EquipmentSlot.Chest,
                "+5 Defense", defense: 5),
            new EquipmentData("vital_shell", "Vital Shell", EquipmentSlot.Chest,
                "+40 HP", hp: 40),
            new EquipmentData("fury_bracers", "Fury Bracers", EquipmentSlot.Arms,
                "+2 Attack; attacks gain +4 Rage", attack: 2, rageOnAttack: 4f),
            new EquipmentData("guardian_leggings", "Guardian Leggings", EquipmentSlot.Legs,
                "+25 HP; +2 Defense", hp: 25, defense: 2),
            new EquipmentData("windstep_boots", "Windstep Boots", EquipmentSlot.Feet,
                "+4 Agility", agility: 4),
            new EquipmentData("star_blade", "Star Blade", EquipmentSlot.Weapon,
                "+7 Attack", attack: 7),
            new EquipmentData("quickfang", "Quickfang", EquipmentSlot.Weapon,
                "+3 Attack; +2 Agility; attacks gain +3 Rage",
                attack: 3, agility: 2, rageOnAttack: 3f),
            new EquipmentData("thorn_sigil", "Thorn Sigil", EquipmentSlot.Accessory,
                "+3 Defense; gain +6 Rage when hit", defense: 3, rageOnHit: 6f)
        };
    }
}
