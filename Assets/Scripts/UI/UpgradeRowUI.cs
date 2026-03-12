using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeRowUI : MonoBehaviour
{
    [Header("Row Settings")]
    [SerializeField] private BodyPartType partType;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeButton;

    private UpgradeSystem upgradeSystem;
    private PlayerData playerData;

    public void Setup(UpgradeSystem system, PlayerData data)
    {
        upgradeSystem = system;
        playerData = data;

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (playerData == null)
        {
            return;
        }

        if (levelText != null)
        {
            levelText.text = partType + " Lv: " + playerData.GetPartLevel(partType);
        }

        if (costText != null)
        {
            costText.text = "Cost: " + playerData.GetPartUpgradeCost(partType) + " Energy";
        }
    }

    private void OnUpgradeClicked()
    {
        if (upgradeSystem == null)
        {
            Debug.LogWarning("UpgradeRowUI: upgradeSystem is null.");
            return;
        }

        upgradeSystem.TryUpgradePart(partType);
        Refresh();
    }
}