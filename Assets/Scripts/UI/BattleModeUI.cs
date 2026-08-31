using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleModeUI : MonoBehaviour
{
    [SerializeField] private Button mainStoryButton;
    [SerializeField] private Button dailyButton;
    [SerializeField] private Button eliteButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private TMP_Text eliteStatusText;

    public void Setup(PanelSwitcher panels, MainStageSystem mainStages)
    {
        Bind(mainStoryButton, () => { mainStages?.RefreshAfterLoad(); panels?.ShowBattlePanel(); });
        Bind(dailyButton, () => panels?.ShowDailyChallengePanel());
        Bind(returnButton, () => panels?.ShowMeditationPanel());
        if (eliteButton != null) eliteButton.interactable = false;
        if (eliteStatusText != null) eliteStatusText.text = "Elite Challenge — Coming Soon";
    }

    private static void Bind(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
