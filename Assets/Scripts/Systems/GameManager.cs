using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerData playerData;

    [Header("Scene References")]
    [SerializeField] private MeditationSystem meditationSystem;
    [SerializeField] private MeditationUI meditationUI;
    [SerializeField] private CollectionSystem collectionSystem;
    [SerializeField] private CollectionUI collectionUI;
    [SerializeField] private UpgradeSystem upgradeSystem;
    [SerializeField] private UpgradeUI upgradeUI;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private PanelSwitcher panelSwitcher;

    private MeditationState meditationState;
    private InventoryData inventoryData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        playerData = new PlayerData();
        playerData.CalculateStats();

        meditationState = new MeditationState();
        inventoryData = new InventoryData();

        if (inventorySystem != null)
        {
            inventorySystem.Setup(inventoryData);
        }

        if (meditationSystem != null)
        {
            meditationSystem.Setup(playerData, meditationState, inventorySystem);
        }

        if (collectionSystem != null)
        {
            collectionSystem.Setup(playerData, inventorySystem);
        }

        if (upgradeSystem != null)
        {
            upgradeSystem.Setup(playerData);
        }

        if (panelSwitcher != null)
        {
            panelSwitcher.Setup();
        }

        if (meditationUI != null)
        {
            meditationUI.Setup(meditationSystem, playerData, meditationState);
        }

        if (collectionUI != null)
        {
            collectionUI.Setup(collectionSystem, playerData, panelSwitcher);
        }

        if (upgradeUI != null)
        {
            upgradeUI.Setup(upgradeSystem, playerData, panelSwitcher);
        }

        if (inventoryUI != null)
        {
            inventoryUI.Setup(inventoryData, panelSwitcher);
        }

        Debug.Log("Game initialized.");
    }

    private void Update()
    {
        if (meditationSystem != null)
        {
            meditationSystem.Tick(Time.deltaTime);
        }

        if (meditationUI != null)
        {
            meditationUI.Refresh();
        }

        if (collectionUI != null)
        {
            collectionUI.Refresh();
        }

        if (upgradeUI != null)
        {
            upgradeUI.Refresh();
        }

        if (inventoryUI != null)
        {
            inventoryUI.Refresh();
        }
    }
}