using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BattlePrototypeUISetup
{
    private const string TargetScene = "Assets/Scenes/MainScene_Rebuild.unity";
    private const string HudName = "MainStageHUD";

    [MenuItem("Tools/Idle StarStalker/Setup Battle Prototype UI")]
    public static void SetupPrototypeUI()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || scene.path != TargetScene)
        {
            Debug.LogWarning($"Open {TargetScene} before setting up the Battle UI.");
            return;
        }

        BattleUI battleUI = FindComponentInScene<BattleUI>(scene);
        StatsUI statsUI = FindComponentInScene<StatsUI>(scene);

        if (battleUI == null || statsUI == null)
        {
            Debug.LogError("BattleUI or StatsUI was not found in the active scene.");
            return;
        }

        RectTransform battlePanel = battleUI.GetComponent<RectTransform>();
        RectTransform hud = FindChildByName(battlePanel, HudName);

        if (hud == null)
        {
            hud = CreateRect(HudName, battlePanel);
            StretchToParent(hud);
        }

        SerializedObject battleSerialized = new SerializedObject(battleUI);

        SetupUltimateUI(scene);
        SetupEquipmentUI(scene);
        SetupBuildSummary(battlePanel, battleSerialized);

        TMP_Text selectedStageText = GetOrCreateText(
            hud, "SelectedStageText", "Stage 1", new Vector2(0f, 210f),
            new Vector2(180f, 36f), 26f
        );
        TMP_Text stageProgressText = GetOrCreateText(
            hud, "StageProgressText", "Cleared: 0 / 20", new Vector2(-145f, 178f),
            new Vector2(220f, 30f), 18f
        );
        TMP_Text staminaText = GetOrCreateText(
            hud, "BattleStaminaText", "Stamina: 20/20 (Full)", new Vector2(145f, 178f),
            new Vector2(260f, 30f), 18f
        );

        Button previousButton = GetOrCreateButton(
            hud, "PreviousStageButton", "<", new Vector2(-125f, 210f),
            new Vector2(46f, 34f)
        );
        Button nextButton = GetOrCreateButton(
            hud, "NextStageButton", ">", new Vector2(125f, 210f),
            new Vector2(46f, 34f)
        );

        Slider playerAction = GetOrCreateSlider(
            hud, "PlayerActionSlider", new Vector2(-105f, -52f),
            new Color(0.2f, 0.75f, 1f, 1f)
        );
        Slider playerRage = GetOrCreateSlider(
            hud, "PlayerRageSlider", new Vector2(-105f, -82f),
            new Color(1f, 0.25f, 0.2f, 1f)
        );
        Slider enemyAction = GetOrCreateSlider(
            hud, "EnemyActionSlider", new Vector2(110f, -52f),
            new Color(0.2f, 0.75f, 1f, 1f)
        );
        Slider enemyRage = GetOrCreateSlider(
            hud, "EnemyRageSlider", new Vector2(110f, -82f),
            new Color(1f, 0.25f, 0.2f, 1f)
        );

        GetOrCreateText(
            hud, "PlayerActionLabel", "Action", new Vector2(-180f, -52f),
            new Vector2(60f, 22f), 13f
        );
        GetOrCreateText(
            hud, "PlayerRageLabel", "Rage", new Vector2(-180f, -82f),
            new Vector2(60f, 22f), 13f
        );
        GetOrCreateText(
            hud, "EnemyActionLabel", "Action", new Vector2(35f, -52f),
            new Vector2(60f, 22f), 13f
        );
        GetOrCreateText(
            hud, "EnemyRageLabel", "Rage", new Vector2(35f, -82f),
            new Vector2(60f, 22f), 13f
        );

        Button sweepButton = GetOrCreateButton(
            hud, "SweepButton", "Sweep", new Vector2(95f, -180f),
            new Vector2(160f, 32f)
        );

        TMP_Text enemyStatsText = GetOrCreateText(
            hud, "EnemyStatsText", "ATK 8  DEF 2  AGI 4  WIS 5  ULT Star Burst",
            new Vector2(0f, 110f), new Vector2(390f, 28f), 14f
        );
        enemyStatsText.fontSize = 12f;
        TMP_Text rewardPreviewText = GetOrCreateText(
            hud, "RewardPreviewText", "Reward: 10 Energy  First: 50 Energy",
            new Vector2(0f, 72f), new Vector2(400f, 34f), 14f
        );
        rewardPreviewText.fontSize = 12f;
        TMP_Text stageFeedbackText = GetOrCreateText(
            hud, "StageFeedbackText", "Stage 1 ready for first clear.",
            new Vector2(0f, 8f), new Vector2(390f, 48f), 15f
        );

        RectTransform startButton = FindChildByName(battlePanel, "StartBattleButton");
        if (startButton != null)
        {
            startButton.anchoredPosition = new Vector2(-95f, -180f);
        }

        Assign(battleSerialized, "playerActionSlider", playerAction);
        Assign(battleSerialized, "playerRageSlider", playerRage);
        Assign(battleSerialized, "enemyActionSlider", enemyAction);
        Assign(battleSerialized, "enemyRageSlider", enemyRage);
        Assign(battleSerialized, "selectedStageText", selectedStageText);
        Assign(battleSerialized, "stageProgressText", stageProgressText);
        Assign(battleSerialized, "battleStaminaText", staminaText);
        Assign(battleSerialized, "previousStageButton", previousButton);
        Assign(battleSerialized, "nextStageButton", nextButton);
        Assign(battleSerialized, "sweepButton", sweepButton);
        Assign(battleSerialized, "enemyStatsText", enemyStatsText);
        Assign(battleSerialized, "rewardPreviewText", rewardPreviewText);
        Assign(battleSerialized, "stageFeedbackText", stageFeedbackText);
        battleSerialized.ApplyModifiedPropertiesWithoutUndo();

        SetupWisdomText(statsUI);
        ApplyPortraitLayout(scene);

        EditorUtility.SetDirty(battleUI);
        EditorUtility.SetDirty(statsUI);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Battle/Main Stage prototype UI created and connected.");
    }

    public static void SetupPrototypeUIBatch()
    {
        Scene scene = EditorSceneManager.OpenScene(
            TargetScene,
            OpenSceneMode.Single
        );

        if (!scene.IsValid())
        {
            throw new System.InvalidOperationException(
                $"Could not open {TargetScene}."
            );
        }

        SetupPrototypeUI();
    }

    public static void AuditPanelBoundsBatch()
    {
        Scene scene = EditorSceneManager.OpenScene(
            TargetScene,
            OpenSceneMode.Single
        );

        int panelsChecked = 0;
        int outOfBounds = 0;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (RectTransform panel in root.GetComponentsInChildren<RectTransform>(true))
            {
                if (!panel.name.EndsWith("Panel")) continue;

                panelsChecked++;
                Rect bounds = panel.rect;

                foreach (Transform childTransform in panel)
                {
                    RectTransform child = childTransform as RectTransform;
                    if (child == null) continue;

                    Vector3[] corners = new Vector3[4];
                    child.GetWorldCorners(corners);

                    for (int i = 0; i < corners.Length; i++)
                    {
                        corners[i] = panel.InverseTransformPoint(corners[i]);
                    }

                    float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
                    float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
                    float minY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
                    float maxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);

                    const float tolerance = 1f;
                    if (minX < bounds.xMin - tolerance ||
                        maxX > bounds.xMax + tolerance ||
                        minY < bounds.yMin - tolerance ||
                        maxY > bounds.yMax + tolerance)
                    {
                        outOfBounds++;
                        Debug.LogWarning(
                            $"UI bounds: {panel.name}/{child.name} is outside " +
                            $"panel rect {bounds}. Child bounds: " +
                            $"({minX:F1}, {minY:F1}) to ({maxX:F1}, {maxY:F1})."
                        );
                    }


                    if (child.name == HudName)
                    {
                        foreach (Transform hudChildTransform in child)
                        {
                            RectTransform hudChild = hudChildTransform as RectTransform;
                            if (hudChild != null && IsOutsidePanel(panel, hudChild))
                            {
                                outOfBounds++;
                                Debug.LogWarning(
                                    $"UI bounds: {panel.name}/{HudName}/{hudChild.name} " +
                                    "is outside the panel rect."
                                );
                            }
                        }
                    }
                }
            }
        }

        Debug.Log(
            $"UI bounds audit complete. Panels checked: {panelsChecked}. " +
            $"Direct children outside bounds: {outOfBounds}."
        );
    }

    public static void FixLegacyPanelLayoutBatch()
    {
        Scene scene = EditorSceneManager.OpenScene(TargetScene, OpenSceneMode.Single);
        SetupPrototypeUI();
        FixLegacyPanelLayout(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Legacy panel layout adjusted to the 640x480 safe area.");
    }

    public static void ApplyPortraitLayoutBatch()
    {
        Scene scene = EditorSceneManager.OpenScene(TargetScene, OpenSceneMode.Single);
        SetupPrototypeUI();
        ApplyPortraitLayout(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;

        Debug.Log("Responsive 430x932 portrait layout and mobile orientation applied.");
    }

    private static void ApplyPortraitLayout(Scene scene)
    {
        Canvas canvas = FindComponentInScene<Canvas>(scene);
        CanvasScaler canvasScaler = FindComponentInScene<CanvasScaler>(scene);
        if (canvas == null || canvasScaler == null)
        {
            Debug.LogError("Canvas or CanvasScaler was not found.");
            return;
        }

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(430f, 932f);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        // Portrait layouts must preserve vertical space on both phones and tablets.
        // Wider devices receive extra side margins instead of losing UI height.
        canvasScaler.matchWidthOrHeight = 1f;
        EditorUtility.SetDirty(canvasScaler);

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform safeArea = FindChildByName(canvasRect, "SafeArea");
        if (safeArea == null)
        {
            safeArea = CreateRect("SafeArea", canvasRect);
            safeArea.gameObject.AddComponent<SafeAreaFitter>();
        }
        else if (safeArea.GetComponent<SafeAreaFitter>() == null)
        {
            safeArea.gameObject.AddComponent<SafeAreaFitter>();
        }

        StretchToParent(safeArea);
        safeArea.SetAsFirstSibling();

        string[] panelNames =
        {
            "MeditationPanel", "CollectionPanel", "UpgradePanel",
            "InventoryPanel", "CombinerPanel", "StatsPanel", "BattlePanel",
            "UltimatePanel", "EquipmentPanel"
        };

        foreach (string panelName in panelNames)
        {
            RectTransform panel = FindRectInScene(scene, panelName);
            if (panel == null) continue;

            panel.SetParent(safeArea, false);
            StretchToParent(panel);
        }

        RectTransform saveButton = FindRectInScene(scene, "SaveButton");
        RectTransform loadButton = FindRectInScene(scene, "LoadButton");
        if (saveButton != null)
        {
            saveButton.SetParent(safeArea, false);
            SetRect(saveButton, -78f, 395f, 140f, 36f);
            saveButton.SetAsLastSibling();
        }
        if (loadButton != null)
        {
            loadButton.SetParent(safeArea, false);
            SetRect(loadButton, 78f, 395f, 140f, 36f);
            loadButton.SetAsLastSibling();
        }

        LayoutMeditation(FindRectInScene(scene, "MeditationPanel"));
        LayoutCollection(FindRectInScene(scene, "CollectionPanel"));
        LayoutUpgrade(FindRectInScene(scene, "UpgradePanel"));
        LayoutInventory(FindRectInScene(scene, "InventoryPanel"));
        LayoutCombiner(FindRectInScene(scene, "CombinerPanel"));
        LayoutStats(FindRectInScene(scene, "StatsPanel"));
        LayoutBattle(FindRectInScene(scene, "BattlePanel"));
        LayoutUltimate(FindRectInScene(scene, "UltimatePanel"));
        LayoutEquipment(FindRectInScene(scene, "EquipmentPanel"));

        EditorUtility.SetDirty(safeArea);
    }

    private static void LayoutMeditation(RectTransform panel)
    {
        Position(panel, "BodyLevelText", 0f, 300f, 360f, 52f);
        Position(panel, "ExpText", 0f, 245f, 360f, 44f);
        Position(panel, "EnergyText", 0f, 195f, 360f, 44f);
        Position(panel, "MeditateButton", 0f, 105f, 260f, 56f);
        Position(panel, "AutoMeditateButton", 0f, 35f, 260f, 56f);

        Position(panel, "GoToCollectionButton", -100f, -190f, 185f, 48f);
        Position(panel, "GoToUpgradeButton", 100f, -190f, 185f, 48f);
        Position(panel, "GoToInventoryButton", -100f, -250f, 185f, 48f);
        Position(panel, "GoToCombinerButton", 100f, -250f, 185f, 48f);
        Position(panel, "GoToStatsButton", -100f, -310f, 185f, 48f);
        Position(panel, "GoToBattleButton", 100f, -310f, 185f, 48f);
        Position(panel, "GoToUltimateButton", -100f, -370f, 185f, 48f);
        Position(panel, "GoToEquipmentButton", 100f, -370f, 185f, 48f);
    }

    private static void LayoutCollection(RectTransform panel)
    {
        Position(panel, "EnergyText", 0f, 230f, 360f, 50f);
        Position(panel, "RuneText", 0f, 170f, 360f, 50f);
        Position(panel, "CollectButton", 0f, 70f, 260f, 60f);
        Position(panel, "GoToMeditationButton", 0f, -360f, 260f, 52f);
    }

    private static void LayoutUpgrade(RectTransform panel)
    {
        Position(panel, "BodyLevelText", 0f, 80f, 390f, 80f);
        Position(panel, "UpgradeCostText", 0f, 0f, 390f, 80f);
        Position(panel, "UpgradeButton", 0f, 0f, 390f, 80f);

        string[] rows = { "HeadRow", "ArmsRow", "LegsRow", "ChestRow", "FeetRow", "WeaponRow" };
        float[] y = { 240f, 145f, 50f, -45f, -140f, -235f };

        for (int i = 0; i < rows.Length; i++)
        {
            RectTransform row = FindChildByName(panel, rows[i]);
            if (row == null) continue;

            row.localScale = new Vector3(0.55f, 0.55f, 1f);
            row.anchorMin = row.anchorMax = row.pivot = new Vector2(0.5f, 0.5f);
            row.anchoredPosition = new Vector2(0f, y[i]);
        }

        Position(panel, "GoToMeditationFromUpgradeButton", 0f, -375f, 260f, 52f);
    }

    private static void LayoutInventory(RectTransform panel)
    {
        Position(panel, "MemoryFragmentText", 0f, 220f, 360f, 60f);
        Position(panel, "RuneText", 0f, 150f, 360f, 60f);
        Position(panel, "GoToMeditationFromInventoryButton", 0f, -360f, 260f, 52f);
    }

    private static void LayoutCombiner(RectTransform panel)
    {
        Position(panel, "CurrentBuffText", 0f, 235f, 380f, 180f);
        Position(panel, "CreateMeditationBuffButton", 0f, 65f, 320f, 58f);
        Position(panel, "CreateCollectionBuffButton", 0f, -15f, 320f, 58f);
        Position(panel, "GoToMeditationFromCombinerButton", 0f, -360f, 260f, 52f);
    }

    private static void LayoutStats(RectTransform panel)
    {
        string[] names =
        {
            "MeditationBonusText", "CollectionBonusText", "HPText", "AttackText",
            "DefenseText", "SpeedText", "WisdomText"
        };
        float startY = 250f;

        for (int i = 0; i < names.Length; i++)
        {
            Position(panel, names[i], 0f, startY - i * 58f, 360f, 48f);
        }

        Position(panel, "GoToMeditationFromStatsButton", 0f, -375f, 260f, 52f);
    }

    private static void LayoutBattle(RectTransform panel)
    {
        Position(panel, "SelectedStageText", 0f, 370f, 180f, 38f);
        Position(panel, "PreviousStageButton", -145f, 370f, 54f, 44f);
        Position(panel, "NextStageButton", 145f, 370f, 54f, 44f);
        Position(panel, "StageProgressText", -105f, 335f, 205f, 30f);
        Position(panel, "BattleStaminaText", 105f, 335f, 205f, 30f);

        Position(panel, "EnemyNameText", 0f, 300f, 380f, 38f);
        Position(panel, "EnemyStatsText", 0f, 262f, 400f, 50f);
        Position(panel, "EnemyHPText", 0f, 222f, 300f, 26f);
        Position(panel, "EnemyHPSlider", 0f, 201f, 310f, 16f);
        Position(panel, "EnemyActionLabel", -155f, 175f, 70f, 20f);
        Position(panel, "EnemyActionSlider", 35f, 175f, 290f, 16f);
        Position(panel, "EnemyRageLabel", -155f, 149f, 70f, 20f);
        Position(panel, "EnemyRageSlider", 35f, 149f, 290f, 16f);

        Position(panel, "RewardPreviewText", 0f, 108f, 400f, 50f);
        Position(panel, "BattleStatusText", 0f, 70f, 300f, 34f);
        Position(panel, "StageFeedbackText", 0f, 25f, 390f, 48f);

        Position(panel, "PlayerNameText", 0f, -35f, 360f, 38f);
        Position(panel, "PlayerHPText", 0f, -68f, 300f, 28f);
        Position(panel, "PlayerHPSlider", 0f, -91f, 310f, 18f);
        Position(panel, "PlayerActionLabel", -155f, -119f, 70f, 22f);
        Position(panel, "PlayerActionSlider", 35f, -119f, 290f, 18f);
        Position(panel, "PlayerRageLabel", -155f, -147f, 70f, 22f);
        Position(panel, "PlayerRageSlider", 35f, -147f, 290f, 18f);

        Position(panel, "BuildSummaryButton", 0f, -250f, 220f, 46f);
        Position(panel, "StartBattleButton", -95f, -315f, 170f, 52f);
        Position(panel, "SweepButton", 95f, -315f, 170f, 52f);
        Position(panel, "ReturnButton", 0f, -375f, 220f, 50f);
        Position(panel, "BuildSummaryText", 0f, 35f, 390f, 650f);
        Position(panel, "CloseBuildSummaryButton", 0f, -350f, 220f, 50f);
    }

    private static void LayoutUltimate(RectTransform panel)
    {
        Position(panel, "UltimateTitleText", 0f, 360f, 380f, 48f);
        Position(panel, "EquippedUltimateText", 0f, 295f, 390f, 70f);
        string[] buttons =
        {
            "StarBurstButton", "IronRetaliationButton", "RapidNovaButton",
            "MeteorFlurryButton", "SwiftAscensionButton"
        };
        float[] y = { 205f, 105f, 5f, -95f, -195f };
        for (int i = 0; i < buttons.Length; i++)
            Position(panel, buttons[i], 0f, y[i], 390f, 84f);
        Position(panel, "ReturnFromUltimateButton", 0f, -375f, 220f, 50f);
    }

    private static void LayoutEquipment(RectTransform panel)
    {
        Position(panel, "EquipmentTitleText", 0f, 365f, 380f, 46f);
        Position(panel, "EquipmentSummaryText", 0f, 305f, 400f, 70f);
        string[] buttons =
        {
            "HeadEquipmentButton", "ChestEquipmentButton", "ArmsEquipmentButton",
            "LegsEquipmentButton", "FeetEquipmentButton", "WeaponEquipmentButton",
            "AccessoryEquipmentButton"
        };
        float[] y = { 225f, 155f, 85f, 15f, -55f, -125f, -195f };
        for (int i = 0; i < buttons.Length; i++)
            Position(panel, buttons[i], 0f, y[i], 390f, 58f);
        Position(panel, "ReturnFromEquipmentButton", 0f, -375f, 220f, 50f);
    }

    private static void SetupUltimateUI(Scene scene)
    {
        RectTransform battlePanel = FindRectInScene(scene, "BattlePanel");
        RectTransform meditationPanel = FindRectInScene(scene, "MeditationPanel");
        if (battlePanel == null || meditationPanel == null) return;

        RectTransform panel = FindRectInScene(scene, "UltimatePanel");
        if (panel == null)
        {
            panel = CreateRect("UltimatePanel", battlePanel.parent);
            StretchToParent(panel);
        }

        UltimateUI ultimateUI = panel.GetComponent<UltimateUI>();
        if (ultimateUI == null) ultimateUI = panel.gameObject.AddComponent<UltimateUI>();

        TMP_Text title = GetOrCreateText(panel, "UltimateTitleText", "ULTIMATES",
            new Vector2(0f, 360f), new Vector2(380f, 48f), 28f);
        TMP_Text equipped = GetOrCreateText(panel, "EquippedUltimateText",
            "Equipped: Star Burst", new Vector2(0f, 295f), new Vector2(390f, 70f), 17f);

        Button star = GetOrCreateButton(panel, "StarBurstButton", "Star Burst",
            new Vector2(0f, 205f), new Vector2(390f, 84f));
        Button iron = GetOrCreateButton(panel, "IronRetaliationButton", "Iron Retaliation",
            new Vector2(0f, 105f), new Vector2(390f, 84f));
        Button rapid = GetOrCreateButton(panel, "RapidNovaButton", "Rapid Nova",
            new Vector2(0f, 5f), new Vector2(390f, 84f));
        Button meteor = GetOrCreateButton(panel, "MeteorFlurryButton", "Meteor Flurry",
            new Vector2(0f, -95f), new Vector2(390f, 84f));
        Button swift = GetOrCreateButton(panel, "SwiftAscensionButton", "Swift Ascension",
            new Vector2(0f, -195f), new Vector2(390f, 84f));
        Button returnButton = GetOrCreateButton(panel, "ReturnFromUltimateButton", "Return",
            new Vector2(0f, -375f), new Vector2(220f, 50f));

        Button navButton = GetOrCreateButton(meditationPanel, "GoToUltimateButton", "Ultimates",
            new Vector2(0f, -370f), new Vector2(240f, 48f));

        SerializedObject uiSerialized = new SerializedObject(ultimateUI);
        Assign(uiSerialized, "equippedUltimateText", equipped);
        Assign(uiSerialized, "starBurstButton", star);
        Assign(uiSerialized, "ironRetaliationButton", iron);
        Assign(uiSerialized, "rapidNovaButton", rapid);
        Assign(uiSerialized, "meteorFlurryButton", meteor);
        Assign(uiSerialized, "swiftAscensionButton", swift);
        Assign(uiSerialized, "returnButton", returnButton);
        uiSerialized.ApplyModifiedPropertiesWithoutUndo();

        PanelSwitcher switcher = FindComponentInScene<PanelSwitcher>(scene);
        SerializedObject switcherSerialized = new SerializedObject(switcher);
        Assign(switcherSerialized, "ultimatePanel", panel.gameObject);
        Assign(switcherSerialized, "goToUltimateButton", navButton);
        switcherSerialized.ApplyModifiedPropertiesWithoutUndo();

        GameManager manager = FindComponentInScene<GameManager>(scene);
        SerializedObject managerSerialized = new SerializedObject(manager);
        Assign(managerSerialized, "ultimateUI", ultimateUI);
        managerSerialized.ApplyModifiedPropertiesWithoutUndo();
        panel.gameObject.SetActive(false);
        EditorUtility.SetDirty(ultimateUI);
        EditorUtility.SetDirty(title);
    }

    private static void SetupEquipmentUI(Scene scene)
    {
        RectTransform battlePanel = FindRectInScene(scene, "BattlePanel");
        RectTransform meditationPanel = FindRectInScene(scene, "MeditationPanel");
        if (battlePanel == null || meditationPanel == null) return;

        RectTransform panel = FindRectInScene(scene, "EquipmentPanel");
        if (panel == null)
        {
            panel = CreateRect("EquipmentPanel", battlePanel.parent);
            StretchToParent(panel);
        }

        EquipmentUI equipmentUI = panel.GetComponent<EquipmentUI>();
        if (equipmentUI == null) equipmentUI = panel.gameObject.AddComponent<EquipmentUI>();

        TMP_Text title = GetOrCreateText(panel, "EquipmentTitleText", "EQUIPMENT",
            new Vector2(0f, 365f), new Vector2(380f, 46f), 28f);
        TMP_Text summary = GetOrCreateText(panel, "EquipmentSummaryText",
            "Equipment Total", new Vector2(0f, 305f), new Vector2(400f, 70f), 15f);

        Button head = GetOrCreateButton(panel, "HeadEquipmentButton", "Head: None",
            new Vector2(0f, 225f), new Vector2(390f, 58f));
        Button chest = GetOrCreateButton(panel, "ChestEquipmentButton", "Chest: None",
            new Vector2(0f, 155f), new Vector2(390f, 58f));
        Button arms = GetOrCreateButton(panel, "ArmsEquipmentButton", "Arms: None",
            new Vector2(0f, 85f), new Vector2(390f, 58f));
        Button legs = GetOrCreateButton(panel, "LegsEquipmentButton", "Legs: None",
            new Vector2(0f, 15f), new Vector2(390f, 58f));
        Button feet = GetOrCreateButton(panel, "FeetEquipmentButton", "Feet: None",
            new Vector2(0f, -55f), new Vector2(390f, 58f));
        Button weapon = GetOrCreateButton(panel, "WeaponEquipmentButton", "Weapon: None",
            new Vector2(0f, -125f), new Vector2(390f, 58f));
        Button accessory = GetOrCreateButton(panel, "AccessoryEquipmentButton", "Accessory: None",
            new Vector2(0f, -195f), new Vector2(390f, 58f));
        Button returnButton = GetOrCreateButton(panel, "ReturnFromEquipmentButton", "Return",
            new Vector2(0f, -375f), new Vector2(220f, 50f));
        Button navButton = GetOrCreateButton(meditationPanel, "GoToEquipmentButton", "Equipment",
            new Vector2(100f, -370f), new Vector2(185f, 48f));

        SerializedObject uiSerialized = new SerializedObject(equipmentUI);
        Assign(uiSerialized, "summaryText", summary);
        Assign(uiSerialized, "headButton", head);
        Assign(uiSerialized, "chestButton", chest);
        Assign(uiSerialized, "armsButton", arms);
        Assign(uiSerialized, "legsButton", legs);
        Assign(uiSerialized, "feetButton", feet);
        Assign(uiSerialized, "weaponButton", weapon);
        Assign(uiSerialized, "accessoryButton", accessory);
        Assign(uiSerialized, "returnButton", returnButton);
        uiSerialized.ApplyModifiedPropertiesWithoutUndo();

        PanelSwitcher switcher = FindComponentInScene<PanelSwitcher>(scene);
        SerializedObject switcherSerialized = new SerializedObject(switcher);
        Assign(switcherSerialized, "equipmentPanel", panel.gameObject);
        Assign(switcherSerialized, "goToEquipmentButton", navButton);
        switcherSerialized.ApplyModifiedPropertiesWithoutUndo();

        GameManager manager = FindComponentInScene<GameManager>(scene);
        SerializedObject managerSerialized = new SerializedObject(manager);
        Assign(managerSerialized, "equipmentUI", equipmentUI);
        managerSerialized.ApplyModifiedPropertiesWithoutUndo();
        panel.gameObject.SetActive(false);
        EditorUtility.SetDirty(equipmentUI);
        EditorUtility.SetDirty(title);
    }

    private static void SetupBuildSummary(
        RectTransform battlePanel,
        SerializedObject battleSerialized)
    {
        RectTransform hud = FindChildByName(battlePanel, HudName);
        Button openButton = GetOrCreateButton(hud, "BuildSummaryButton", "View Build",
            new Vector2(0f, -250f), new Vector2(220f, 46f));

        RectTransform overlay = FindChildByName(battlePanel, "BuildSummaryOverlay");
        if (overlay == null)
        {
            overlay = CreateRect("BuildSummaryOverlay", battlePanel);
            StretchToParent(overlay);
            Image background = overlay.gameObject.AddComponent<Image>();
            background.color = new Color(0.025f, 0.035f, 0.065f, 0.98f);
        }

        TMP_Text title = GetOrCreateText(overlay, "BuildSummaryTitle", "BATTLE BUILD",
            new Vector2(0f, 380f), new Vector2(380f, 48f), 28f);
        TMP_Text summary = GetOrCreateText(overlay, "BuildSummaryText", "Build summary",
            new Vector2(0f, 35f), new Vector2(390f, 650f), 16f);
        summary.alignment = TextAlignmentOptions.TopLeft;
        Button closeButton = GetOrCreateButton(overlay, "CloseBuildSummaryButton", "Back to Stage",
            new Vector2(0f, -350f), new Vector2(220f, 50f));

        Assign(battleSerialized, "buildSummaryButton", openButton);
        Assign(battleSerialized, "buildSummaryPanel", overlay.gameObject);
        Assign(battleSerialized, "buildSummaryText", summary);
        Assign(battleSerialized, "closeBuildSummaryButton", closeButton);
        overlay.gameObject.SetActive(false);
        EditorUtility.SetDirty(title);
    }

    private static void SetRect(
        RectTransform rect,
        float x,
        float y,
        float width,
        float height)
    {
        if (rect == null) return;

        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);
        rect.localScale = Vector3.one;
    }

    private static void FixLegacyPanelLayout(Scene scene)
    {
        CanvasScaler canvasScaler = FindComponentInScene<CanvasScaler>(scene);
        if (canvasScaler != null)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(800f, 600f);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
            EditorUtility.SetDirty(canvasScaler);
        }

        RectTransform meditation = FindRectInScene(scene, "MeditationPanel");
        Position(meditation, "GoToCollectionButton", -150f, -175f, 130f, 30f);
        Position(meditation, "GoToUpgradeButton", 0f, -175f, 130f, 30f);
        Position(meditation, "GoToInventoryButton", 150f, -175f, 130f, 30f);
        Position(meditation, "GoToCombinerButton", -150f, -215f, 130f, 30f);
        Position(meditation, "GoToStatsButton", 0f, -215f, 130f, 30f);
        Position(meditation, "GoToBattleButton", 150f, -215f, 130f, 30f);

        RectTransform upgrade = FindRectInScene(scene, "UpgradePanel");
        string[] rows = { "HeadRow", "ArmsRow", "LegsRow", "ChestRow", "FeetRow", "WeaponRow" };
        foreach (string rowName in rows)
        {
            RectTransform row = FindChildByName(upgrade, rowName);
            if (row != null) row.localScale = new Vector3(0.85f, 0.85f, 1f);
        }
        Position(upgrade, "GoToMeditationFromUpgradeButton", 0f, -205f, 240f, 50f);

        RectTransform combiner = FindRectInScene(scene, "CombinerPanel");
        Position(combiner, "CurrentBuffText", 0f, 90f, 400f, 300f);

        RectTransform stats = FindRectInScene(scene, "StatsPanel");
        Position(stats, "GoToMeditationFromStatsButton", 0f, -205f, 200f, 50f);

        RectTransform battle = FindRectInScene(scene, "BattlePanel");
        Position(battle, "StartBattleButton", -95f, -180f, 160f, 30f);
        Position(battle, "ReturnButton", 0f, -220f, 160f, 30f);
    }

    private static bool IsOutsidePanel(RectTransform panel, RectTransform child)
    {
        Vector3[] corners = new Vector3[4];
        child.GetWorldCorners(corners);
        Rect bounds = panel.rect;

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 local = panel.InverseTransformPoint(corners[i]);
            if (local.x < bounds.xMin - 1f || local.x > bounds.xMax + 1f ||
                local.y < bounds.yMin - 1f || local.y > bounds.yMax + 1f)
                return true;
        }

        return false;
    }

    private static RectTransform FindRectInScene(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == name) return root.GetComponent<RectTransform>();

            RectTransform result = FindChildByName(root.transform, name);
            if (result != null) return result;
        }

        return null;
    }

    private static void Position(
        RectTransform parent,
        string childName,
        float x,
        float y,
        float width,
        float height)
    {
        RectTransform child = FindChildByName(parent, childName);
        if (child == null) return;

        child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
        child.anchoredPosition = new Vector2(x, y);
        child.sizeDelta = new Vector2(width, height);
    }

    private static void SetupWisdomText(StatsUI statsUI)
    {
        RectTransform statsPanel = statsUI.GetComponent<RectTransform>();
        RectTransform existing = FindChildByName(statsPanel, "WisdomText");
        TMP_Text wisdomText;

        if (existing != null)
        {
            wisdomText = existing.GetComponent<TMP_Text>();
        }
        else
        {
            RectTransform agility = FindChildByName(statsPanel, "SpeedText");
            if (agility == null)
            {
                Debug.LogWarning("SpeedText was not found; WisdomText was not created.");
                return;
            }

            GameObject clone = Object.Instantiate(agility.gameObject, statsPanel);
            clone.name = "WisdomText";
            RectTransform rect = clone.GetComponent<RectTransform>();
            rect.anchoredPosition = agility.anchoredPosition + new Vector2(0f, -50f);
            wisdomText = clone.GetComponent<TMP_Text>();
            wisdomText.text = "Wisdom: 5";
        }

        SerializedObject statsSerialized = new SerializedObject(statsUI);
        Assign(statsSerialized, "wisdomText", wisdomText);
        statsSerialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static TMP_Text GetOrCreateText(
        Transform parent,
        string name,
        string value,
        Vector2 position,
        Vector2 size,
        float fontSize)
    {
        RectTransform existing = FindChildByName(parent, name);
        TMP_Text text;

        if (existing != null)
        {
            text = existing.GetComponent<TMP_Text>();
        }
        else
        {
            RectTransform rect = CreateRect(name, parent);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        }

        RectTransform textRect = text.rectTransform;
        textRect.anchoredPosition = position;
        textRect.sizeDelta = size;
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static Button GetOrCreateButton(
        Transform parent,
        string name,
        string label,
        Vector2 position,
        Vector2 size)
    {
        RectTransform existing = FindChildByName(parent, name);
        RectTransform rect = existing != null ? existing : CreateRect(name, parent);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        if (existing != null) return rect.GetComponent<Button>();

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(0.17f, 0.23f, 0.36f, 0.95f);

        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;

        TMP_Text text = GetOrCreateText(
            rect, "Label", label, Vector2.zero, size, 17f
        );
        StretchToParent(text.rectTransform);
        return button;
    }

    private static Slider GetOrCreateSlider(
        Transform parent,
        string name,
        Vector2 position,
        Color fillColor)
    {
        RectTransform existing = FindChildByName(parent, name);
        RectTransform root = existing != null ? existing : CreateRect(name, parent);
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(0.5f, 0.5f);
        root.anchoredPosition = position;
        root.sizeDelta = new Vector2(125f, 18f);

        if (existing != null) return root.GetComponent<Slider>();

        Image background = root.gameObject.AddComponent<Image>();
        background.color = new Color(0.05f, 0.06f, 0.09f, 0.9f);

        RectTransform fillArea = CreateRect("Fill Area", root);
        StretchToParent(fillArea);
        fillArea.offsetMin = new Vector2(3f, 3f);
        fillArea.offsetMax = new Vector2(-3f, -3f);

        RectTransform fill = CreateRect("Fill", fillArea);
        StretchToParent(fill);
        Image fillImage = fill.gameObject.AddComponent<Image>();
        fillImage.color = fillColor;

        Slider slider = root.gameObject.AddComponent<Slider>();
        slider.interactable = false;
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 0f;
        slider.fillRect = fill;
        slider.targetGraphic = fillImage;
        slider.direction = Slider.Direction.LeftToRight;
        return slider;
    }

    private static RectTransform CreateRect(string name, Transform parent)
    {
        GameObject gameObject = new GameObject(
            name,
            typeof(RectTransform),
            typeof(CanvasRenderer)
        );
        Undo.RegisterCreatedObjectUndo(gameObject, $"Create {name}");
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        return rect;
    }

    private static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }

    private static void Assign(SerializedObject target, string property, Object value)
    {
        SerializedProperty serializedProperty = target.FindProperty(property);
        if (serializedProperty == null)
        {
            Debug.LogError($"Serialized property '{property}' was not found.");
            return;
        }

        serializedProperty.objectReferenceValue = value;
    }

    private static RectTransform FindChildByName(Transform parent, string name)
    {
        if (parent == null) return null;

        foreach (Transform child in parent)
        {
            if (child.name == name) return child as RectTransform;

            RectTransform nested = FindChildByName(child, name);
            if (nested != null) return nested;
        }

        return null;
    }

    private static T FindComponentInScene<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T component = root.GetComponentInChildren<T>(true);
            if (component != null) return component;
        }

        return null;
    }
}
