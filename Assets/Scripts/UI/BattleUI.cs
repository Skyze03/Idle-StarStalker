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
            playerNameText.text = "Player";
        }

        if (enemyNameText != null)
        {
            enemyNameText.text = enemyData.enemyName;
        }

        if (playerHPText != null)
        {
            playerHPText.text =
                $"HP: {battleState.playerCurrentHP} / {playerData.stats.hp}";
        }

        if (enemyHPText != null)
        {
            enemyHPText.text =
                $"HP: {battleState.enemyCurrentHP} / {enemyData.maxHP}";
        }

        if (playerHPSlider != null)
        {
            playerHPSlider.maxValue = playerData.stats.hp;
            playerHPSlider.value = battleState.playerCurrentHP;
        }

        if (enemyHPSlider != null)
        {
            enemyHPSlider.maxValue = enemyData.maxHP;
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
            selectedStageText.text = $"Stage {state.selectedStage}";

        if (stageProgressText != null)
        {
            stageProgressText.text =
                $"Cleared: {state.highestClearedStage} / {MainStageSystem.TotalStages}";
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
}
