using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    private SaveSystem saveSystem;
    private GameManager gameManager;

    public void Setup(SaveSystem system, GameManager manager)
    {
        saveSystem = system;
        gameManager = manager;

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

    private void OnSaveClicked()
    {
        if (saveSystem == null || gameManager == null)
        {
            Debug.LogWarning("SaveLoadUI: saveSystem or gameManager is null.");
            return;
        }

        saveSystem.SaveGame(
            gameManager.PlayerDataRef,
            gameManager.MeditationStateRef,
            gameManager.InventoryDataRef,
            gameManager.BuffDataRef
        );
    }

    private void OnLoadClicked()
    {
        if (saveSystem == null || gameManager == null)
        {
            Debug.LogWarning("SaveLoadUI: saveSystem or gameManager is null.");
            return;
        }

        bool loaded = saveSystem.LoadGame(
            gameManager.PlayerDataRef,
            gameManager.MeditationStateRef,
            gameManager.InventoryDataRef,
            gameManager.BuffDataRef
        );

        if (loaded)
        {
            gameManager.RefreshAllUI();
        }
    }
}