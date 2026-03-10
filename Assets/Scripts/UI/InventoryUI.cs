using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI memoryFragmentText;
    [SerializeField] private TextMeshProUGUI runeText;
    [SerializeField] private Button goToMeditationFromInventoryButton;

    private InventoryData inventoryData;
    private PanelSwitcher panelSwitcher;

    public void Setup(InventoryData data, PanelSwitcher switcher)
    {
        inventoryData = data;
        panelSwitcher = switcher;

        if (goToMeditationFromInventoryButton != null)
        {
            goToMeditationFromInventoryButton.onClick.RemoveAllListeners();
            goToMeditationFromInventoryButton.onClick.AddListener(OnGoToMeditationClicked);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (inventoryData == null)
        {
            return;
        }

        if (memoryFragmentText != null)
        {
            memoryFragmentText.text = "Memory Fragments: " + inventoryData.memoryFragmentCount;
        }

        if (runeText != null)
        {
            runeText.text = "Runes: " + inventoryData.runeCount;
        }
    }

    private void OnGoToMeditationClicked()
    {
        if (panelSwitcher == null)
        {
            Debug.LogWarning("InventoryUI: panelSwitcher is null.");
            return;
        }

        panelSwitcher.ShowMeditationPanel();
    }
}