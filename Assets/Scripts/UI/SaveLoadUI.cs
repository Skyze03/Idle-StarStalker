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

    public void Setup(SaveSystem system, GameManager manager, BattleState state)
    {
        saveSystem = system;
        gameManager = manager;
        battleState = state;

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

    public void Refresh()
    {
        bool locked = battleState != null && battleState.battleRunning;
        if (saveButton != null) saveButton.interactable = !locked;
        if (loadButton != null) loadButton.interactable = !locked;
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
            gameManager.MainStageStateRef
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
            gameManager.MainStageStateRef
        );

        if (loaded)
        {
            gameManager.HandleGameLoaded();
            gameManager.RefreshAllUI();
        }
    }
}
