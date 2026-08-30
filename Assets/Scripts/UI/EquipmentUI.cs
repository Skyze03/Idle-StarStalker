using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private Button headButton;
    [SerializeField] private Button chestButton;
    [SerializeField] private Button armsButton;
    [SerializeField] private Button legsButton;
    [SerializeField] private Button feetButton;
    [SerializeField] private Button weaponButton;
    [SerializeField] private Button accessoryButton;
    [SerializeField] private Button returnButton;

    private EquipmentSystem equipmentSystem;
    private PanelSwitcher panelSwitcher;

    public void Setup(EquipmentSystem system, PanelSwitcher switcher)
    {
        equipmentSystem = system;
        panelSwitcher = switcher;
        Bind(headButton, EquipmentSlot.Head);
        Bind(chestButton, EquipmentSlot.Chest);
        Bind(armsButton, EquipmentSlot.Arms);
        Bind(legsButton, EquipmentSlot.Legs);
        Bind(feetButton, EquipmentSlot.Feet);
        Bind(weaponButton, EquipmentSlot.Weapon);
        Bind(accessoryButton, EquipmentSlot.Accessory);
        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() => panelSwitcher?.ShowMeditationPanel());
        }
        Refresh();
    }

    public void Refresh()
    {
        if (equipmentSystem == null) return;
        RefreshButton(headButton, EquipmentSlot.Head);
        RefreshButton(chestButton, EquipmentSlot.Chest);
        RefreshButton(armsButton, EquipmentSlot.Arms);
        RefreshButton(legsButton, EquipmentSlot.Legs);
        RefreshButton(feetButton, EquipmentSlot.Feet);
        RefreshButton(weaponButton, EquipmentSlot.Weapon);
        RefreshButton(accessoryButton, EquipmentSlot.Accessory);

        int hp = 0, attack = 0, defense = 0, agility = 0, wisdom = 0;
        float rageAttack = 0f, rageHit = 0f;
        foreach (EquipmentData item in equipmentSystem.GetAllEquipped())
        {
            hp += item.hp; attack += item.attack; defense += item.defense;
            agility += item.agility; wisdom += item.wisdom;
            rageAttack += item.rageOnAttack; rageHit += item.rageOnHit;
        }

        if (summaryText != null)
            summaryText.text = $"Owned: {equipmentSystem.OwnedCount}/10\n" +
                $"Equipment Total: HP +{hp}  ATK +{attack}  " +
                $"DEF +{defense}  AGI +{agility}  WIS +{wisdom}\n" +
                $"Rage on Attack +{rageAttack:0}  Rage when Hit +{rageHit:0}";
    }

    private void Bind(Button button, EquipmentSlot slot)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            equipmentSystem.Cycle(slot);
            Refresh();
        });
    }

    private void RefreshButton(Button button, EquipmentSlot slot)
    {
        if (button == null) return;
        EquipmentData item = equipmentSystem.GetEquipped(slot);
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
            label.text = item == null
                ? $"{slot}: None — tap to cycle"
                : $"{slot}: {item.itemName}\n{item.description}";
    }
}
