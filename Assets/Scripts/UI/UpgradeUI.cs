using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("Row References")]
    [SerializeField] private UpgradeRowUI headRow;
    [SerializeField] private UpgradeRowUI armsRow;
    [SerializeField] private UpgradeRowUI legsRow;
    [SerializeField] private UpgradeRowUI chestRow;
    [SerializeField] private UpgradeRowUI feetRow;
    [SerializeField] private UpgradeRowUI weaponRow;

    [Header("Navigation")]
    [SerializeField] private Button goToMeditationFromUpgradeButton;

    private UpgradeSystem upgradeSystem;
    private PlayerData playerData;
    private PanelSwitcher panelSwitcher;

    public void Setup(UpgradeSystem system, PlayerData data, PanelSwitcher switcher)
    {
        upgradeSystem = system;
        playerData = data;
        panelSwitcher = switcher;

        if (headRow != null)
        {
            headRow.Setup(upgradeSystem, playerData);
        }

        if (armsRow != null)
        {
            armsRow.Setup(upgradeSystem, playerData);
        }

        if (legsRow != null)
        {
            legsRow.Setup(upgradeSystem, playerData);
        }

        if (chestRow != null)
        {
            chestRow.Setup(upgradeSystem, playerData);
        }

        if (feetRow != null)
        {
            feetRow.Setup(upgradeSystem, playerData);
        }

        if (weaponRow != null)
        {
            weaponRow.Setup(upgradeSystem, playerData);
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
        if (headRow != null) headRow.Refresh();
        if (armsRow != null) armsRow.Refresh();
        if (legsRow != null) legsRow.Refresh();
        if (chestRow != null) chestRow.Refresh();
        if (feetRow != null) feetRow.Refresh();
        if (weaponRow != null) weaponRow.Refresh();
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