using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditBuildUI : MonoBehaviour
{
    [SerializeField] private GameObject equipmentPage;
    [SerializeField] private GameObject ultimatePage;
    [SerializeField] private GameObject summaryPage;
    [SerializeField] private Button equipmentTabButton;
    [SerializeField] private Button ultimateTabButton;
    [SerializeField] private Button summaryTabButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private TMP_Text equipmentSummaryText;
    [SerializeField] private TMP_Text buildSummaryText;
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private Button[] equipmentChoiceButtons;
    [SerializeField] private Button[] ultimateButtons;
    [SerializeField] private TMP_Text equipmentDetailText;
    [SerializeField] private Button equipSelectedButton;
    [SerializeField] private Button lockSelectedButton;
    [SerializeField] private Button upgradeSelectedButton;
    [SerializeField] private Button dismantleSelectedButton;

    private EquipmentSystem equipmentSystem;
    private UltimateSystem ultimateSystem;
    private PlayerData playerData;
    private BattleSystem battleSystem;
    private PanelSwitcher panelSwitcher;
    private EquipmentSlot selectedSlot = EquipmentSlot.Head;
    private EquipmentInstance selectedInstance;

    public EquipmentSlot SelectedSlot => selectedSlot;

    public void Setup(EquipmentSystem equipment, UltimateSystem ultimates,
        PlayerData player, BattleSystem battle, PanelSwitcher switcher)
    {
        equipmentSystem = equipment;
        ultimateSystem = ultimates;
        playerData = player;
        battleSystem = battle;
        panelSwitcher = switcher;

        BindTab(equipmentTabButton, ShowEquipmentPage);
        BindTab(ultimateTabButton, ShowUltimatePage);
        BindTab(summaryTabButton, ShowSummaryPage);
        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() => panelSwitcher?.ShowMeditationPanel());
        }

        EquipmentSlot[] slots = (EquipmentSlot[])Enum.GetValues(typeof(EquipmentSlot));
        for (int i = 0; i < slotButtons.Length && i < slots.Length; i++)
        {
            EquipmentSlot slot = slots[i];
            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => SelectSlot(slot));
        }

        string[] ultimateIds =
        {
            "star_burst", "iron_retaliation", "rapid_nova",
            "meteor_flurry", "swift_ascension"
        };
        for (int i = 0; i < ultimateButtons.Length && i < ultimateIds.Length; i++)
        {
            string id = ultimateIds[i];
            ultimateButtons[i].onClick.RemoveAllListeners();
            ultimateButtons[i].onClick.AddListener(() =>
            {
                ultimateSystem?.Equip(id);
                Refresh();
            });
        }

        ShowEquipmentPage();
        BindTab(equipSelectedButton, EquipSelected);
        BindTab(lockSelectedButton, ToggleSelectedLock);
        BindTab(upgradeSelectedButton, UpgradeSelected);
        BindTab(dismantleSelectedButton, DismantleSelected);
    }

    public void ShowEquipmentPage() => ShowPage(equipmentPage);
    public void ShowUltimatePage() => ShowPage(ultimatePage);
    public void ShowSummaryPage() => ShowPage(summaryPage);

    private void ShowPage(GameObject page)
    {
        if (equipmentPage != null) equipmentPage.SetActive(page == equipmentPage);
        if (ultimatePage != null) ultimatePage.SetActive(page == ultimatePage);
        if (summaryPage != null) summaryPage.SetActive(page == summaryPage);
        Refresh();
    }

    private void SelectSlot(EquipmentSlot slot)
    {
        selectedSlot = slot;
        Refresh();
    }

    public void Refresh()
    {
        RefreshEquipment();
        RefreshUltimates();
        RefreshSummary();
    }

    private void RefreshEquipment()
    {
        if (equipmentSystem == null) return;
        EquipmentSlot[] slots = (EquipmentSlot[])Enum.GetValues(typeof(EquipmentSlot));
        for (int i = 0; i < slotButtons.Length && i < slots.Length; i++)
        {
            EquipmentData equipped = equipmentSystem.GetEquipped(slots[i]);
            SetLabel(slotButtons[i],
                $"{slots[i]}\n{(equipped == null ? "Empty" : equipped.itemName)}");
        }

        List<EquipmentInstance> choices = equipmentSystem.GetInstancesForSlot(selectedSlot);
        for (int i = 0; i < equipmentChoiceButtons.Length; i++)
        {
            Button button = equipmentChoiceButtons[i];
            button.onClick.RemoveAllListeners();
            if (i == 0)
            {
                button.gameObject.SetActive(true);
                SetLabel(button, "UNEQUIP\nLeave this slot empty");
                button.interactable = equipmentSystem.GetEquipped(selectedSlot) != null;
                button.onClick.AddListener(() =>
                {
                    equipmentSystem.Equip(selectedSlot, string.Empty);
                    Refresh();
                });
                continue;
            }

            int choiceIndex = i - 1;
            if (choiceIndex >= choices.Count)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            EquipmentInstance instance = choices[choiceIndex];
            EquipmentData item = EquipmentData.GetById(instance.templateId);
            button.gameObject.SetActive(true);
            bool selected = selectedInstance?.instanceId == instance.instanceId;
            SetLabel(button, $"{item.itemName}  Lv.{instance.level}\n{item.description}\n" +
                (instance.locked ? "LOCKED" : selected ? "SELECTED" : "Tap for details"));
            button.interactable = true;
            button.onClick.AddListener(() =>
            {
                selectedInstance = instance;
                Refresh();
            });
        }

        if (equipmentSummaryText != null)
            equipmentSummaryText.text =
                $"{selectedSlot} choices — owned equipment {equipmentSystem.OwnedCount}/10";
        if (equipmentDetailText != null) equipmentDetailText.text = BuildComparisonText();
        if (equipSelectedButton != null) equipSelectedButton.interactable = selectedInstance != null;
        if (lockSelectedButton != null) lockSelectedButton.interactable = selectedInstance != null;
        if (upgradeSelectedButton != null)
        {
            upgradeSelectedButton.interactable = selectedInstance != null &&
                selectedInstance.level < EquipmentSystem.MaxEquipmentLevel;
            SetLabel(upgradeSelectedButton, selectedInstance == null ? "Select Equipment" :
                selectedInstance.level >= EquipmentSystem.MaxEquipmentLevel ? "MAX LEVEL" :
                $"Upgrade  {equipmentSystem.GetUpgradeCost(selectedInstance)} Energy + " +
                $"{equipmentSystem.GetUpgradeStarDustCost(selectedInstance)} Dust");
        }
        if (dismantleSelectedButton != null)
        {
            bool protectedItem = selectedInstance == null || selectedInstance.locked ||
                equipmentSystem.IsEquippedInstance(selectedInstance.instanceId);
            dismantleSelectedButton.interactable = !protectedItem;
            SetLabel(dismantleSelectedButton,
                selectedInstance == null ? "Select Equipment" :
                selectedInstance.locked ? "Locked" :
                equipmentSystem.IsEquippedInstance(selectedInstance.instanceId) ? "Equipped" :
                $"Dismantle  +{EquipmentSystem.DismantleStarDustReward} Dust");
        }
    }

    private void EquipSelected()
    {
        if (selectedInstance != null) equipmentSystem.EquipInstance(selectedSlot, selectedInstance.instanceId);
        Refresh();
    }

    private void ToggleSelectedLock()
    {
        if (selectedInstance != null) equipmentSystem.ToggleLock(selectedInstance.instanceId);
        Refresh();
    }

    private void UpgradeSelected()
    {
        if (selectedInstance != null) equipmentSystem.TryUpgrade(selectedInstance.instanceId);
        Refresh();
    }

    private void DismantleSelected()
    {
        if (selectedInstance != null &&
            equipmentSystem.TryDismantle(selectedInstance.instanceId))
            selectedInstance = null;
        Refresh();
    }

    private string BuildComparisonText()
    {
        if (selectedInstance == null) return "Select an equipment instance";
        EquipmentData selectedData = EquipmentData.GetById(selectedInstance.templateId);
        EquipmentInstance current = equipmentSystem.GetEquippedInstance(selectedSlot);
        EquipmentStatBlock selected = equipmentSystem.GetEffectiveStats(selectedInstance);
        EquipmentStatBlock equipped = equipmentSystem.GetEffectiveStats(current);
        string currentName = current == null ? "Empty" :
            $"{EquipmentData.GetById(current.templateId).itemName} Lv.{current.level}";
        return $"SELECTED: {selectedData.itemName} Lv.{selectedInstance.level}  " +
            (selectedInstance.locked ? "LOCKED" : "Unlocked") +
            $"\nCURRENT: {currentName}\n" +
            Compare("HP", selected.hp, equipped.hp) + "   " + Compare("ATK", selected.attack, equipped.attack) +
            "   " + Compare("DEF", selected.defense, equipped.defense) + "\n" +
            Compare("AGI", selected.agility, equipped.agility) + "   " + Compare("WIS", selected.wisdom, equipped.wisdom) +
            $"   Rage/A {selected.rageOnAttack:0.0} ({selected.rageOnAttack - equipped.rageOnAttack:+0.0;-0.0;0.0})";
    }

    private static string Compare(string name, int selected, int current) =>
        $"{name} {selected} ({selected - current:+#;-#;0})";

    private void RefreshUltimates()
    {
        if (ultimateSystem == null || playerData == null) return;
        IReadOnlyList<UltimateData> catalog = ultimateSystem.Catalog;
        for (int i = 0; i < ultimateButtons.Length && i < catalog.Count; i++)
        {
            UltimateData ultimate = catalog[i];
            bool unlocked = ultimateSystem.IsUnlocked(ultimate.id);
            bool equipped = playerData.equippedUltimateId == ultimate.id;
            ultimateButtons[i].interactable = unlocked && !equipped;
            SetLabel(ultimateButtons[i],
                $"{ultimate.ultimateName}\n{ultimate.description}\n" +
                (equipped ? "EQUIPPED" : unlocked ? "Tap to equip" : "LOCKED"));
        }
    }

    private void RefreshSummary()
    {
        if (buildSummaryText == null || battleSystem == null) return;
        CombatBuildSnapshot build = battleSystem.CreatePlayerBuildPreview();
        if (build == null) return;
        string equipmentLines = string.Empty;
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            EquipmentData item = equipmentSystem?.GetEquipped(slot);
            equipmentLines += item == null ? $"{slot}: Empty\n" : $"{slot}: {item.itemName}\n";
        }
        buildSummaryText.text =
            $"ULTIMATE\n{build.ultimate.ultimateName}\n{build.ultimate.description}\n\n" +
            $"FINAL COMBAT STATS\nHP {build.maxHP}   ATK {build.attack}   DEF {build.defense}\n" +
            $"AGI {build.agility}   WIS {build.wisdom}\n" +
            $"Rage/Attack +{build.bonusRageOnAttack:0}   Rage/Hit +{build.bonusRageOnHit:0}\n\n" +
            "EQUIPMENT\n" + equipmentLines;
    }

    private static void BindTab(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    private static void SetLabel(Button button, string value)
    {
        TMP_Text label = button != null ? button.GetComponentInChildren<TMP_Text>(true) : null;
        if (label != null) label.text = value;
    }
}
