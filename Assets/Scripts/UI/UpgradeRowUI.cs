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
            int level = playerData.GetPartLevel(partType);
            levelText.text = $"{partType} Lv.{level}\n{GetProgressionText(level)}";
        }

        if (costText != null)
        {
            costText.gameObject.SetActive(false);
        }

        if (upgradeButton != null)
        {
            upgradeButton.interactable = playerData.CanUpgradePart(partType);
            TMP_Text buttonLabel = upgradeButton.GetComponentInChildren<TMP_Text>(true);
            if (buttonLabel != null)
            {
                buttonLabel.text = playerData.CanUpgradePart(partType)
                    ? $"{playerData.GetPartUpgradeCost(partType)} ENERGY"
                    : $"PLAYER LV.{playerData.GetPartLevel(partType) + 1} REQUIRED";
            }
        }
    }

    private string GetProgressionText(int level)
    {
        switch (partType)
        {
            case BodyPartType.Weapon:
                return $"Attack {10 + (level - 1) * 5}  →  {10 + level * 5}";
            case BodyPartType.Head:
                return $"WIS {5 + (level - 1) * 2} → {5 + level * 2}\n" +
                    $"Meditate +{level - 1} → +{level}";
            case BodyPartType.Arms:
                return $"Collection +{level - 1}  →  +{level}";
            case BodyPartType.Chest:
                return $"Defense {5 + (level - 1) * 3}  →  {5 + level * 3}";
            case BodyPartType.Legs:
                return $"HP {100 + (level - 1) * 20}  →  {100 + level * 20}";
            case BodyPartType.Feet:
                return $"Agility {5 + (level - 1) * 2}  →  {5 + level * 2}";
            default:
                return string.Empty;
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
