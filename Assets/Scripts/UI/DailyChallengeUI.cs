using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyChallengeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text attemptsText;
    [SerializeField] private TMP_Text enemyText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button returnButton;
    private DailyChallengeSystem system;
    private EnemyData enemy;
    private PanelSwitcher panels;

    public void Setup(DailyChallengeSystem daily, EnemyData enemyData, PanelSwitcher switcher)
    {
        system = daily; enemy = enemyData; panels = switcher;
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => { if (system.TryStart()) panels.ShowBattlePanel(); });
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() => panels.ShowBattleModePanel());
        Refresh();
    }

    public void Refresh()
    {
        if (system == null) return;
        system.RefreshDailyReset();
        if (gameObject.activeInHierarchy) system.ConfigureTodayEnemy();
        if (dateText != null) dateText.text = $"UTC Daily — {system.State.utcDateKey}";
        if (attemptsText != null) attemptsText.text =
            $"Attempts: {system.State.remainingAttempts}/{DailyChallengeSystem.MaxDailyAttempts}";
        if (enemyText != null && enemy != null) enemyText.text =
            $"{enemy.enemyName}\nHP {enemy.maxHP}  ATK {enemy.attack}  DEF {enemy.defense}\n" +
            $"AGI {enemy.agility}  WIS {enemy.wisdom}  ULT {enemy.equippedUltimate.ultimateName}";
        if (rewardText != null) rewardText.text =
            $"Victory Reward\n{DailyChallengeSystem.RewardEnergy} Energy + {DailyChallengeSystem.RewardRunes} Rune";
        if (startButton != null) startButton.interactable = system.CanStart;
    }
}
