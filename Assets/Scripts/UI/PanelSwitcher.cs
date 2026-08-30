using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    private BattleState battleState;

    [Header("Panel References")]
    [SerializeField] private GameObject meditationPanel;
    [SerializeField] private GameObject collectionPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject combinerPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject ultimatePanel;
    [SerializeField] private GameObject equipmentPanel;

    [Header("Navigation Buttons")]
    [SerializeField] private Button goToCollectionButton;
    [SerializeField] private Button goToUpgradeButton;
    [SerializeField] private Button goToInventoryButton;
    [SerializeField] private Button goToCombinerButton;
    [SerializeField] private Button goToStatsButton;
    [SerializeField] private Button goToBattleButton;
    [SerializeField] private Button goToUltimateButton;
    [SerializeField] private Button goToEquipmentButton;

    public void Setup(BattleState battleState)
    {
        this.battleState = battleState;

        if (goToCollectionButton != null)
        {
            goToCollectionButton.onClick.RemoveAllListeners();
            goToCollectionButton.onClick.AddListener(OnGoToCollectionClicked);
        }

        if (goToUpgradeButton != null)
        {
            goToUpgradeButton.onClick.RemoveAllListeners();
            goToUpgradeButton.onClick.AddListener(OnGoToUpgradeClicked);
        }

        if (goToInventoryButton != null)
        {
            goToInventoryButton.onClick.RemoveAllListeners();
            goToInventoryButton.onClick.AddListener(OnGoToInventoryClicked);
        }

        if (goToCombinerButton != null)
        {
            goToCombinerButton.onClick.RemoveAllListeners();
            goToCombinerButton.onClick.AddListener(OnGoToCombinerClicked);
        }

        if (goToStatsButton != null)
        {
            goToStatsButton.onClick.RemoveAllListeners();
            goToStatsButton.onClick.AddListener(OnGoToStatsClicked);
        }

        if (goToBattleButton != null)
        {
            goToBattleButton.onClick.RemoveAllListeners();
            goToBattleButton.onClick.AddListener(OnGoToBattleClicked);
        }

        if (goToUltimateButton != null)
        {
            goToUltimateButton.onClick.RemoveAllListeners();
            goToUltimateButton.onClick.AddListener(ShowUltimatePanel);
        }
        if (goToEquipmentButton != null)
        {
            goToEquipmentButton.onClick.RemoveAllListeners();
            goToEquipmentButton.onClick.AddListener(ShowEquipmentPanel);
        }

        ShowMeditationPanel();
        Refresh();
    }

    public void Refresh()
    {
        bool navigationLocked = IsBattleRunning();

        if (goToCollectionButton != null)
            goToCollectionButton.interactable = !navigationLocked;
        if (goToUpgradeButton != null)
            goToUpgradeButton.interactable = !navigationLocked;
        if (goToInventoryButton != null)
            goToInventoryButton.interactable = !navigationLocked;
        if (goToCombinerButton != null)
            goToCombinerButton.interactable = !navigationLocked;
        if (goToStatsButton != null)
            goToStatsButton.interactable = !navigationLocked;
        if (goToUltimateButton != null)
            goToUltimateButton.interactable = !navigationLocked;
        if (goToEquipmentButton != null)
            goToEquipmentButton.interactable = !navigationLocked;
    }

    private bool IsBattleRunning()
    {
        return battleState != null && battleState.battleRunning;
    }

    private bool CanLeaveBattle()
    {
        if (!IsBattleRunning())
        {
            return true;
        }

        Debug.Log("Cannot leave the battle until it has ended.");
        return false;
    }

    public void ShowMeditationPanel()
    {
        if (!CanLeaveBattle()) return;

        if (meditationPanel != null) meditationPanel.SetActive(true);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (ultimatePanel != null) ultimatePanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
    }

    public void ShowCollectionPanel()
    {
        if (!CanLeaveBattle()) return;

        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (ultimatePanel != null) ultimatePanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
    }

    public void ShowUpgradePanel()
    {
        if (!CanLeaveBattle()) return;

        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (ultimatePanel != null) ultimatePanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
    }

    public void ShowInventoryPanel()
    {
        if (!CanLeaveBattle()) return;

        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (ultimatePanel != null) ultimatePanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
    }

    public void ShowCombinerPanel()
    {
        if (!CanLeaveBattle()) return;

        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(true);
        if (statsPanel != null) statsPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (ultimatePanel != null) ultimatePanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
    }

    public void ShowStatsPanel()
    {
        if (!CanLeaveBattle()) return;

        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(true);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (ultimatePanel != null) ultimatePanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
    }

    public void ShowBattlePanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(true);
        if (ultimatePanel != null) ultimatePanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
    }

    public void ShowUltimatePanel()
    {
        if (!CanLeaveBattle()) return;
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (ultimatePanel != null) ultimatePanel.SetActive(true);
        if (equipmentPanel != null) equipmentPanel.SetActive(false);
    }

    public void ShowEquipmentPanel()
    {
        if (!CanLeaveBattle()) return;
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(false);
        if (battlePanel != null) battlePanel.SetActive(false);
        if (ultimatePanel != null) ultimatePanel.SetActive(false);
        if (equipmentPanel != null) equipmentPanel.SetActive(true);
    }

    private void OnGoToCollectionClicked()
    {
        ShowCollectionPanel();
    }

    private void OnGoToUpgradeClicked()
    {
        ShowUpgradePanel();
    }

    private void OnGoToInventoryClicked()
    {
        ShowInventoryPanel();
    }

    private void OnGoToCombinerClicked()
    {
        ShowCombinerPanel();
    }

    private void OnGoToStatsClicked()
    {
        ShowStatsPanel();
    }

    private void OnGoToBattleClicked()
    {
        ShowBattlePanel();
    }
}
