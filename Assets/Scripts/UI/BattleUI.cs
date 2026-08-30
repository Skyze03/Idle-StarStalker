using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerHPText;
    [SerializeField] private Slider playerHPSlider;
    [SerializeField] private Slider playerActionSlider;
    [SerializeField] private Slider playerRageSlider;

    [Header("Enemy UI")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private TMP_Text enemyHPText;
    [SerializeField] private Slider enemyHPSlider;
    [SerializeField] private Slider enemyActionSlider;
    [SerializeField] private Slider enemyRageSlider;

    [Header("Battle UI")]
    [SerializeField] private TMP_Text battleStatusText;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private Button returnButton;

    [Header("Main Stage UI")]
    [SerializeField] private TMP_Text selectedStageText;
    [SerializeField] private TMP_Text stageProgressText;
    [SerializeField] private TMP_Text battleStaminaText;
    [SerializeField] private Button previousStageButton;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private Button sweepButton;
    [SerializeField] private TMP_Text enemyStatsText;
    [SerializeField] private TMP_Text rewardPreviewText;
    [SerializeField] private TMP_Text stageFeedbackText;
    [SerializeField] private Button buildSummaryButton;
    [SerializeField] private GameObject buildSummaryPanel;
    [SerializeField] private TMP_Text buildSummaryText;
    [SerializeField] private Button closeBuildSummaryButton;

    private BattleSystem battleSystem;
    private PlayerData playerData;
    private BattleState battleState;
    private EnemyData enemyData;
    private PanelSwitcher panelSwitcher;
    private MainStageSystem mainStageSystem;

    public void Setup(
        BattleSystem battleSystem,
        PlayerData playerData,
        BattleState battleState,
        EnemyData enemyData,
        PanelSwitcher panelSwitcher,
        MainStageSystem mainStageSystem)
    {
        this.battleSystem = battleSystem;
        this.playerData = playerData;
        this.battleState = battleState;
        this.enemyData = enemyData;
        this.panelSwitcher = panelSwitcher;
        this.mainStageSystem = mainStageSystem;

        if (startBattleButton != null)
        {
            startBattleButton.onClick.RemoveAllListeners();
            startBattleButton.onClick.AddListener(OnStartBattleClicked);
        }

        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(OnReturnClicked);
        }

        if (previousStageButton != null)
        {
            previousStageButton.onClick.RemoveAllListeners();
            previousStageButton.onClick.AddListener(OnPreviousStageClicked);
        }

        if (nextStageButton != null)
        {
            nextStageButton.onClick.RemoveAllListeners();
            nextStageButton.onClick.AddListener(OnNextStageClicked);
        }

        if (sweepButton != null)
        {
            sweepButton.onClick.RemoveAllListeners();
            sweepButton.onClick.AddListener(OnSweepClicked);
        }

        if (buildSummaryButton != null)
        {
            buildSummaryButton.onClick.RemoveAllListeners();
            buildSummaryButton.onClick.AddListener(OnBuildSummaryClicked);
        }
        if (closeBuildSummaryButton != null)
        {
            closeBuildSummaryButton.onClick.RemoveAllListeners();
            closeBuildSummaryButton.onClick.AddListener(OnCloseBuildSummaryClicked);
        }
        if (buildSummaryPanel != null) buildSummaryPanel.SetActive(false);

        Refresh();
    }

    public void Refresh()
    {
        if (playerData == null || battleState == null || enemyData == null)
        {
            return;
        }

        if (playerNameText != null)
        {
            UltimateData displayedUltimate = battleState.battleRunning &&
                battleSystem.PlayerBuild != null
                    ? battleSystem.PlayerBuild.ultimate
                    : playerData.equippedUltimate;
            string playerUltimate = displayedUltimate != null
                ? displayedUltimate.ultimateName
                : "None";
            playerNameText.text = $"Player — ULT {playerUltimate}";
        }

        if (enemyNameText != null)
        {
            enemyNameText.text = enemyData.enemyName;
        }

        if (playerHPText != null)
        {
            int playerMaxHP = battleState.battleRunning && battleSystem.PlayerBuild != null
                ? battleSystem.PlayerBuild.maxHP
                : playerData.stats.hp;
            playerHPText.text =
                $"HP: {battleState.playerCurrentHP} / {playerMaxHP}";
        }

        if (enemyHPText != null)
        {
            int enemyMaxHP = battleState.battleRunning && battleSystem.EnemyBuild != null
                ? battleSystem.EnemyBuild.maxHP
                : enemyData.maxHP;
            enemyHPText.text =
                $"HP: {battleState.enemyCurrentHP} / {enemyMaxHP}";
        }

        if (playerHPSlider != null)
        {
            playerHPSlider.maxValue = battleState.battleRunning &&
                battleSystem.PlayerBuild != null
                    ? battleSystem.PlayerBuild.maxHP
                    : playerData.stats.hp;
            playerHPSlider.value = battleState.playerCurrentHP;
        }

        if (enemyHPSlider != null)
        {
            enemyHPSlider.maxValue = battleState.battleRunning &&
                battleSystem.EnemyBuild != null
                    ? battleSystem.EnemyBuild.maxHP
                    : enemyData.maxHP;
            enemyHPSlider.value = battleState.enemyCurrentHP;
        }

        if (playerActionSlider != null)
        {
            playerActionSlider.minValue = 0f;
            playerActionSlider.maxValue = BattleSystem.ActionThreshold;
            playerActionSlider.value = battleState.playerActionValue;
        }

        if (enemyActionSlider != null)
        {
            enemyActionSlider.minValue = 0f;
            enemyActionSlider.maxValue = BattleSystem.ActionThreshold;
            enemyActionSlider.value = battleState.enemyActionValue;
        }

        if (playerRageSlider != null)
        {
            playerRageSlider.minValue = 0f;
            playerRageSlider.maxValue = BattleSystem.RageThreshold;
            playerRageSlider.value = battleState.playerRage;
        }

        if (enemyRageSlider != null)
        {
            enemyRageSlider.minValue = 0f;
            enemyRageSlider.maxValue = BattleSystem.RageThreshold;
            enemyRageSlider.value = battleState.enemyRage;
        }

        if (battleStatusText != null)
        {
            if (battleState.battleRunning)
            {
                battleStatusText.text = "Running";
            }
            else
            {
                switch (battleState.battleResult)
                {
                    case BattleResult.Victory:
                        battleStatusText.text = "Victory";
                        break;

                    case BattleResult.Defeat:
                        battleStatusText.text = "Defeat";
                        break;

                    default:
                        battleStatusText.text = "Ready";
                        break;
                }
            }
        }

        if (startBattleButton != null)
        {
            startBattleButton.interactable = !battleState.battleRunning;
        }

        if (returnButton != null)
        {
            returnButton.interactable = !battleState.battleRunning;
        }

        if (buildSummaryButton != null)
            buildSummaryButton.interactable = !battleState.battleRunning;

        if (battleState.battleRunning && buildSummaryPanel != null)
            buildSummaryPanel.SetActive(false);

        RefreshMainStageUI();
    }

    private void RefreshMainStageUI()
    {
        if (mainStageSystem == null || mainStageSystem.State == null)
        {
            return;
        }

        MainStageState state = mainStageSystem.State;

        if (selectedStageText != null)
            selectedStageText.text = state.selectedStage == MainStageSystem.TotalStages
                ? $"Stage {state.selectedStage} — FINAL"
                : $"Stage {state.selectedStage}";

        if (stageProgressText != null)
        {
            stageProgressText.text = mainStageSystem.IsPrototypeComplete
                ? "MAIN STORY COMPLETE"
                : $"Cleared: {state.highestClearedStage} / {MainStageSystem.TotalStages}";
        }

        if (battleStaminaText != null)
        {
            battleStaminaText.text = state.battleStamina >= MainStageSystem.MaxStamina
                ? $"Stamina: {state.battleStamina}/{MainStageSystem.MaxStamina} (Full)"
                : $"Stamina: {state.battleStamina}/{MainStageSystem.MaxStamina} " +
                  $"(+1 in {mainStageSystem.SecondsUntilNextStamina}s)";
        }

        if (startBattleButton != null)
            startBattleButton.interactable = mainStageSystem.CanStartSelectedStage();

        if (previousStageButton != null)
        {
            previousStageButton.interactable =
                !battleState.battleRunning && state.selectedStage > 1;
        }

        if (nextStageButton != null)
        {
            nextStageButton.interactable =
                !battleState.battleRunning &&
                state.selectedStage < state.highestUnlockedStage;
        }

        if (sweepButton != null)
            sweepButton.interactable = mainStageSystem.CanSweepSelectedStage();

        if (enemyStatsText != null)
        {
            string ultimateName = enemyData.equippedUltimate != null
                ? enemyData.equippedUltimate.ultimateName
                : "None";
            enemyStatsText.text =
                $"ATK {enemyData.attack}  DEF {enemyData.defense}  " +
                $"AGI {enemyData.agility}  WIS {enemyData.wisdom}  ULT {ultimateName}\n" +
                $"Trait: {EnemyTraitUtility.GetDescription(enemyData.traits)}";
        }

        if (rewardPreviewText != null)
        {
            RewardBundle normal = mainStageSystem.GetNormalReward(state.selectedStage);
            rewardPreviewText.text = $"Reward: {FormatReward(normal)}";

            if (!mainStageSystem.IsSelectedStageCleared)
            {
                RewardBundle firstClear =
                    mainStageSystem.GetFirstClearReward(state.selectedStage);
                rewardPreviewText.text += $"  First: {FormatReward(firstClear)}";

                string unlockId =
                    mainStageSystem.GetUltimateUnlockForStage(state.selectedStage);
                if (!string.IsNullOrEmpty(unlockId))
                {
                    rewardPreviewText.text +=
                        $" + Unlock {UltimateData.GetById(unlockId).ultimateName}";
                }

                string equipmentId =
                    mainStageSystem.GetEquipmentUnlockForStage(state.selectedStage);
                EquipmentData equipmentDrop = EquipmentData.GetById(equipmentId);
                if (equipmentDrop != null)
                    rewardPreviewText.text += $" + Equipment {equipmentDrop.itemName}";
            }
            else if (mainStageSystem.IsPrototypeComplete &&
                state.selectedStage == MainStageSystem.TotalStages)
            {
                rewardPreviewText.text += "  Prototype finale cleared — replay available";
            }
        }

        if (stageFeedbackText != null)
            stageFeedbackText.text = battleState.battleRunning
                ? battleSystem.CombatLogText
                : mainStageSystem.LastFeedback;
    }

    private void OnStartBattleClicked()
    {
        if (battleSystem == null)
        {
            return;
        }

        if (mainStageSystem != null)
        {
            mainStageSystem.TryStartSelectedStage();
        }
        else
        {
            battleSystem.StartBattle();
        }
        if (buildSummaryPanel != null) buildSummaryPanel.SetActive(false);
        Refresh();
    }

    private void OnReturnClicked()
    {
        if (panelSwitcher == null ||
            (battleState != null && battleState.battleRunning))
        {
            return;
        }

        panelSwitcher.ShowMeditationPanel();
    }

    private void OnPreviousStageClicked()
    {
        if (mainStageSystem == null) return;

        mainStageSystem.SelectStage(mainStageSystem.State.selectedStage - 1);
        Refresh();
    }

    private void OnNextStageClicked()
    {
        if (mainStageSystem == null) return;

        mainStageSystem.SelectStage(mainStageSystem.State.selectedStage + 1);
        Refresh();
    }

    private void OnSweepClicked()
    {
        if (mainStageSystem == null) return;

        mainStageSystem.TrySweepStage(mainStageSystem.State.selectedStage);
        Refresh();
    }

    private void OnBuildSummaryClicked()
    {
        if (battleState == null || battleState.battleRunning ||
            buildSummaryPanel == null) return;
        RefreshBuildSummary();
        buildSummaryPanel.SetActive(true);
        buildSummaryPanel.transform.SetAsLastSibling();
    }

    private void OnCloseBuildSummaryClicked()
    {
        if (buildSummaryPanel != null) buildSummaryPanel.SetActive(false);
    }

    private void RefreshBuildSummary()
    {
        if (buildSummaryText == null || battleSystem == null) return;
        CombatBuildSnapshot build = battleSystem.CreatePlayerBuildPreview();
        if (build == null) return;

        string equipmentLines = string.Empty;
        EquipmentSystem equipment = battleSystem.EquipmentSystem;
        foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
        {
            EquipmentData item = equipment?.GetEquipped(slot);
            equipmentLines += item == null
                ? $"{slot}: None\n"
                : $"{slot}: {item.itemName} — {item.description}\n";
        }

        buildSummaryText.text =
            $"ULTIMATE\n{build.ultimate.ultimateName}: {build.ultimate.description}\n\n" +
            $"FINAL COMBAT STATS\nHP {build.maxHP}   ATK {build.attack}   " +
            $"DEF {build.defense}\nAGI {build.agility}   WIS {build.wisdom}\n" +
            $"Rage/Attack +{build.bonusRageOnAttack:0}   " +
            $"Rage/Hit +{build.bonusRageOnHit:0}\n\n" +
            "EQUIPMENT\n" + equipmentLines;
    }

    private string FormatReward(RewardBundle reward)
    {
        if (reward == null) return "None";

        string result = $"{reward.energy} Energy";
        if (reward.memoryFragments > 0)
            result += $", {reward.memoryFragments} Fragments";
        if (reward.runes > 0)
            result += $", {reward.runes} Runes";
        return result;
    }
}
