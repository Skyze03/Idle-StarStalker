using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerHPText;
    [SerializeField] private Slider playerHPSlider;

    [Header("Enemy UI")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private TMP_Text enemyHPText;
    [SerializeField] private Slider enemyHPSlider;

    [Header("Battle UI")]
    [SerializeField] private TMP_Text battleStatusText;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private Button returnButton;

    private BattleSystem battleSystem;
    private PlayerData playerData;
    private BattleState battleState;
    private EnemyData enemyData;
    private PanelSwitcher panelSwitcher;

    public void Setup(
        BattleSystem battleSystem,
        PlayerData playerData,
        BattleState battleState,
        EnemyData enemyData,
        PanelSwitcher panelSwitcher)
    {
        this.battleSystem = battleSystem;
        this.playerData = playerData;
        this.battleState = battleState;
        this.enemyData = enemyData;
        this.panelSwitcher = panelSwitcher;

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
    }

    private void OnStartBattleClicked()
    {
        if (battleSystem == null)
        {
            return;
        }

        battleSystem.StartBattle();
        Refresh();
    }

    private void OnReturnClicked()
    {
        if (panelSwitcher == null)
        {
            return;
        }

        panelSwitcher.ShowMeditationPanel();
    }
}