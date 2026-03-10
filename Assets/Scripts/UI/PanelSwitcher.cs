using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject meditationPanel;
    [SerializeField] private GameObject collectionPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject inventoryPanel;

    [Header("Navigation Buttons")]
    [SerializeField] private Button goToCollectionButton;
    [SerializeField] private Button goToUpgradeButton;
    [SerializeField] private Button goToInventoryButton;

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

        ShowMeditationPanel();
    }

    public void ShowMeditationPanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(true);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    public void ShowCollectionPanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    public void ShowUpgradePanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    public void ShowInventoryPanel()
    {
        if (meditationPanel != null) meditationPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
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
}