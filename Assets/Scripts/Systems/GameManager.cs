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
    [SerializeField] private MainStageSystem mainStageSystem;
    [SerializeField] private UltimateSystem ultimateSystem;
    [SerializeField] private UltimateUI ultimateUI;
    [SerializeField] private EquipmentSystem equipmentSystem;
    [SerializeField] private EquipmentUI equipmentUI;
    [SerializeField] private GameFeedbackUI gameFeedbackUI;
    [SerializeField] private EditBuildUI editBuildUI;
    [SerializeField] private DailyChallengeSystem dailyChallengeSystem;
    [SerializeField] private DailyChallengeUI dailyChallengeUI;
    [SerializeField] private BattleModeUI battleModeUI;
    [SerializeField] private RewardPipeline rewardPipeline;
    private MeditationState meditationState;
    private InventoryData inventoryData;
    private BuffData buffData;

    private BattleState battleState;
    private EnemyData enemyData;
    private MainStageState mainStageState;
    private DailyChallengeState dailyChallengeState;
    public PlayerData PlayerDataRef => playerData;
    public MeditationState MeditationStateRef => meditationState;
    public InventoryData InventoryDataRef => inventoryData;
    public BuffData BuffDataRef => buffData;

    public BattleState BattleStateRef => battleState;
    public EnemyData EnemyDataRef => enemyData;
    public MainStageState MainStageStateRef => mainStageState;
    public MainStageSystem MainStageSystemRef => mainStageSystem;
    public UltimateSystem UltimateSystemRef => ultimateSystem;
    public EquipmentSystem EquipmentSystemRef => equipmentSystem;
    public DailyChallengeState DailyChallengeStateRef => dailyChallengeState;
    public DailyChallengeSystem DailyChallengeSystemRef => dailyChallengeSystem;

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
        mainStageState = new MainStageState();
        dailyChallengeState = new DailyChallengeState();

        enemyData = new EnemyData(
            "Training Shade",
            60,
            8,
            2,
            4,
            5,
            UltimateData.CreateStarBurst()
        );

        if (ultimateSystem == null)
            ultimateSystem = gameObject.AddComponent<UltimateSystem>();
        ultimateSystem.Setup(playerData, battleState);
        if (equipmentSystem == null)
            equipmentSystem = gameObject.AddComponent<EquipmentSystem>();

        if (inventorySystem != null)
        {
            inventorySystem.Setup(inventoryData);
        }
        equipmentSystem.Setup(playerData, battleState, inventorySystem);
        if (rewardPipeline == null) rewardPipeline = gameObject.AddComponent<RewardPipeline>();
        rewardPipeline.Setup(playerData, inventorySystem, equipmentSystem, ultimateSystem);

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
                enemyData,
                equipmentSystem
            );
        }

        if (mainStageSystem == null)
        {
            mainStageSystem = gameObject.AddComponent<MainStageSystem>();
        }

        if (battleSystem != null && mainStageSystem != null)
        {
            mainStageSystem.Setup(
                playerData,
                inventorySystem,
                battleSystem,
                battleState,
                enemyData,
                mainStageState,
                ultimateSystem,
                equipmentSystem,
                rewardPipeline
            );
        }
        if (panelSwitcher != null)
        {
            panelSwitcher.Setup(battleState);
        }
        if (dailyChallengeSystem == null)
            dailyChallengeSystem = gameObject.AddComponent<DailyChallengeSystem>();
        dailyChallengeSystem.Setup(dailyChallengeState, playerData, inventorySystem,
            battleSystem, battleState, enemyData, panelSwitcher, rewardPipeline);

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
                panelSwitcher,
                mainStageSystem
            );
        }

        if (ultimateUI != null)
            ultimateUI.Setup(ultimateSystem, playerData, panelSwitcher);
        if (equipmentUI != null)
            equipmentUI.Setup(equipmentSystem, panelSwitcher);
        if (editBuildUI != null)
            editBuildUI.Setup(equipmentSystem, ultimateSystem, playerData, battleSystem, panelSwitcher);
        if (gameFeedbackUI != null)
            gameFeedbackUI.Setup(
                meditationSystem,
                collectionSystem,
                mainStageSystem,
                upgradeSystem,
                combinerSystem,
                equipmentSystem,
                ultimateSystem
            );
        if (gameFeedbackUI != null)
        {
            dailyChallengeSystem.ResultRequested -= gameFeedbackUI.ShowResult;
            dailyChallengeSystem.ResultRequested += gameFeedbackUI.ShowResult;
        }
        if (battleModeUI != null) battleModeUI.Setup(panelSwitcher, mainStageSystem);
        if (dailyChallengeUI != null)
            dailyChallengeUI.Setup(dailyChallengeSystem, enemyData, panelSwitcher);

        if (saveLoadUI != null && saveSystem != null)
        {
            saveLoadUI.Setup(
                saveSystem,
                this,
                battleState,
                battleSystem,
                battleUI != null ? battleUI.gameObject : null
            );
        }

        Debug.Log("Game initialized.");
    }

    private void Update()
    {
        if (meditationSystem != null &&
            (battleState == null || !battleState.battleRunning))
        {
            meditationSystem.Tick(Time.deltaTime);
        }
        if (battleSystem != null)
        {
            battleSystem.Tick(Time.deltaTime);
        }

        if (mainStageSystem != null)
        {
            mainStageSystem.Tick();
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
        if (panelSwitcher != null) panelSwitcher.Refresh();
        if (battleUI != null) battleUI.Refresh();
        if (ultimateUI != null) ultimateUI.Refresh();
        if (equipmentUI != null) equipmentUI.Refresh();
        if (editBuildUI != null) editBuildUI.Refresh();
        if (dailyChallengeUI != null) dailyChallengeUI.Refresh();
        if (saveLoadUI != null) saveLoadUI.Refresh();
    }

    public void HandleGameLoaded()
    {
        if (mainStageSystem != null)
        {
            mainStageSystem.RefreshAfterLoad();
        }
        if (ultimateSystem != null)
        {
            ultimateSystem.NormalizePlayerUltimates();
        }
        if (equipmentSystem != null)
        {
            equipmentSystem.Normalize(mainStageState?.highestClearedStage ?? 0);
        }
    }
}
