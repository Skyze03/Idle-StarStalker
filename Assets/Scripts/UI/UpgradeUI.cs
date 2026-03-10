using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI bodyLevelText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button goToMeditationFromUpgradeButton;

    private UpgradeSystem upgradeSystem;
    private PlayerData playerData;
    private PanelSwitcher panelSwitcher;

    public void Setup(UpgradeSystem system, PlayerData data, PanelSwitcher switcher)
    {
        upgradeSystem = system;
        playerData = data;
        panelSwitcher = switcher;

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }

        if (goToMeditationFromUpgradeButton != null)
        {
            goToMeditationFromUpgradeButton.onClick.RemoveAllListeners();
            goToMeditationFromUpgradeButton.onClick.AddListener(OnGoToMeditationClicked);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (playerData == null)
        {
            return;
        }

        if (bodyLevelText != null)
        {
            bodyLevelText.text = "Body Level: " + playerData.bodyLevel;
        }

        if (upgradeCostText != null)
        {
            upgradeCostText.text = "Upgrade Cost: " + playerData.GetBodyUpgradeCost() + " Energy";
        }
    }

    private void OnUpgradeButtonClicked()
    {
        if (upgradeSystem == null)
        {
            Debug.LogWarning("UpgradeUI: upgradeSystem is null.");
            return;
        }

        upgradeSystem.TryUpgradeBody();
        Refresh();
    }

    private void OnGoToMeditationClicked()
    {
        if (panelSwitcher == null)
        {
            Debug.LogWarning("UpgradeUI: panelSwitcher is null.");
            return;
        }

        panelSwitcher.ShowMeditationPanel();
    }
}