using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    private SaveSystem saveSystem;
    private GameManager gameManager;
    private BattleState battleState;
    private BattleSystem battleSystem;
    private GameObject battlePanel;

    public void Setup(
        SaveSystem system,
        GameManager manager,
        BattleState state,
        BattleSystem battle,
        GameObject battlePanelObject)
    {
        saveSystem = system;
        gameManager = manager;
        battleState = state;
        battleSystem = battle;
        battlePanel = battlePanelObject;

        if (battleSystem != null)
        {
            battleSystem.BattleStarted -= OnBattleStarted;
            battleSystem.BattleStarted += OnBattleStarted;
            battleSystem.BattleEnded -= OnBattleEnded;
            battleSystem.BattleEnded += OnBattleEnded;
        }

        if (saveButton != null)
        {
            saveButton.onClick.RemoveAllListeners();
            saveButton.onClick.AddListener(OnSaveClicked);
        }

        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(OnLoadClicked);
        }
    }

    private void OnDestroy()
    {
        if (battleSystem == null) return;
        battleSystem.BattleStarted -= OnBattleStarted;
        battleSystem.BattleEnded -= OnBattleEnded;
    }

    private void OnBattleStarted()
    {
        Refresh();
    }

    private void OnBattleEnded(BattleResult result)
    {
        Refresh();
    }

    public void Refresh()
    {
        bool battlePageVisible = battlePanel != null && battlePanel.activeInHierarchy;
        bool locked = battlePageVisible ||
            (battleState != null && battleState.battleRunning);
        if (saveButton != null) saveButton.gameObject.SetActive(!locked);
        if (loadButton != null) loadButton.gameObject.SetActive(!locked);
    }

    private void OnSaveClicked()
    {
        if (battleState != null && battleState.battleRunning) return;
        if (saveSystem == null || gameManager == null)
        {
            Debug.LogWarning("SaveLoadUI: saveSystem or gameManager is null.");
            return;
        }

        saveSystem.SaveGame(
            gameManager.PlayerDataRef,
            gameManager.MeditationStateRef,
            gameManager.InventoryDataRef,
            gameManager.BuffDataRef,
            gameManager.MainStageStateRef,
            gameManager.DailyChallengeStateRef
        );
    }

    private void OnLoadClicked()
    {
        if (battleState != null && battleState.battleRunning) return;
        if (saveSystem == null || gameManager == null)
        {
            Debug.LogWarning("SaveLoadUI: saveSystem or gameManager is null.");
            return;
        }

        bool loaded = saveSystem.LoadGame(
            gameManager.PlayerDataRef,
            gameManager.MeditationStateRef,
            gameManager.InventoryDataRef,
            gameManager.BuffDataRef,
            gameManager.MainStageStateRef,
            gameManager.DailyChallengeStateRef
        );

        if (loaded)
        {
            gameManager.HandleGameLoaded();
            gameManager.RefreshAllUI();
        }
    }
}
