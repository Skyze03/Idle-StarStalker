using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button collectButton;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private Button goToMeditationButton;

    private CollectionSystem collectionSystem;
    private PlayerData playerData;
    private PanelSwitcher panelSwitcher;

    public void Setup(CollectionSystem system, PlayerData data, PanelSwitcher switcher)
    {
        collectionSystem = system;
        playerData = data;
        panelSwitcher = switcher;

        if (collectButton != null)
        {
            collectButton.onClick.RemoveAllListeners();
            collectButton.onClick.AddListener(OnCollectButtonClicked);
        }

        if (goToMeditationButton != null)
        {
            goToMeditationButton.onClick.RemoveAllListeners();
            goToMeditationButton.onClick.AddListener(OnGoToMeditationClicked);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (playerData == null)
        {
            return;
        }

        if (energyText != null)
        {
            energyText.text = "Energy: " + playerData.energy;
        }
    }

    private void OnCollectButtonClicked()
    {
        if (collectionSystem == null)
        {
            Debug.LogWarning("CollectionUI: collectionSystem is null.");
            return;
        }

        collectionSystem.CollectOnce();
        Refresh();
    }

    private void OnGoToMeditationClicked()
    {
        if (panelSwitcher == null)
        {
            Debug.LogWarning("CollectionUI: panelSwitcher is null.");
            return;
        }

        panelSwitcher.ShowMeditationPanel();
    }
}