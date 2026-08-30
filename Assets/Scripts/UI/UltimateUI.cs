using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UltimateUI : MonoBehaviour
{
    [SerializeField] private TMP_Text equippedUltimateText;
    [SerializeField] private Button starBurstButton;
    [SerializeField] private Button ironRetaliationButton;
    [SerializeField] private Button rapidNovaButton;
    [SerializeField] private Button meteorFlurryButton;
    [SerializeField] private Button swiftAscensionButton;
    [SerializeField] private Button returnButton;

    private UltimateSystem ultimateSystem;
    private PlayerData playerData;
    private PanelSwitcher panelSwitcher;

    public void Setup(UltimateSystem system, PlayerData data, PanelSwitcher switcher)
    {
        ultimateSystem = system;
        playerData = data;
        panelSwitcher = switcher;

        Bind(starBurstButton, "star_burst");
        Bind(ironRetaliationButton, "iron_retaliation");
        Bind(rapidNovaButton, "rapid_nova");
        Bind(meteorFlurryButton, "meteor_flurry");
        Bind(swiftAscensionButton, "swift_ascension");

        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() => panelSwitcher?.ShowMeditationPanel());
        }

        Refresh();
    }

    public void Refresh()
    {
        if (ultimateSystem == null || playerData == null) return;

        UltimateData equipped = playerData.equippedUltimate;
        if (equippedUltimateText != null)
            equippedUltimateText.text =
                $"Equipped: {equipped.ultimateName}\n{equipped.description}";

        RefreshButton(starBurstButton, "star_burst");
        RefreshButton(ironRetaliationButton, "iron_retaliation");
        RefreshButton(rapidNovaButton, "rapid_nova");
        RefreshButton(meteorFlurryButton, "meteor_flurry");
        RefreshButton(swiftAscensionButton, "swift_ascension");
    }

    private void Bind(Button button, string ultimateId)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            ultimateSystem.Equip(ultimateId);
            Refresh();
        });
    }

    private void RefreshButton(Button button, string ultimateId)
    {
        if (button == null) return;
        UltimateData ultimate = UltimateData.GetById(ultimateId);
        bool unlocked = ultimateSystem.IsUnlocked(ultimateId);
        bool equipped = playerData.equippedUltimateId == ultimateId;
        button.interactable = unlocked && !equipped;

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            string state = equipped ? "EQUIPPED" : unlocked ? "Tap to equip" : "LOCKED";
            label.text = $"{ultimate.ultimateName} — {state}\n{ultimate.description}";
        }
    }
}
