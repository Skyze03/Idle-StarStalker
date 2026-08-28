using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class StatsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI meditationBonusText;
    [SerializeField] private TextMeshProUGUI collectionBonusText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [FormerlySerializedAs("speedText")]
    [SerializeField] private TextMeshProUGUI agilityText;
    [SerializeField] private TextMeshProUGUI wisdomText;
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

        if (agilityText != null)
        {
            agilityText.text = "Agility: " + playerData.stats.agility;
        }

        if (wisdomText != null)
        {
            wisdomText.text = "Wisdom: " + playerData.stats.wisdom;
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
