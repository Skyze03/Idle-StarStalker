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
        SetupInventoryResources(scene);
        SetupEditBuildUI(scene);
        SetupFeedbackUI(scene);
        SetupBattleModes(scene);
        SetupBuildSummary(battlePanel, battleSerialized);
        SetupCombatLog(hud, battleSerialized);

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

        Slider sharedTimeline = GetOrCreateSlider(
            hud, "SharedActionTimeline", new Vector2(0f, 125f),
            new Color(0.35f, 0.42f, 0.58f, 1f)
        );
        RectTransform playerMarker = GetOrCreateTimelineMarker(
            sharedTimeline.transform, "PlayerTimelineMarker", "P",
            new Color(0.2f, 0.75f, 1f, 1f), -16f
        );
        RectTransform enemyMarker = GetOrCreateTimelineMarker(
            sharedTimeline.transform, "EnemyTimelineMarker", "E",
            new Color(1f, 0.35f, 0.3f, 1f), 16f
        );
        sharedTimeline.GetComponent<RectTransform>().sizeDelta = new Vector2(350f, 18f);
        GetOrCreateText(hud, "SharedActionTimelineLabel", "ACTION TIMELINE",
            new Vector2(0f, 153f), new Vector2(220f, 24f), 14f);

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

        playerAction.gameObject.SetActive(false);
        enemyAction.gameObject.SetActive(false);
        RectTransform playerActionLabel = FindChildByName(hud, "PlayerActionLabel");
        RectTransform enemyActionLabel = FindChildByName(hud, "EnemyActionLabel");
        if (playerActionLabel != null) playerActionLabel.gameObject.SetActive(false);
        if (enemyActionLabel != null) enemyActionLabel.gameObject.SetActive(false);
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
        Assign(battleSerialized, "sharedActionTimeline", sharedTimeline);
        Assign(battleSerialized, "playerTimelineMarker", playerMarker);
        Assign(battleSerialized, "enemyTimelineMarker", enemyMarker);
        Assign(battleSerialized, "sharedActionTimelineLabel",
            FindChildByName(hud, "SharedActionTimelineLabel")?.gameObject);
        Assign(battleSerialized, "playerRageLabel",
            FindChildByName(hud, "PlayerRageLabel")?.gameObject);
        Assign(battleSerialized, "enemyRageLabel",
            FindChildByName(hud, "EnemyRageLabel")?.gameObject);
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
            , "EditBuildPanel", "BattleModePanel", "DailyChallengePanel"
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
        LayoutEditBuild(FindRectInScene(scene, "EditBuildPanel"));
        LayoutBattleMode(FindRectInScene(scene, "BattleModePanel"));
        LayoutDailyChallenge(FindRectInScene(scene, "DailyChallengePanel"));

        EditorUtility.SetDirty(safeArea);
    }

    private static void LayoutMeditation(RectTransform panel)
    {
        Position(panel, "LevelText", 0f, 270f, 360f, 42f);
        Position(panel, "ExpText", 0f, 225f, 360f, 38f);
        Position(panel, "ExpSlider", 0f, 185f, 320f, 22f);
        Position(panel, "MeditateButton", 0f, 95f, 140f, 140f);
        Position(panel, "AutoMeditateButton", 0f, -20f, 260f, 58f);

        NormalizeMeditationText(panel, "LevelText", 22f);
        NormalizeMeditationText(panel, "ExpText", 18f);
        NormalizeMeditateButton(panel);

        Position(panel, "GoToCollectionButton", -100f, -190f, 185f, 48f);
        Position(panel, "GoToUpgradeButton", 100f, -190f, 185f, 48f);
        Position(panel, "GoToInventoryButton", -100f, -250f, 185f, 48f);
        Position(panel, "GoToCombinerButton", 100f, -250f, 185f, 48f);
        Position(panel, "GoToStatsButton", -100f, -310f, 185f, 48f);
        Position(panel, "GoToBattleButton", 100f, -310f, 185f, 48f);
        Position(panel, "GoToUltimateButton", -100f, -370f, 185f, 48f);
        Position(panel, "GoToEquipmentButton", 100f, -370f, 185f, 48f);
        Position(panel, "GoToEditBuildButton", 0f, -370f, 390f, 48f);
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
        string[] rows = { "WeaponRow", "HeadRow", "ArmsRow", "ChestRow", "LegsRow", "FeetRow" };
        Vector2[] positions =
        {
            new Vector2(-102f, 225f), new Vector2(102f, 225f),
            new Vector2(-102f, 25f), new Vector2(102f, 25f),
            new Vector2(-102f, -175f), new Vector2(102f, -175f)
        };

        for (int i = 0; i < rows.Length; i++)
        {
            RectTransform row = FindChildByName(panel, rows[i]);
            if (row == null) continue;
            foreach (LayoutGroup layout in row.GetComponents<LayoutGroup>())
                Object.DestroyImmediate(layout);
            foreach (ContentSizeFitter fitter in row.GetComponents<ContentSizeFitter>())
                Object.DestroyImmediate(fitter);
            SetRect(row, positions[i].x, positions[i].y, 190f, 180f);
            Position(row, "BodyLevelText", 0f, 28f, 182f, 100f);
            Position(row, "UpgradeCostText", 0f, -28f, 170f, 30f);
            Position(row, "UpgradeButton", 0f, -62f, 174f, 48f);

            RectTransform cost = FindChildByName(row, "UpgradeCostText");
            if (cost != null) cost.gameObject.SetActive(false);
            RectTransform level = FindChildByName(row, "BodyLevelText");
            TMP_Text levelLabel = level != null ? level.GetComponent<TMP_Text>() : null;
            if (levelLabel != null)
            {
                levelLabel.fontSize = 12f;
                levelLabel.enableAutoSizing = false;
                levelLabel.textWrappingMode = TextWrappingModes.NoWrap;
                levelLabel.overflowMode = TextOverflowModes.Overflow;
                levelLabel.alignment = TextAlignmentOptions.Center;
            }
            RectTransform buttonRect = FindChildByName(row, "UpgradeButton");
            TMP_Text buttonLabel = buttonRect != null
                ? buttonRect.GetComponentInChildren<TMP_Text>(true)
                : null;
            if (buttonLabel != null)
            {
                buttonLabel.fontSize = 12f;
                buttonLabel.enableAutoSizing = false;
                buttonLabel.alignment = TextAlignmentOptions.Center;
            }
        }

        Position(panel, "GoToMeditationFromUpgradeButton", 0f, -385f, 220f, 46f);
    }

    private static void LayoutInventory(RectTransform panel)
    {
        Position(panel, "MemoryFragmentText", 0f, 220f, 360f, 60f);
        Position(panel, "RuneText", 0f, 150f, 360f, 60f);
        Position(panel, "StarDustText", 0f, 80f, 360f, 60f);
        Position(panel, "GoToMeditationFromInventoryButton", 0f, -360f, 260f, 52f);
    }

    private static void SetupInventoryResources(Scene scene)
    {
        InventoryUI ui = FindComponentInScene<InventoryUI>(scene);
        if (ui == null) return;
        RectTransform panel = ui.GetComponent<RectTransform>();
        TMP_Text starDust = GetOrCreateText(panel, "StarDustText", "Star Dust: 0",
            new Vector2(0f, 80f), new Vector2(360f, 60f), 20f);
        SerializedObject serialized = new SerializedObject(ui);
        Assign(serialized, "starDustText", starDust);
        serialized.ApplyModifiedPropertiesWithoutUndo();
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
        Position(panel, "ReturnButton", -170f, 385f, 72f, 40f);
        SetButtonLabel(panel, "ReturnButton", "Back", 15f);
        Position(panel, "SelectedStageText", 0f, 345f, 200f, 44f);
        Position(panel, "PreviousStageButton", -125f, 345f, 52f, 42f);
        Position(panel, "NextStageButton", 125f, 345f, 52f, 42f);
        Position(panel, "StageProgressText", -105f, 300f, 205f, 34f);
        Position(panel, "BattleStaminaText", 105f, 300f, 205f, 34f);

        Position(panel, "EnemyNameText", 0f, 285f, 390f, 48f);
        Position(panel, "EnemyStatsText", 0f, 145f, 400f, 100f);
        Position(panel, "EnemyHPText", 105f, 205f, 190f, 26f);
        Position(panel, "EnemyHPSlider", 105f, 181f, 180f, 16f);
        Position(panel, "EnemyRageLabel", 35f, 148f, 50f, 20f);
        Position(panel, "EnemyRageSlider", 125f, 148f, 130f, 16f);

        Position(panel, "RewardPreviewText", 0f, 35f, 400f, 90f);
        Position(panel, "BattleStatusText", 0f, -235f, 300f, 34f);
        Position(panel, "StageFeedbackText", 0f, -285f, 390f, 70f);

        Position(panel, "PlayerNameText", -105f, 250f, 190f, 42f);
        Position(panel, "PlayerHPText", -105f, 205f, 190f, 26f);
        Position(panel, "PlayerHPSlider", -105f, 181f, 180f, 16f);
        Position(panel, "PlayerRageLabel", -175f, 148f, 50f, 20f);
        Position(panel, "PlayerRageSlider", -85f, 148f, 130f, 16f);
        Position(panel, "SharedActionTimelineLabel", 0f, -260f, 220f, 20f);
        Position(panel, "SharedActionTimeline", 0f, -292f, 350f, 18f);

        Position(panel, "BuildSummaryButton", 0f, -285f, 190f, 44f);
        Position(panel, "StartBattleButton", -82f, -345f, 150f, 46f);
        Position(panel, "SweepButton", 82f, -345f, 150f, 46f);
        Position(panel, "CombatLogToggleButton", -125f, -385f, 160f, 38f);
        Position(panel, "CombatLogPanel", 0f, -260f, 400f, 210f);

        SetTextStyle(panel, "SelectedStageText", 26f, false);
        SetTextStyle(panel, "StageProgressText", 17f, false);
        SetTextStyle(panel, "BattleStaminaText", 17f, false);
        SetTextStyle(panel, "EnemyNameText", 22f, false);
        SetTextStyle(panel, "EnemyStatsText", 17f, false);
        SetTextStyle(panel, "RewardPreviewText", 17f, false);
        Position(panel, "BuildSummaryText", 0f, 35f, 390f, 650f);
        Position(panel, "CloseBuildSummaryButton", 0f, -350f, 220f, 50f);
    }

    private static void LayoutEditBuild(RectTransform panel)
    {
        Position(panel, "EditBuildTitle", 0f, 385f, 390f, 42f);
        Position(panel, "EquipmentTabButton", -130f, 335f, 120f, 42f);
        Position(panel, "UltimateTabButton", 0f, 335f, 120f, 42f);
        Position(panel, "SummaryTabButton", 130f, 335f, 120f, 42f);
        Position(panel, "ReturnFromEditBuildButton", 0f, -395f, 220f, 46f);
    }

    private static void LayoutBattleMode(RectTransform panel) { }
    private static void LayoutDailyChallenge(RectTransform panel) { }

    private static void SetupBattleModes(Scene scene)
    {
        RectTransform battle = FindRectInScene(scene, "BattlePanel");
        if (battle == null) return;
        Transform parent = battle.parent;

        RectTransform hub = FindRectInScene(scene, "BattleModePanel");
        if (hub == null) { hub = CreateRect("BattleModePanel", parent); StretchToParent(hub); }
        BattleModeUI hubUI = hub.GetComponent<BattleModeUI>();
        if (hubUI == null) hubUI = hub.gameObject.AddComponent<BattleModeUI>();
        GetOrCreateText(hub, "BattleModeTitle", "BATTLE MODES", new Vector2(0f, 330f), new Vector2(390f, 60f), 30f);
        Button main = GetOrCreateButton(hub, "MainStoryModeButton", "Main Story\n20-stage progression", new Vector2(0f, 190f), new Vector2(360f, 100f));
        Button daily = GetOrCreateButton(hub, "DailyModeButton", "Daily Challenge\n3 attempts per UTC day", new Vector2(0f, 60f), new Vector2(360f, 100f));
        Button elite = GetOrCreateButton(hub, "EliteModeButton", "Elite Challenge", new Vector2(0f, -70f), new Vector2(360f, 100f));
        TMP_Text eliteStatus = GetOrCreateText(hub, "EliteModeStatus", "Coming Soon", new Vector2(0f, -135f), new Vector2(300f, 30f), 15f);
        Button hubReturn = GetOrCreateButton(hub, "ReturnFromBattleModes", "Back", new Vector2(0f, -360f), new Vector2(200f, 48f));
        SerializedObject h = new SerializedObject(hubUI);
        Assign(h, "mainStoryButton", main); Assign(h, "dailyButton", daily); Assign(h, "eliteButton", elite);
        Assign(h, "returnButton", hubReturn); Assign(h, "eliteStatusText", eliteStatus); h.ApplyModifiedPropertiesWithoutUndo();

        RectTransform dailyPanel = FindRectInScene(scene, "DailyChallengePanel");
        if (dailyPanel == null) { dailyPanel = CreateRect("DailyChallengePanel", parent); StretchToParent(dailyPanel); }
        DailyChallengeUI dailyUI = dailyPanel.GetComponent<DailyChallengeUI>();
        if (dailyUI == null) dailyUI = dailyPanel.gameObject.AddComponent<DailyChallengeUI>();
        GetOrCreateText(dailyPanel, "DailyTitle", "DAILY CHALLENGE", new Vector2(0f, 340f), new Vector2(390f, 55f), 28f);
        TMP_Text date = GetOrCreateText(dailyPanel, "DailyDate", "UTC Daily", new Vector2(0f, 285f), new Vector2(390f, 36f), 17f);
        TMP_Text attempts = GetOrCreateText(dailyPanel, "DailyAttempts", "Attempts: 3/3", new Vector2(0f, 235f), new Vector2(390f, 40f), 20f);
        TMP_Text enemy = GetOrCreateText(dailyPanel, "DailyEnemy", "Daily enemy", new Vector2(0f, 80f), new Vector2(390f, 190f), 19f);
        TMP_Text reward = GetOrCreateText(dailyPanel, "DailyReward", "Victory Reward", new Vector2(0f, -80f), new Vector2(390f, 100f), 18f);
        Button dailyStart = GetOrCreateButton(dailyPanel, "DailyStartButton", "Start Daily Battle", new Vector2(0f, -255f), new Vector2(280f, 54f));
        Button dailyReturn = GetOrCreateButton(dailyPanel, "ReturnFromDaily", "Back to Modes", new Vector2(0f, -340f), new Vector2(220f, 48f));
        SerializedObject d = new SerializedObject(dailyUI);
        Assign(d, "dateText", date); Assign(d, "attemptsText", attempts); Assign(d, "enemyText", enemy);
        Assign(d, "rewardText", reward); Assign(d, "startButton", dailyStart); Assign(d, "returnButton", dailyReturn); d.ApplyModifiedPropertiesWithoutUndo();

        PanelSwitcher switcher = FindComponentInScene<PanelSwitcher>(scene);
        SerializedObject s = new SerializedObject(switcher);
        Assign(s, "battleModePanel", hub.gameObject); Assign(s, "dailyChallengePanel", dailyPanel.gameObject); s.ApplyModifiedPropertiesWithoutUndo();
        GameManager manager = FindComponentInScene<GameManager>(scene);
        SerializedObject m = new SerializedObject(manager);
        Assign(m, "battleModeUI", hubUI); Assign(m, "dailyChallengeUI", dailyUI); m.ApplyModifiedPropertiesWithoutUndo();
        hub.gameObject.SetActive(false); dailyPanel.gameObject.SetActive(false);
    }

    private static void SetupEditBuildUI(Scene scene)
    {
        RectTransform meditationPanel = FindRectInScene(scene, "MeditationPanel");
        RectTransform battlePanel = FindRectInScene(scene, "BattlePanel");
        if (meditationPanel == null || battlePanel == null) return;

        RectTransform panel = FindRectInScene(scene, "EditBuildPanel");
        if (panel == null)
        {
            panel = CreateRect("EditBuildPanel", battlePanel.parent);
            StretchToParent(panel);
        }
        EditBuildUI ui = panel.GetComponent<EditBuildUI>();
        if (ui == null) ui = panel.gameObject.AddComponent<EditBuildUI>();

        GetOrCreateText(panel, "EditBuildTitle", "EDIT BUILD",
            new Vector2(0f, 385f), new Vector2(390f, 42f), 28f);
        Button equipmentTab = GetOrCreateButton(panel, "EquipmentTabButton", "Equipment",
            new Vector2(-130f, 335f), new Vector2(120f, 42f));
        Button ultimateTab = GetOrCreateButton(panel, "UltimateTabButton", "Ultimate",
            new Vector2(0f, 335f), new Vector2(120f, 42f));
        Button summaryTab = GetOrCreateButton(panel, "SummaryTabButton", "My Build",
            new Vector2(130f, 335f), new Vector2(120f, 42f));

        RectTransform equipmentPage = GetOrCreatePage(panel, "EditEquipmentPage");
        TMP_Text equipmentSummary = GetOrCreateText(equipmentPage, "EditEquipmentSummary",
            "Head choices", new Vector2(0f, 290f), new Vector2(390f, 34f), 15f);
        EquipmentSlot[] slots = (EquipmentSlot[])System.Enum.GetValues(typeof(EquipmentSlot));
        Button[] slotButtons = new Button[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            float x = -145f + (i % 4) * 97f;
            float y = 225f - (i / 4) * 82f;
            slotButtons[i] = GetOrCreateButton(equipmentPage,
                $"{slots[i]}SlotButton", slots[i].ToString(),
                new Vector2(x, y), new Vector2(88f, 70f));
        }
        Button[] choices = new Button[4];
        for (int i = 0; i < choices.Length; i++)
        {
            float x = i % 2 == 0 ? -100f : 100f;
            float y = 50f - (i / 2) * 120f;
            choices[i] = GetOrCreateButton(equipmentPage, $"EquipmentChoice{i + 1}",
                "Empty", new Vector2(x, y), new Vector2(185f, 100f));
        }
        TMP_Text equipmentDetail = GetOrCreateText(equipmentPage, "EquipmentInstanceDetail",
            "Select an equipment instance", new Vector2(0f, -175f), new Vector2(390f, 105f), 13f);
        Button equipSelected = GetOrCreateButton(equipmentPage, "EquipSelectedInstanceButton",
            "Equip Selected", new Vector2(-100f, -250f), new Vector2(185f, 44f));
        Button lockSelected = GetOrCreateButton(equipmentPage, "LockSelectedInstanceButton",
            "Lock / Unlock", new Vector2(100f, -250f), new Vector2(185f, 44f));
        Button upgradeSelected = GetOrCreateButton(equipmentPage, "UpgradeSelectedInstanceButton",
            "Upgrade", new Vector2(-100f, -305f), new Vector2(185f, 44f));
        Button dismantleSelected = GetOrCreateButton(equipmentPage, "DismantleSelectedInstanceButton",
            "Dismantle", new Vector2(100f, -305f), new Vector2(185f, 44f));

        RectTransform ultimatePage = GetOrCreatePage(panel, "EditUltimatePage");
        Button[] ultimateButtons = new Button[5];
        string[] ultimateLabels = { "Star Burst", "Iron Retaliation", "Rapid Nova", "Meteor Flurry", "Swift Ascension" };
        for (int i = 0; i < ultimateButtons.Length; i++)
        {
            ultimateButtons[i] = GetOrCreateButton(ultimatePage, $"EditUltimate{i + 1}Button",
                ultimateLabels[i], new Vector2(0f, 250f - i * 105f), new Vector2(390f, 90f));
        }

        RectTransform summaryPage = GetOrCreatePage(panel, "EditSummaryPage");
        TMP_Text buildSummary = GetOrCreateText(summaryPage, "EditBuildSummaryText", "Build summary",
            new Vector2(0f, 5f), new Vector2(390f, 590f), 16f);
        buildSummary.alignment = TextAlignmentOptions.TopLeft;
        Button returnButton = GetOrCreateButton(panel, "ReturnFromEditBuildButton", "Return",
            new Vector2(0f, -395f), new Vector2(220f, 46f));

        SerializedObject serialized = new SerializedObject(ui);
        Assign(serialized, "equipmentPage", equipmentPage.gameObject);
        Assign(serialized, "ultimatePage", ultimatePage.gameObject);
        Assign(serialized, "summaryPage", summaryPage.gameObject);
        Assign(serialized, "equipmentTabButton", equipmentTab);
        Assign(serialized, "ultimateTabButton", ultimateTab);
        Assign(serialized, "summaryTabButton", summaryTab);
        Assign(serialized, "returnButton", returnButton);
        Assign(serialized, "equipmentSummaryText", equipmentSummary);
        Assign(serialized, "buildSummaryText", buildSummary);
        AssignArray(serialized, "slotButtons", slotButtons);
        AssignArray(serialized, "equipmentChoiceButtons", choices);
        AssignArray(serialized, "ultimateButtons", ultimateButtons);
        Assign(serialized, "equipmentDetailText", equipmentDetail);
        Assign(serialized, "equipSelectedButton", equipSelected);
        Assign(serialized, "lockSelectedButton", lockSelected);
        Assign(serialized, "upgradeSelectedButton", upgradeSelected);
        Assign(serialized, "dismantleSelectedButton", dismantleSelected);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        Button nav = GetOrCreateButton(meditationPanel, "GoToEditBuildButton", "Edit Build",
            new Vector2(0f, -370f), new Vector2(390f, 48f));
        RectTransform legacyUltimate = FindChildByName(meditationPanel, "GoToUltimateButton");
        RectTransform legacyEquipment = FindChildByName(meditationPanel, "GoToEquipmentButton");
        if (legacyUltimate != null) legacyUltimate.gameObject.SetActive(false);
        if (legacyEquipment != null) legacyEquipment.gameObject.SetActive(false);

        PanelSwitcher switcher = FindComponentInScene<PanelSwitcher>(scene);
        SerializedObject switcherSerialized = new SerializedObject(switcher);
        Assign(switcherSerialized, "editBuildPanel", panel.gameObject);
        Assign(switcherSerialized, "goToEditBuildButton", nav);
        switcherSerialized.ApplyModifiedPropertiesWithoutUndo();
        GameManager manager = FindComponentInScene<GameManager>(scene);
        SerializedObject managerSerialized = new SerializedObject(manager);
        Assign(managerSerialized, "editBuildUI", ui);
        managerSerialized.ApplyModifiedPropertiesWithoutUndo();
        panel.gameObject.SetActive(false);
    }

    private static RectTransform GetOrCreatePage(Transform parent, string name)
    {
        RectTransform page = FindChildByName(parent, name);
        if (page == null) page = CreateRect(name, parent);
        StretchToParent(page);
        page.offsetMin = new Vector2(0f, 70f);
        page.offsetMax = new Vector2(0f, -130f);
        return page;
    }

    private static void SetupFeedbackUI(Scene scene)
    {
        Canvas canvas = FindComponentInScene<Canvas>(scene);
        if (canvas == null) return;
        RectTransform safeArea = FindChildByName(canvas.transform, "SafeArea");
        Transform parent = safeArea != null ? safeArea : canvas.transform;
        RectTransform root = FindChildByName(parent, "GameFeedbackLayer");
        if (root == null) root = CreateRect("GameFeedbackLayer", parent);
        StretchToParent(root);
        GameFeedbackUI ui = root.GetComponent<GameFeedbackUI>();
        if (ui == null) ui = root.gameObject.AddComponent<GameFeedbackUI>();

        RectTransform toast = FindChildByName(root, "FeedbackToast");
        if (toast == null)
        {
            toast = CreateRect("FeedbackToast", root);
            Image image = toast.gameObject.AddComponent<Image>();
            image.color = new Color(0.04f, 0.07f, 0.12f, 0.58f);
        }
        Image toastBackground = toast.GetComponent<Image>();
        if (toastBackground != null)
            toastBackground.color = new Color(0.04f, 0.07f, 0.12f, 0.58f);
        SetRect(toast, 0f, 155f, 380f, 34f);
        TMP_Text toastText = GetOrCreateText(toast, "FeedbackToastText", "Reward feedback",
            Vector2.zero, new Vector2(368f, 30f), 14f);

        RectTransform result = FindChildByName(root, "BattleResultPanel");
        if (result == null)
        {
            result = CreateRect("BattleResultPanel", root);
            Image image = result.gameObject.AddComponent<Image>();
            image.color = new Color(0.025f, 0.035f, 0.065f, 0.985f);
        }
        SetRect(result, 0f, 0f, 400f, 520f);
        TMP_Text title = GetOrCreateText(result, "BattleResultTitle", "VICTORY",
            new Vector2(0f, 185f), new Vector2(360f, 56f), 30f);
        TMP_Text body = GetOrCreateText(result, "BattleResultBody", "Rewards",
            new Vector2(0f, 20f), new Vector2(350f, 250f), 18f);
        Button continueButton = GetOrCreateButton(result, "BattleResultContinueButton", "Continue",
            new Vector2(0f, -190f), new Vector2(220f, 52f));

        SerializedObject serialized = new SerializedObject(ui);
        Assign(serialized, "toastPanel", toast.gameObject);
        Assign(serialized, "toastText", toastText);
        Assign(serialized, "resultPanel", result.gameObject);
        Assign(serialized, "resultTitleText", title);
        Assign(serialized, "resultBodyText", body);
        Assign(serialized, "resultContinueButton", continueButton);
        serialized.ApplyModifiedPropertiesWithoutUndo();
        GameManager manager = FindComponentInScene<GameManager>(scene);
        SerializedObject managerSerialized = new SerializedObject(manager);
        Assign(managerSerialized, "gameFeedbackUI", ui);
        managerSerialized.ApplyModifiedPropertiesWithoutUndo();
        toast.gameObject.SetActive(false);
        result.gameObject.SetActive(false);
        root.SetAsLastSibling();
    }

    private static void SetupCombatLog(
        RectTransform hud,
        SerializedObject battleSerialized)
    {
        Button toggle = GetOrCreateButton(hud, "CombatLogToggleButton", "Combat Log ▲",
            new Vector2(-125f, -385f), new Vector2(160f, 38f));
        RectTransform panel = FindChildByName(hud, "CombatLogPanel");
        if (panel == null)
        {
            panel = CreateRect("CombatLogPanel", hud);
            Image background = panel.gameObject.AddComponent<Image>();
            background.color = new Color(0.025f, 0.035f, 0.065f, 0.96f);
        }
        SetRect(panel, 0f, -260f, 400f, 210f);
        TMP_Text text = GetOrCreateText(panel, "CombatLogText", "Combat log",
            Vector2.zero, new Vector2(370f, 180f), 14f);
        text.alignment = TextAlignmentOptions.BottomLeft;
        text.textWrappingMode = TextWrappingModes.Normal;

        Assign(battleSerialized, "combatLogToggleButton", toggle);
        Assign(battleSerialized, "combatLogPanel", panel.gameObject);
        Assign(battleSerialized, "combatLogText", text);
        panel.gameObject.SetActive(false);
        toggle.gameObject.SetActive(false);
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

        SetRect(child, x, y, width, height);
    }

    private static void SetTextStyle(
        RectTransform parent,
        string childName,
        float fontSize,
        bool autoSize)
    {
        RectTransform rect = FindChildByName(parent, childName);
        TMP_Text text = rect != null ? rect.GetComponent<TMP_Text>() : null;
        if (text == null) return;
        text.fontSize = fontSize;
        text.enableAutoSizing = autoSize;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.alignment = TextAlignmentOptions.Center;
    }

    private static void SetButtonLabel(
        RectTransform parent,
        string buttonName,
        string value,
        float fontSize)
    {
        RectTransform rect = FindChildByName(parent, buttonName);
        TMP_Text label = rect != null
            ? rect.GetComponentInChildren<TMP_Text>(true)
            : null;
        if (label != null)
        {
            label.text = value;
            label.fontSize = fontSize;
            label.enableAutoSizing = false;
            return;
        }
        Text legacy = rect != null ? rect.GetComponentInChildren<Text>(true) : null;
        if (legacy != null)
        {
            legacy.text = value;
            legacy.fontSize = Mathf.RoundToInt(fontSize);
            legacy.resizeTextForBestFit = false;
        }
    }

    private static void NormalizeMeditationText(
        RectTransform panel,
        string childName,
        float fontSize)
    {
        RectTransform rect = FindChildByName(panel, childName);
        TMP_Text text = rect != null ? rect.GetComponent<TMP_Text>() : null;
        if (text == null) return;

        text.fontSize = fontSize;
        text.enableAutoSizing = false;
        text.fontStyle = FontStyles.Normal;
        text.fontWeight = FontWeight.Regular;
        text.alignment = TextAlignmentOptions.Center;
        TMP_FontAsset defaultFont = TMP_Settings.defaultFontAsset;
        if (defaultFont != null)
        {
            text.font = defaultFont;
            text.fontSharedMaterial = defaultFont.material;
        }
        if (childName == "LevelText") text.text = "Level: 1";
        if (childName == "ExpText") text.text = "EXP: 0 / 100";
        EditorUtility.SetDirty(text);
    }

    private static void NormalizeMeditateButton(RectTransform panel)
    {
        RectTransform buttonRect = FindChildByName(panel, "MeditateButton");
        if (buttonRect == null) return;

        Image image = buttonRect.GetComponent<Image>();
        if (image != null) image.preserveAspect = true;

        Text legacyLabel = buttonRect.GetComponentInChildren<Text>(true);
        if (legacyLabel != null)
        {
            legacyLabel.text = "Meditate";
            legacyLabel.fontSize = 22;
            legacyLabel.resizeTextForBestFit = false;
            legacyLabel.alignment = TextAnchor.MiddleCenter;
            legacyLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
            legacyLabel.verticalOverflow = VerticalWrapMode.Overflow;
            legacyLabel.color = new Color(0.15f, 0.15f, 0.18f, 1f);
            EditorUtility.SetDirty(legacyLabel);
        }
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

    private static RectTransform GetOrCreateTimelineMarker(
        Transform parent, string name, string label, Color color, float y)
    {
        RectTransform marker = FindChildByName(parent, name);
        if (marker == null)
        {
            marker = CreateRect(name, parent);
            Image image = marker.gameObject.AddComponent<Image>();
            image.color = color;
        }
        marker.anchorMin = marker.anchorMax = new Vector2(0f, 0.5f);
        marker.pivot = new Vector2(0.5f, 0.5f);
        marker.anchoredPosition = new Vector2(0f, y);
        marker.sizeDelta = new Vector2(32f, 32f);
        TMP_Text text = GetOrCreateText(marker, "Label", label, Vector2.zero,
            new Vector2(32f, 32f), 16f);
        text.fontStyle = FontStyles.Bold;
        return marker;
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

    private static void AssignArray<T>(SerializedObject target, string property, T[] values)
        where T : Object
    {
        SerializedProperty serializedProperty = target.FindProperty(property);
        if (serializedProperty == null)
        {
            Debug.LogError($"Serialized property '{property}' was not found.");
            return;
        }
        serializedProperty.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
            serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
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
