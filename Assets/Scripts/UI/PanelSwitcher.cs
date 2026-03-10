using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject meditationPanel;
    [SerializeField] private GameObject collectionPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject combinerPanel;

    [Header("Navigation Buttons")]
    [SerializeField] private Button goToCollectionButton;
    [SerializeField] private Button goToUpgradeButton;
    [SerializeField] private Button goToInventoryButton;
    [SerializeField] private Button goToCombinerButton;

    public void Setup()
    {
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

        ShowMeditationPanel();
    }

    public void ShowMeditationPanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(true);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
    }

    public void ShowCollectionPanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
    }

    public void ShowUpgradePanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(false);
    }

    public void ShowInventoryPanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        if (combinerPanel != null) combinerPanel.SetActive(false);
    }

    public void ShowCombinerPanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (combinerPanel != null) combinerPanel.SetActive(true);
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
}