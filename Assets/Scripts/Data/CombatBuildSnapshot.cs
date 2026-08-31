using System;

[Serializable]
public class CombatBuildSnapshot
{
    public int maxHP;
    public int attack;
    public int defense;
    public int agility;
    public int wisdom;
    public UltimateData ultimate;
    public float bonusRageOnAttack;
    public float bonusRageOnHit;

    public static CombatBuildSnapshot FromPlayer(
        PlayerData player,
        EquipmentSystem equipmentSystem)
    {
        CombatBuildSnapshot snapshot = new CombatBuildSnapshot
        {
            maxHP = player.stats.hp,
            attack = player.stats.attack,
            defense = player.stats.defense,
            agility = player.stats.agility,
            wisdom = player.stats.wisdom,
            ultimate = UltimateData.GetById(player.equippedUltimate.id)
        };

        if (equipmentSystem != null)
        {
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                EquipmentStatBlock item = equipmentSystem.GetEffectiveStats(equipmentSystem.GetEquippedInstance(slot));
                snapshot.maxHP += item.hp; snapshot.attack += item.attack; snapshot.defense += item.defense;
                snapshot.agility += item.agility; snapshot.wisdom += item.wisdom;
                snapshot.bonusRageOnAttack += item.rageOnAttack; snapshot.bonusRageOnHit += item.rageOnHit;
            }
        }
        return snapshot;
    }

    public static CombatBuildSnapshot FromEnemy(EnemyData enemy)
    {
        CombatBuildSnapshot snapshot = new CombatBuildSnapshot
        {
            maxHP = enemy.maxHP,
            attack = enemy.attack,
            defense = enemy.defense,
            agility = enemy.agility,
            wisdom = enemy.wisdom,
            ultimate = UltimateData.GetById(enemy.equippedUltimate.id)
        };
        if ((enemy.traits & EnemyTrait.Frenzy) != 0)
            snapshot.bonusRageOnAttack += 5f;
        if ((enemy.traits & EnemyTrait.Bulwark) != 0)
            snapshot.bonusRageOnHit += 8f;
        return snapshot;
    }
}
