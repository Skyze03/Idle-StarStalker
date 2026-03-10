using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombinerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currentBuffText;
    [SerializeField] private Button createMeditationBuffButton;
    [SerializeField] private Button createCollectionBuffButton;
    [SerializeField] private Button goToMeditationFromCombinerButton;

    private CombinerSystem combinerSystem;
    private BuffData buffData;
    private PanelSwitcher panelSwitcher;

    public void Setup(CombinerSystem system, BuffData buff, PanelSwitcher switcher)
    {
        combinerSystem = system;
        buffData = buff;
        panelSwitcher = switcher;

        if (createMeditationBuffButton != null)
        {
            createMeditationBuffButton.onClick.RemoveAllListeners();
            createMeditationBuffButton.onClick.AddListener(OnCreateMeditationBuffClicked);
        }

        if (createCollectionBuffButton != null)
        {
            createCollectionBuffButton.onClick.RemoveAllListeners();
            createCollectionBuffButton.onClick.AddListener(OnCreateCollectionBuffClicked);
        }

        if (goToMeditationFromCombinerButton != null)
        {
            goToMeditationFromCombinerButton.onClick.RemoveAllListeners();
            goToMeditationFromCombinerButton.onClick.AddListener(OnGoToMeditationClicked);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (buffData == null)
        {
            return;
        }

        if (currentBuffText != null)
        {
            currentBuffText.text = buffData.GetCurrentBuffDescription();
        }
    }

    private void OnCreateMeditationBuffClicked()
    {
        if (combinerSystem == null)
        {
            Debug.LogWarning("CombinerUI: combinerSystem is null.");
            return;
        }

        combinerSystem.CreateMeditationBuff();
        Refresh();
    }

    private void OnCreateCollectionBuffClicked()
    {
        if (combinerSystem == null)
        {
            Debug.LogWarning("CombinerUI: combinerSystem is null.");
            return;
        }

        combinerSystem.CreateCollectionBuff();
        Refresh();
    }

    private void OnGoToMeditationClicked()
    {
        if (panelSwitcher == null)
        {
            Debug.LogWarning("CombinerUI: panelSwitcher is null.");
            return;
        }

        panelSwitcher.ShowMeditationPanel();
    }
}