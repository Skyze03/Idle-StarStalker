using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MeditationUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button meditateButton;
    [SerializeField] private Button autoMeditateButton;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI autoButtonText;

    private MeditationSystem meditationSystem;
    private PlayerData playerData;
    private MeditationState meditationState;

    public void Setup(MeditationSystem system, PlayerData data, MeditationState state)
    {
        meditationSystem = system;
        playerData = data;
        meditationState = state;

        if (meditateButton != null)
        {
            meditateButton.onClick.RemoveAllListeners();
            meditateButton.onClick.AddListener(OnMeditateButtonClicked);
        }

        if (autoMeditateButton != null)
        {
            autoMeditateButton.onClick.RemoveAllListeners();
            autoMeditateButton.onClick.AddListener(OnAutoMeditateButtonClicked);
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
            levelText.text = "Level: " + playerData.level;
        }

        if (expText != null)
        {
            expText.text = "EXP: " + playerData.exp + " / " + playerData.GetRequiredExp();
        }

        if (expSlider != null)
        {
            if (playerData.GetRequiredExp() > 0)
            {
                expSlider.value = (float)playerData.exp / playerData.GetRequiredExp();
            }
            else
            {
                expSlider.value = 0f;
            }
        }

        RefreshAutoMeditationUI();
    }

    private void RefreshAutoMeditationUI()
    {
        if (meditationState == null)
        {
            return;
        }

        if (autoMeditateButton != null)
        {
            autoMeditateButton.interactable = meditationState.autoMeditationUnlocked;
        }

        if (autoButtonText != null)
        {
            if (!meditationState.autoMeditationUnlocked)
            {
                autoButtonText.text = "Auto Locked";
            }
            else if (meditationState.autoMeditationEnabled)
            {
                autoButtonText.text = "Auto ON";
            }
            else
            {
                autoButtonText.text = "Auto OFF";
            }
        }
    }

    private void OnMeditateButtonClicked()
    {
        if (meditationSystem == null)
        {
            Debug.LogWarning("MeditationUI: meditationSystem is null.");
            return;
        }

        meditationSystem.StartMeditation();
        Refresh();
    }

    private void OnAutoMeditateButtonClicked()
    {
        if (meditationSystem == null)
        {
            Debug.LogWarning("MeditationUI: meditationSystem is null.");
            return;
        }

        meditationSystem.ToggleAutoMeditation();
        Refresh();
    }
}