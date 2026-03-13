using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI meditationBonusText;
    [SerializeField] private TextMeshProUGUI collectionBonusText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Button goToMeditationFromStatsButton;

    private PlayerData playerData;
    private PanelSwitcher panelSwitcher;

    public void Setup(PlayerData data, PanelSwitcher switcher)
    {
        playerData = data;
        panelSwitcher = switcher;

        if (goToMeditationFromStatsButton != null)
        {
            goToMeditationFromStatsButton.onClick.RemoveAllListeners();
            goToMeditationFromStatsButton.onClick.AddListener(OnGoToMeditationClicked);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (playerData == null || playerData.stats == null)
        {
            return;
        }

        if (meditationBonusText != null)
        {
            meditationBonusText.text = "Meditation Bonus: " + playerData.stats.meditationExpBonus;
        }

        if (collectionBonusText != null)
        {
            collectionBonusText.text = "Collection Bonus: " + playerData.stats.collectionEnergyBonus;
        }

        if (hpText != null)
        {
            hpText.text = "HP: " + playerData.stats.hp;
        }

        if (attackText != null)
        {
            attackText.text = "Attack: " + playerData.stats.attack;
        }

        if (defenseText != null)
        {
            defenseText.text = "Defense: " + playerData.stats.defense;
        }

        if (speedText != null)
        {
            speedText.text = "Speed: " + playerData.stats.speed;
        }
    }

    private void OnGoToMeditationClicked()
    {
        if (panelSwitcher == null)
        {
            Debug.LogWarning("StatsUI: panelSwitcher is null.");
            return;
        }

        panelSwitcher.ShowMeditationPanel();
    }
}