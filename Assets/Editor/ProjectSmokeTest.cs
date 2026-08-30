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

            Require(stages != null, "MainStageSystem was not initialized.");
            Require(stageState != null, "MainStageState was not initialized.");
            Require(battle != null, "BattleSystem was not initialized.");
            Require(upgrades != null, "UpgradeSystem was not found.");
            Require(panels != null, "PanelSwitcher was not found.");
            Require(ultimates != null, "UltimateSystem was not initialized.");
            Require(equipment != null, "EquipmentSystem was not initialized.");
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

            panels.ShowBattlePanel();
            int energyBeforeBattle = player.energy;
            Require(stages.TryStartSelectedStage(), "Stage 1 did not start.");

            panels.ShowMeditationPanel();
            GameObject battlePanel = FindGameObject("BattlePanel");
            GameObject meditationPanel = FindGameObject("MeditationPanel");
            Require(battlePanel != null && battlePanel.activeSelf,
                "Battle panel was not kept active during battle.");
            Require(meditationPanel != null && !meditationPanel.activeSelf,
                "Meditation panel opened during battle.");

            RunBattleToCompletion(battle);

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
            Require(equipment.IsOwned("star_blade") && equipment.OwnedCount == 1,
                "Stage 1 first clear did not unlock Star Blade exactly once.");

            int energyBeforeSweep = player.energy;
            Require(stages.TrySweepStage(1), "Cleared Stage 1 could not be swept.");
            Require(stageState.battleStamina == 18,
                "Sweep did not consume exactly 1 stamina.");
            Require(player.energy == energyBeforeSweep + 10,
                "Stage 1 sweep reward was incorrect.");
            Require(equipment.OwnedCount == 1,
                "Sweep incorrectly granted duplicate or additional equipment.");

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
