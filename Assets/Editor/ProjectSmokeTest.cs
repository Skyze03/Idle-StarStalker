using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ProjectSmokeTest
{
    private const string TargetScene = "Assets/Scenes/MainScene_Rebuild.unity";
    private const string PhaseKey = "IdleStarStalker.SmokeTest.Phase";
    private const string ResultKey = "IdleStarStalker.SmokeTest.Result";

    static ProjectSmokeTest()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.delayCall += ResumeBatchRun;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorApplication.delayCall += ResumeBatchRun;
        }
    }

    public static void RunBatch()
    {
        SessionState.SetString(PhaseKey, "entering");
        SessionState.SetString(ResultKey, string.Empty);
        EditorSceneManager.OpenScene(TargetScene, OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
    }

    private static void ResumeBatchRun()
    {
        string phase = SessionState.GetString(PhaseKey, string.Empty);

        if (phase == "entering" && EditorApplication.isPlaying)
        {
            EditorApplication.delayCall += RunAssertions;
            return;
        }

        if (phase == "exiting" && !EditorApplication.isPlaying)
        {
            string result = SessionState.GetString(ResultKey, "Unknown failure");
            bool success = result == "PASS";

            SessionState.EraseString(PhaseKey);
            SessionState.EraseString(ResultKey);

            if (success)
            {
                Debug.Log("PROJECT SMOKE TEST: PASS");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("PROJECT SMOKE TEST: FAIL - " + result);
                EditorApplication.Exit(1);
            }
        }
    }

    private static void RunAssertions()
    {
        try
        {
            GameManager gameManager = UnityEngine.Object.FindFirstObjectByType<GameManager>(
                FindObjectsInactive.Include
            );
            Require(gameManager != null, "GameManager was not initialized.");

            MainStageSystem stages = gameManager.MainStageSystemRef;
            MainStageState stageState = gameManager.MainStageStateRef;
            BattleSystem battle = UnityEngine.Object.FindFirstObjectByType<BattleSystem>(
                FindObjectsInactive.Include
            );
            UpgradeSystem upgrades = UnityEngine.Object.FindFirstObjectByType<UpgradeSystem>(
                FindObjectsInactive.Include
            );
            PanelSwitcher panels = UnityEngine.Object.FindFirstObjectByType<PanelSwitcher>(
                FindObjectsInactive.Include
            );
            UltimateSystem ultimates = gameManager.UltimateSystemRef;
            EquipmentSystem equipment = gameManager.EquipmentSystemRef;
            MeditationSystem meditation = UnityEngine.Object.FindFirstObjectByType<MeditationSystem>(
                FindObjectsInactive.Include);
            CollectionSystem collection = UnityEngine.Object.FindFirstObjectByType<CollectionSystem>(
                FindObjectsInactive.Include);
            GameFeedbackUI feedback = UnityEngine.Object.FindFirstObjectByType<GameFeedbackUI>(
                FindObjectsInactive.Include);
            EditBuildUI editBuild = UnityEngine.Object.FindFirstObjectByType<EditBuildUI>(
                FindObjectsInactive.Include);

            Require(stages != null, "MainStageSystem was not initialized.");
            Require(stageState != null, "MainStageState was not initialized.");
            Require(battle != null, "BattleSystem was not initialized.");
            Require(upgrades != null, "UpgradeSystem was not found.");
            Require(panels != null, "PanelSwitcher was not found.");
            Require(ultimates != null, "UltimateSystem was not initialized.");
            Require(equipment != null, "EquipmentSystem was not initialized.");
            Require(meditation != null && collection != null && feedback != null,
                "Unified action feedback was not initialized.");
            Require(editBuild != null, "Edit Build UI was not initialized.");

            meditation.StartMeditation();
            Require(feedback.IsToastVisible && feedback.CurrentToast.Contains("EXP"),
                "Meditation did not show gain feedback.");
            collection.CollectOnce();
            Require(feedback.IsToastVisible && feedback.CurrentToast.Contains("Energy"),
                "Collection did not show gain feedback.");
            Require(feedback.ActiveToastCount == 2,
                "Consecutive gain messages replaced each other instead of stacking.");
            RectTransform toastTemplate = FindGameObject("FeedbackToast").GetComponent<RectTransform>();
            Require(toastTemplate.sizeDelta == new Vector2(380f, 34f),
                "Gain feedback did not preserve width while reducing vertical height.");
            var toastInstances = new System.Collections.Generic.List<RectTransform>();
            foreach (GameObject candidate in Resources.FindObjectsOfTypeAll<GameObject>())
                if (candidate.name == "FeedbackToastInstance" && candidate.scene.IsValid())
                    toastInstances.Add(candidate.GetComponent<RectTransform>());
            float toastOriginY = FindGameObject("FeedbackToast")
                .GetComponent<RectTransform>().anchoredPosition.y;
            Require(toastInstances.Count == 2 &&
                toastInstances.TrueForAll(rect => rect.anchoredPosition.y >= toastOriginY),
                "New gain feedback spawned progressively lower on the screen.");

            GameObject editBuildNav = FindGameObject("GoToEditBuildButton");
            Require(editBuildNav != null && editBuildNav.activeSelf,
                "The unified Edit Build navigation button was missing.");
            editBuildNav.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            Require(editBuild.gameObject.activeSelf,
                "Edit Build page did not open.");
            editBuild.ShowUltimatePage();
            Require(FindGameObject("EditUltimatePage").activeSelf,
                "Edit Build Ultimate tab did not open.");
            editBuild.ShowSummaryPage();
            Require(FindGameObject("EditSummaryPage").activeSelf,
                "Edit Build My Build tab did not open.");
            panels.ShowMeditationPanel();

            GameObject timeline = FindGameObject("SharedActionTimeline");
            GameObject playerMarker = FindGameObject("PlayerTimelineMarker");
            GameObject enemyMarker = FindGameObject("EnemyTimelineMarker");
            Require(timeline != null && !timeline.activeSelf &&
                playerMarker != null && enemyMarker != null,
                "Battle preview did not hide the Shared Action Timeline.");
            Require(!FindGameObject("PlayerActionSlider").activeSelf &&
                !FindGameObject("EnemyActionSlider").activeSelf,
                "Legacy separate Action sliders remained visible.");
            Require(equipment.OwnedCount == 0,
                "A new character incorrectly started with all prototype equipment.");
            Require(stages.GetEnemyUltimateId(1) == "star_burst" &&
                stages.GetEnemyUltimateId(5) == "iron_retaliation" &&
                stages.GetEnemyUltimateId(10) == "rapid_nova" &&
                stages.GetEnemyUltimateId(15) == "meteor_flurry" &&
                stages.GetEnemyUltimateId(20) == "swift_ascension",
                "Enemy Ultimate stage distribution was incorrect.");
            Require(stages.GetEnemyTraits(1) == EnemyTrait.None &&
                stages.GetEnemyTraits(2) == EnemyTrait.Frenzy &&
                stages.GetEnemyTraits(3) == EnemyTrait.Bulwark &&
                stages.GetEnemyTraits(4) == EnemyTrait.Swift &&
                stages.GetEnemyTraits(5) ==
                    (EnemyTrait.Frenzy | EnemyTrait.Bulwark) &&
                stages.GetEnemyTraits(10) ==
                    (EnemyTrait.Swift | EnemyTrait.Sage) &&
                stages.GetEnemyTraits(20) ==
                    (EnemyTrait.Bulwark | EnemyTrait.Sage),
                "Enemy trait stage distribution was incorrect.");
            Require(ultimates.IsUnlocked("star_burst"),
                "Starter Ultimate was not unlocked.");
            Require(!ultimates.Equip("meteor_flurry"),
                "A locked Ultimate could be equipped.");
            Require(stageState.battleStamina == MainStageSystem.MaxStamina,
                "New game stamina was not full.");

            RectTransform meditateRect = FindGameObject("MeditateButton")
                ?.GetComponent<RectTransform>();
            RectTransform expSliderRect = FindGameObject("ExpSlider")
                ?.GetComponent<RectTransform>();
            RectTransform autoRect = FindGameObject("AutoMeditateButton")
                ?.GetComponent<RectTransform>();
            Require(meditateRect != null && expSliderRect != null && autoRect != null,
                "Meditation controls were not found.");
            Require(meditateRect.localScale == Vector3.one &&
                expSliderRect.localScale == Vector3.one,
                "Meditation controls retained legacy local scaling.");
            Require(meditateRect.sizeDelta.x == meditateRect.sizeDelta.y &&
                meditateRect.sizeDelta.x == 140f,
                "Meditate button was not restored to a circular square layout.");
            Require(expSliderRect.sizeDelta == new Vector2(320f, 22f),
                "EXP Slider did not use the portrait layout size.");
            Require(Mathf.Abs(expSliderRect.anchoredPosition.y -
                autoRect.anchoredPosition.y) > 50f,
                "EXP Slider overlaps the Auto Meditation button.");
            UnityEngine.UI.Text meditateLabel =
                meditateRect.GetComponentInChildren<UnityEngine.UI.Text>(true);
            Require(meditateLabel != null && meditateLabel.text == "Meditate",
                "Meditate button label was missing.");
            MeditationUI meditationView =
                UnityEngine.Object.FindFirstObjectByType<MeditationUI>(
                    FindObjectsInactive.Include
                );
            Require(meditationView != null, "MeditationUI was not found.");
            foreach (TMPro.TMP_Text text in
                meditationView.GetComponentsInChildren<TMPro.TMP_Text>(true))
            {
                if (text.name == "LevelText" || text.name == "ExpText")
                    Require(text.font == TMPro.TMP_Settings.defaultFontAsset,
                        $"{text.name} did not use the default English TMP font.");
            }

            PlayerData player = gameManager.PlayerDataRef;
            player.energy = 100;
            int originalWeaponLevel = player.weaponLevel;

            Require(!upgrades.TryUpgradePart(BodyPartType.Weapon),
                "A body part exceeded Player Level 1.");
            Require(player.weaponLevel == originalWeaponLevel,
                "Blocked upgrade changed the weapon level.");

            player.level = 2;
            Require(upgrades.TryUpgradePart(BodyPartType.Weapon),
                "Weapon did not upgrade after Player Level increased.");
            Require(feedback.CurrentToast.Contains("Weapon upgraded"),
                "Successful Upgrade did not use unified action feedback.");
            gameManager.RefreshAllUI();
            RectTransform weaponRow = FindGameObject("WeaponRow").GetComponent<RectTransform>();
            RectTransform headRow = FindGameObject("HeadRow").GetComponent<RectTransform>();
            RectTransform armsRow = FindGameObject("ArmsRow").GetComponent<RectTransform>();
            Require(weaponRow.anchoredPosition.y == headRow.anchoredPosition.y &&
                weaponRow.anchoredPosition.x < headRow.anchoredPosition.x &&
                armsRow.anchoredPosition.y < weaponRow.anchoredPosition.y,
                "Upgrade parts were not arranged Weapon/Head first in a two-column grid.");
            foreach (TMPro.TMP_Text text in weaponRow.GetComponentsInChildren<TMPro.TMP_Text>(true))
                if (text.name == "BodyLevelText")
                    Require(text.textWrappingMode == TMPro.TextWrappingModes.NoWrap,
                        "Upgrade progression text still allowed mid-word wrapping.");
            bool weaponProgressVisible = false;
            foreach (TMPro.TMP_Text text in weaponRow.GetComponentsInChildren<TMPro.TMP_Text>(true))
                if (text.text.Contains("Attack") && text.text.Contains("→"))
                    weaponProgressVisible = true;
            Require(weaponProgressVisible,
                "Weapon upgrade card did not show current-to-next stat progression.");
            TMPro.TMP_Text weaponCost = FindGameObject("WeaponRow")
                .GetComponent<UpgradeRowUI>()
                .GetComponentInChildren<UnityEngine.UI.Button>(true)
                .GetComponentInChildren<TMPro.TMP_Text>(true);
            Require(weaponCost != null && !weaponCost.text.Contains("Upgrade"),
                "Upgrade button still used a generic Upgrade label.");

            panels.ShowBattlePanel();
            gameManager.RefreshAllUI();
            GameObject saveButton = FindGameObject("SaveButton");
            GameObject loadButton = FindGameObject("LoadButton");
            Require(saveButton != null && loadButton != null &&
                !saveButton.activeSelf && !loadButton.activeSelf,
                "Save/Load remained visible on the Battle page before combat.");
            int energyBeforeBattle = player.energy;
            Require(stages.TryStartSelectedStage(), "Stage 1 did not start.");
            gameManager.RefreshAllUI();
            Require(timeline.activeSelf &&
                FindGameObject("PlayerHPSlider").activeSelf &&
                !FindGameObject("StartBattleButton").activeSelf,
                "Starting battle did not switch from preview to combat HUD.");
            Require(FindGameObject("PlayerNameText").GetComponent<RectTransform>().anchoredPosition.x < 0f &&
                FindGameObject("EnemyNameText").GetComponent<RectTransform>().anchoredPosition.x > 0f &&
                timeline.GetComponent<RectTransform>().anchoredPosition.y < -250f,
                "Combat HUD was not arranged player-left/enemy-right with a low timeline.");
            GameObject logToggle = FindGameObject("CombatLogToggleButton");
            GameObject logPanel = FindGameObject("CombatLogPanel");
            Require(logToggle.activeSelf && !logPanel.activeSelf &&
                !FindGameObject("BattleStatusText").activeSelf &&
                !FindGameObject("StageFeedbackText").activeSelf,
                "Combat status/log remained exposed instead of using the collapsed log.");
            logToggle.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            Require(logPanel.activeSelf &&
                FindGameObject("CombatLogText").GetComponent<TMPro.TMP_Text>().text.Length > 0,
                "Combat Log did not expand with battle details.");
            logToggle.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            Require(!logPanel.activeSelf, "Combat Log did not collapse.");
            Require(saveButton != null && loadButton != null &&
                !saveButton.activeSelf && !loadButton.activeSelf,
                "Save/Load remained visible during battle.");
            battle.BattleState.playerActionValue = 25f;
            battle.BattleState.enemyActionValue = 75f;
            gameManager.RefreshAllUI();
            Require(Mathf.Approximately(
                    playerMarker.GetComponent<RectTransform>().anchorMin.x, 0.25f) &&
                Mathf.Approximately(
                    enemyMarker.GetComponent<RectTransform>().anchorMin.x, 0.75f),
                "Shared Action Timeline markers did not reflect combat progress.");
            battle.BattleState.playerActionValue = 0f;
            battle.BattleState.enemyActionValue = 0f;

            panels.ShowMeditationPanel();
            GameObject battlePanel = FindGameObject("BattlePanel");
            GameObject meditationPanel = FindGameObject("MeditationPanel");
            Require(battlePanel != null && battlePanel.activeSelf,
                "Battle panel was not kept active during battle.");
            Require(meditationPanel != null && !meditationPanel.activeSelf,
                "Meditation panel opened during battle.");

            RunBattleToCompletion(battle);
            gameManager.RefreshAllUI();
            Require(feedback.IsResultVisible &&
                feedback.CurrentResult.Contains("Stage 1 cleared") &&
                feedback.CurrentResult.Contains("Star Blade"),
                "Battle result panel omitted first-clear reward feedback.");
            FindGameObject("BattleResultContinueButton")
                .GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            Require(!feedback.IsResultVisible,
                "Battle result panel did not close.");
            Require(!saveButton.activeSelf && !loadButton.activeSelf,
                "Save/Load appeared while the Battle page remained open.");
            panels.ShowMeditationPanel();
            gameManager.RefreshAllUI();
            Require(saveButton.activeSelf && loadButton.activeSelf,
                "Save/Load did not return after leaving the Battle page.");

            Require(battle.BattleState.battleResult == BattleResult.Victory,
                "Stage 1 did not end in Victory.");
            Require(stageState.highestClearedStage == 1,
                "Stage 1 was not recorded as cleared.");
            Require(stageState.highestUnlockedStage == 2,
                "Stage 2 was not unlocked.");
            Require(stageState.selectedStage == 2,
                "The next unlocked stage was not selected.");
            Require(stageState.battleStamina == 19,
                "Stage challenge did not consume exactly 1 stamina.");
            Require(player.energy == energyBeforeBattle + 60,
                "Stage 1 normal + first-clear Energy reward was incorrect.");
            Require(equipment.IsOwned("star_blade") &&
                equipment.OwnedCount >= 1 && equipment.OwnedCount <= 2,
                "Stage 1 first clear did not grant its fixed equipment correctly.");

            int energyBeforeSweep = player.energy;
            int equipmentBeforeSweep = equipment.OwnedCount;
            Require(stages.TrySweepStage(1), "Cleared Stage 1 could not be swept.");
            Require(stageState.battleStamina == 18,
                "Sweep did not consume exactly 1 stamina.");
            Require(player.energy == energyBeforeSweep + 10,
                "Stage 1 sweep reward was incorrect.");
            Require(feedback.IsToastVisible && feedback.CurrentToast.Contains("Sweep"),
                "Sweep did not show reward feedback.");
            Require(equipment.OwnedCount >= equipmentBeforeSweep &&
                equipment.OwnedCount <= equipmentBeforeSweep + 1,
                "Sweep equipment drop count exceeded its single-drop rule.");

            string deterministicDrop = stages.RollEquipmentDrop(
                5, MainStageSystem.BattleEquipmentDropChance, 0.199f);
            Require(!string.IsNullOrEmpty(deterministicDrop) &&
                deterministicDrop == EquipmentSystem.GetStageEquipmentId(5) &&
                string.IsNullOrEmpty(stages.RollEquipmentDrop(
                    5, MainStageSystem.BattleEquipmentDropChance, 0.2f)),
                "Equipment drop chance or current-and-earlier-stage pool was incorrect.");
            RewardBundle multiEquipmentReward = new RewardBundle();
            multiEquipmentReward.AddEquipment("star_blade");
            multiEquipmentReward.AddEquipment("seer_circlet");
            Require(multiEquipmentReward.equipmentTemplateIds.Length == 2,
                "A reward could not contain fixed and random equipment together.");

            DailyChallengeSystem daily = gameManager.DailyChallengeSystemRef;
            Require(daily != null && daily.State.remainingAttempts == 3,
                "Daily Challenge did not initialize with three attempts.");
            panels.ShowBattleModePanel();
            Require(FindGameObject("BattleModePanel").activeSelf,
                "Battle navigation did not open the Battle Mode Hub.");
            panels.ShowDailyChallengePanel();
            gameManager.RefreshAllUI();
            Require(FindGameObject("DailyChallengePanel").activeSelf &&
                FindGameObject("DailyAttempts").GetComponent<TMPro.TMP_Text>().text.Contains("3/3"),
                "Daily Challenge preview did not show today's attempts.");
            Require(daily.TryStart(), "Daily Challenge did not start.");
            panels.ShowBattlePanel();
            RunBattleToCompletion(battle);
            gameManager.RefreshAllUI();
            Require(daily.State.remainingAttempts == 2,
                "Daily Challenge did not consume exactly one attempt.");
            Require(FindGameObject("DailyChallengePanel").activeSelf && feedback.IsResultVisible,
                "Daily Challenge did not return to its preview with a result panel.");
            FindGameObject("BattleResultContinueButton")
                .GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            stages.RefreshAfterLoad();

            var mappedDrops = new System.Collections.Generic.HashSet<string>();
            for (int stage = 1; stage <= 10; stage++)
                mappedDrops.Add(stages.GetEquipmentUnlockForStage(stage));
            Require(mappedDrops.Count == 10 &&
                string.IsNullOrEmpty(stages.GetEquipmentUnlockForStage(11)),
                "Prototype equipment drops were not uniquely mapped to Stages 1-10.");

            TestUltimatePrototype(gameManager, battle, ultimates);
            TestEquipmentPrototype(gameManager, battle, equipment);

            stageState.battleStamina = 10;
            stageState.lastStaminaRefreshUtcTicks = DateTime.UtcNow
                .AddSeconds(-(MainStageSystem.StaminaRecoverySeconds * 2 + 1))
                .Ticks;
            stages.Tick();
            Require(stageState.battleStamina == 12,
                "Offline stamina recovery did not restore 2 stamina.");

            TestPrototypeCompletion(gameManager, stages, battle);

            SessionState.SetString(ResultKey, "PASS");
        }
        catch (Exception exception)
        {
            SessionState.SetString(ResultKey, exception.Message);
            Debug.LogException(exception);
        }
        finally
        {
            SessionState.SetString(PhaseKey, "exiting");
            EditorApplication.isPlaying = false;
        }
    }

    private static void RunBattleToCompletion(BattleSystem battle)
    {
        const int maxTicks = 5000;

        for (int i = 0; i < maxTicks && battle.BattleState.battleRunning; i++)
        {
            battle.Tick(1f);
        }

        Require(!battle.BattleState.battleRunning,
            "Battle did not finish within the smoke-test tick limit.");
    }

    private static void TestUltimatePrototype(
        GameManager gameManager,
        BattleSystem battle,
        UltimateSystem ultimates)
    {
        PlayerData player = gameManager.PlayerDataRef;
        EnemyData enemy = gameManager.EnemyDataRef;

        string[] unlocks =
        {
            "iron_retaliation", "rapid_nova", "meteor_flurry", "swift_ascension"
        };
        foreach (string id in unlocks)
            Require(ultimates.Unlock(id), $"Could not unlock {id}.");

        SaveData saveRoundTrip = new SaveData
        {
            unlockedUltimateIds = player.unlockedUltimateIds,
            equippedUltimateId = "meteor_flurry"
        };
        SaveData restored = JsonUtility.FromJson<SaveData>(
            JsonUtility.ToJson(saveRoundTrip)
        );
        Require(restored.unlockedUltimateIds.Length == 5 &&
            restored.equippedUltimateId == "meteor_flurry",
            "Ultimate save fields did not survive JSON serialization.");

        enemy.maxHP = 10000;
        enemy.attack = 1;
        enemy.defense = 2;
        enemy.agility = 1;
        enemy.traits = EnemyTrait.Frenzy | EnemyTrait.Bulwark;
        int normalDamage = Mathf.Max(1, player.stats.attack - enemy.defense);

        TestDamageUltimate(battle, ultimates, enemy, "star_burst",
            Mathf.RoundToInt(normalDamage * 2.5f), 0f);
        TestDamageUltimate(battle, ultimates, enemy, "iron_retaliation",
            normalDamage + Mathf.RoundToInt(player.stats.defense * 1.8f), 0f);
        TestDamageUltimate(battle, ultimates, enemy, "rapid_nova",
            Mathf.RoundToInt(normalDamage * 1.8f), 40f);
        TestDamageUltimate(battle, ultimates, enemy, "meteor_flurry",
            Mathf.RoundToInt(normalDamage * 0.9f) * 3, 0f);

        Require(ultimates.Equip("swift_ascension"),
            "Swift Ascension could not be equipped.");
        battle.ResetBattle();
        Require(battle.StartBattle(), "Swift Ascension test battle did not start.");
        int hpBefore = battle.BattleState.enemyCurrentHP;
        battle.BattleState.playerRage = BattleSystem.RageThreshold;
        battle.Tick(4f);
        Require(battle.BattleState.enemyCurrentHP == hpBefore,
            "Swift Ascension incorrectly dealt damage.");
        Require(battle.BattleState.playerAgilityBuffActions == 3,
            "Swift Ascension did not grant a three-action Agility buff.");
        battle.ResetBattle();
        ultimates.Equip("star_burst");
    }

    private static void TestDamageUltimate(
        BattleSystem battle,
        UltimateSystem ultimates,
        EnemyData enemy,
        string ultimateId,
        int expectedDamage,
        float expectedRage)
    {
        Require(ultimates.Equip(ultimateId), $"Could not equip {ultimateId}.");
        battle.ResetBattle();
        Require(battle.StartBattle(), $"{ultimateId} test battle did not start.");
        string differentUltimate = ultimateId == "star_burst"
            ? "iron_retaliation"
            : "star_burst";
        Require(!ultimates.Equip(differentUltimate),
            "Ultimate changed after the battle Build was locked.");
        Require(battle.PlayerBuild.ultimate.id == ultimateId,
            "Battle snapshot did not preserve the selected Ultimate.");
        int hpBefore = battle.BattleState.enemyCurrentHP;
        battle.BattleState.playerRage = BattleSystem.RageThreshold;
        battle.Tick(4f);
        Require(hpBefore - battle.BattleState.enemyCurrentHP == expectedDamage,
            $"{ultimateId} damage was incorrect.");
        Require(Mathf.Approximately(battle.BattleState.playerRage, expectedRage),
            $"{ultimateId} post-cast Rage was incorrect.");
        battle.ResetBattle();
    }

    private static void TestEquipmentPrototype(
        GameManager gameManager,
        BattleSystem battle,
        EquipmentSystem equipment)
    {
        foreach (EquipmentData item in EquipmentData.GetAll())
            if (!equipment.IsOwned(item.id)) equipment.Unlock(item.id);

        int countAfterCatalog = equipment.OwnedCount;
        EquipmentInstance duplicate = equipment.GrantInstance("star_blade");
        Require(duplicate != null && equipment.OwnedCount == countAfterCatalog + 1 &&
            equipment.GetInstancesForSlot(EquipmentSlot.Weapon).Count >= 3,
            "Duplicate equipment was not stored as an independent instance.");
        Require(equipment.ToggleLock(duplicate.instanceId) && duplicate.locked,
            "Equipment instance lock state did not change.");
        Require(equipment.EquipInstance(EquipmentSlot.Weapon, duplicate.instanceId),
            "A specific equipment instance could not be equipped.");
        Require(equipment.ToggleLock(duplicate.instanceId) && !duplicate.locked,
            "Equipment instance could not be unlocked.");
        Require(!equipment.TryDismantle(duplicate.instanceId),
            "Equipped equipment was dismantled.");

        InventorySystem inventory = UnityEngine.Object.FindFirstObjectByType<InventorySystem>(
            FindObjectsInactive.Include);
        Require(inventory != null, "InventorySystem was not initialized.");
        EquipmentInstance disposable = equipment.GrantInstance("star_blade");
        Require(equipment.ToggleLock(disposable.instanceId) &&
            !equipment.TryDismantle(disposable.instanceId),
            "Locked equipment was dismantled.");
        Require(equipment.ToggleLock(disposable.instanceId),
            "Disposable equipment could not be unlocked.");
        int countBeforeDismantle = equipment.OwnedCount;
        int dustBeforeDismantle = inventory.StarDust;
        Require(equipment.TryDismantle(disposable.instanceId) &&
            equipment.OwnedCount == countBeforeDismantle - 1 &&
            inventory.StarDust == dustBeforeDismantle + EquipmentSystem.DismantleStarDustReward,
            "Equipment dismantling did not remove one instance and grant Star Dust.");
        gameManager.PlayerDataRef.energy += 1000;
        int energyBeforeEnhance = gameManager.PlayerDataRef.energy;
        int dustBeforeEnhance = inventory.StarDust;
        Require(equipment.GetUpgradeCost(duplicate) == 50 &&
            equipment.GetUpgradeStarDustCost(duplicate) == 5 &&
            equipment.TryUpgrade(duplicate.instanceId) && duplicate.level == 2,
            "Equipment Lv.1 to Lv.2 enhancement failed.");
        Require(gameManager.PlayerDataRef.energy == energyBeforeEnhance - 50 &&
            inventory.StarDust == dustBeforeEnhance - 5 &&
            equipment.GetEffectiveStats(duplicate).attack == 8,
            "Equipment enhancement cost or rounded effective stat was incorrect.");

        Require(!equipment.Equip(EquipmentSlot.Head, "star_blade"),
            "Weapon could be equipped in the Head slot.");
        Require(equipment.Equip(EquipmentSlot.Head, "seer_circlet"), "Head equip failed.");
        Require(equipment.Equip(EquipmentSlot.Chest, "iron_carapace"), "Chest equip failed.");
        Require(equipment.Equip(EquipmentSlot.Arms, "fury_bracers"), "Arms equip failed.");
        Require(equipment.Equip(EquipmentSlot.Legs, "guardian_leggings"), "Legs equip failed.");
        Require(equipment.Equip(EquipmentSlot.Feet, "windstep_boots"), "Feet equip failed.");
        Require(equipment.Equip(EquipmentSlot.Weapon, "quickfang"), "Weapon equip failed.");
        Require(equipment.Equip(EquipmentSlot.Accessory, "thorn_sigil"), "Accessory equip failed.");

        PlayerData player = gameManager.PlayerDataRef;
        EnemyData enemy = gameManager.EnemyDataRef;
        enemy.maxHP = 10000;
        enemy.attack = 1;
        enemy.defense = 2;
        enemy.agility = 1;
        battle.ResetBattle();
        CombatBuildSnapshot preview = battle.CreatePlayerBuildPreview();

        GameObject summaryOverlay = FindGameObject("BuildSummaryOverlay");
        GameObject summaryButtonObject = FindGameObject("BuildSummaryButton");
        GameObject summaryTextObject = FindGameObject("BuildSummaryText");
        Require(summaryOverlay != null && summaryButtonObject != null &&
            summaryTextObject != null, "Build Summary UI was not connected.");
        summaryButtonObject.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
        Require(summaryOverlay.activeSelf,
            "Build Summary did not open before battle.");
        string summaryText = summaryTextObject.GetComponent<TMPro.TMP_Text>().text;
        Require(summaryText.Contains("Star Burst") &&
            summaryText.Contains("Quickfang") &&
            summaryText.Contains("Thorn Sigil"),
            "Build Summary omitted Ultimate or equipment information.");
        FindGameObject("CloseBuildSummaryButton")
            .GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
        Require(!summaryOverlay.activeSelf, "Build Summary did not close.");

        Require(battle.StartBattle(), "Equipment test battle did not start.");
        CombatBuildSnapshot build = battle.PlayerBuild;
        Require(Mathf.Approximately(battle.EnemyBuild.bonusRageOnAttack, 5f) &&
            Mathf.Approximately(battle.EnemyBuild.bonusRageOnHit, 8f),
            "Enemy passive Rage effects were absent from the battle snapshot.");
        Require(build.maxHP == preview.maxHP && build.attack == preview.attack &&
            build.defense == preview.defense && build.agility == preview.agility &&
            build.wisdom == preview.wisdom &&
            build.ultimate.id == preview.ultimate.id,
            "Build Summary preview differed from the locked battle snapshot.");
        Require(build.maxHP == player.stats.hp + 25, "Equipment HP total was incorrect.");
        Require(build.attack == player.stats.attack + 5, "Equipment Attack total was incorrect.");
        Require(build.defense == player.stats.defense + 10, "Equipment Defense total was incorrect.");
        Require(build.agility == player.stats.agility + 6, "Equipment Agility total was incorrect.");
        Require(build.wisdom == player.stats.wisdom + 4, "Equipment Wisdom total was incorrect.");
        Require(!equipment.Equip(EquipmentSlot.Weapon, "star_blade"),
            "Equipment changed after the battle Build was locked.");

        battle.Tick(2f);
        Require(Mathf.Approximately(battle.BattleState.playerRage, 18.8f),
            "Attack-triggered equipment Rage was incorrect.");
        float rageBeforeHit = battle.BattleState.playerRage;
        battle.BattleState.enemyActionValue = BattleSystem.ActionThreshold;
        battle.Tick(0f);
        Require(Mathf.Approximately(
                battle.BattleState.playerRage - rageBeforeHit, 23.7f),
            "Hit-triggered equipment Rage was incorrect.");
        battle.ResetBattle();

        SaveData saveRoundTrip = new SaveData
        {
            ownedEquipmentIds = player.ownedEquipmentIds,
            equippedHeadItemId = player.equippedHeadItemId,
            equippedAccessoryItemId = player.equippedAccessoryItemId
        };
        SaveData restored = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(saveRoundTrip));
        Require(restored.ownedEquipmentIds.Length == 10 &&
            restored.equippedHeadItemId == "seer_circlet" &&
            restored.equippedAccessoryItemId == "thorn_sigil",
            "Equipment save fields did not survive JSON serialization.");
    }

    private static void TestPrototypeCompletion(
        GameManager gameManager,
        MainStageSystem stages,
        BattleSystem battle)
    {
        PlayerData player = gameManager.PlayerDataRef;
        MainStageState state = gameManager.MainStageStateRef;

        player.level = 100;
        player.headLevel = 100;
        player.armsLevel = 100;
        player.legsLevel = 100;
        player.chestLevel = 100;
        player.feetLevel = 100;
        player.weaponLevel = 100;
        player.CalculateStats();

        state.highestClearedStage = 19;
        state.highestUnlockedStage = 20;
        state.selectedStage = 20;
        state.battleStamina = MainStageSystem.MaxStamina;
        Require(stages.SelectStage(20), "Final prototype stage could not be selected.");
        Require(stages.TryStartSelectedStage(), "Final prototype stage did not start.");
        RunBattleToCompletion(battle);

        Require(battle.BattleState.battleResult == BattleResult.Victory,
            "Stage 20 did not end in Victory.");
        Require(stages.IsPrototypeComplete && state.highestClearedStage == 20,
            "Stage 20 did not set the prototype-complete state.");
        Require(state.highestUnlockedStage == 20 && state.selectedStage == 20,
            "Prototype completion advanced to a nonexistent Stage 21.");
        Require(stages.LastFeedback.Contains("Main Story Prototype Complete"),
            "Prototype completion feedback was not displayed.");
        Require(stages.CanSweepSelectedStage(),
            "Stage 20 was not available to sweep after completion.");
        int staminaBeforeSweep = state.battleStamina;
        Require(stages.TrySweepStage(20), "Completed Stage 20 could not be swept.");
        Require(state.battleStamina == staminaBeforeSweep - 1,
            "Stage 20 sweep did not consume exactly one stamina.");
    }

    private static GameObject FindGameObject(string objectName)
    {
        foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (gameObject.name == objectName && gameObject.scene.IsValid())
                return gameObject;
        }

        return null;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
