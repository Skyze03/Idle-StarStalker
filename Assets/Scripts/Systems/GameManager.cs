using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Core Data")]
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
    [SerializeField] private CombinerSystem combinerSystem;
    [SerializeField] private CombinerUI combinerUI;
    [SerializeField] private StatsUI statsUI;
    [SerializeField] private PanelSwitcher panelSwitcher;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private SaveLoadUI saveLoadUI;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private BattleUI battleUI;
    private MeditationState meditationState;
    private InventoryData inventoryData;
    private BuffData buffData;

    private BattleState battleState;
    private EnemyData enemyData;
    public PlayerData PlayerDataRef => playerData;
    public MeditationState MeditationStateRef => meditationState;
    public InventoryData InventoryDataRef => inventoryData;
    public BuffData BuffDataRef => buffData;

    public BattleState BattleStateRef => battleState;
    public EnemyData EnemyDataRef => enemyData;

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
        buffData = new BuffData();

        battleState = new BattleState();

        enemyData = new EnemyData(
            "Training Shade",
            60,
            8,
            2,
            4,
            20
        );

        if (inventorySystem != null)
        {
            inventorySystem.Setup(inventoryData);
        }

        if (combinerSystem != null)
        {
            combinerSystem.Setup(inventoryData, buffData);
        }

        if (meditationSystem != null)
        {
            meditationSystem.Setup(playerData, meditationState, inventorySystem, buffData);
        }

        if (collectionSystem != null)
        {
            collectionSystem.Setup(playerData, inventorySystem, buffData);
        }

        if (upgradeSystem != null)
        {
            upgradeSystem.Setup(playerData);
        }

        if (battleSystem != null)
        {
            battleSystem.Setup(
                playerData,
                battleState,
                enemyData
            );
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

        if (combinerUI != null)
        {
            combinerUI.Setup(combinerSystem, buffData, panelSwitcher);
        }

        if (statsUI != null)
        {
            statsUI.Setup(playerData, panelSwitcher);
        }

        if (battleUI != null)
        {
            battleUI.Setup(
                battleSystem,
                playerData,
                battleState,
                enemyData,
                panelSwitcher
            );
        }

        if (saveLoadUI != null && saveSystem != null)
        {
            saveLoadUI.Setup(saveSystem, this);
        }

        Debug.Log("Game initialized.");
    }

    private void Update()
    {
        if (meditationSystem != null)
        {
            meditationSystem.Tick(Time.deltaTime);
        }

        if (battleSystem != null)
        {
            battleSystem.Tick(Time.deltaTime);
        }

        RefreshAllUI();
    }

    public void RefreshAllUI()
    {
        if (meditationUI != null) meditationUI.Refresh();
        if (collectionUI != null) collectionUI.Refresh();
        if (upgradeUI != null) upgradeUI.Refresh();
        if (inventoryUI != null) inventoryUI.Refresh();
        if (combinerUI != null) combinerUI.Refresh();
        if (statsUI != null) statsUI.Refresh();
        if (battleUI != null) battleUI.Refresh();
    }
}