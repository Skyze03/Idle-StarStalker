using UnityEngine;

public class MeditationSystem : MonoBehaviour
{
    private PlayerData playerData;
    private MeditationState meditationState;
    private InventorySystem inventorySystem;
    private BuffData buffData;

    [SerializeField] private int expPerMeditation = 10;
    [SerializeField] private int autoMeditationUnlockLevel = 5;
    [SerializeField] private float memoryFragmentDropChance = 0.3f;

    public void Setup(PlayerData data, MeditationState state, InventorySystem inventory, BuffData buff)
    {
        playerData = data;
        meditationState = state;
        inventorySystem = inventory;
        buffData = buff;
    }

    public void StartMeditation()
    {
        if (playerData == null)
        {
            Debug.LogWarning("MeditationSystem: playerData is null.");
            return;
        }

        int totalExpGain = expPerMeditation;

        if (buffData != null)
        {
            totalExpGain += buffData.meditationExpBonus;
        }

        playerData.exp += totalExpGain;
        CheckLevelUp();
        TryDropMemoryFragment();

        Debug.Log("Meditation once. Current EXP = " + playerData.exp);
    }

    private void CheckLevelUp()
    {
        while (playerData.exp >= playerData.GetRequiredExp())
        {
            int requiredExp = playerData.GetRequiredExp();
            playerData.exp -= requiredExp;
            playerData.level++;

            Debug.Log("Level Up! New Level = " + playerData.level);

            if (meditationState != null && playerData.level >= autoMeditationUnlockLevel)
            {
                meditationState.autoMeditationUnlocked = true;
            }
        }
    }

    private void TryDropMemoryFragment()
    {
        if (inventorySystem == null)
        {
            return;
        }

        float roll = Random.value;

        if (roll <= memoryFragmentDropChance)
        {
            inventorySystem.AddMemoryFragment(1);
        }
    }

    public void ToggleAutoMeditation()
    {
        if (meditationState == null)
        {
            Debug.LogWarning("MeditationSystem: meditationState is null.");
            return;
        }

        if (!meditationState.autoMeditationUnlocked)
        {
            Debug.Log("Auto Meditation is still locked.");
            return;
        }

        meditationState.autoMeditationEnabled = !meditationState.autoMeditationEnabled;

        if (!meditationState.autoMeditationEnabled)
        {
            meditationState.autoMeditationTimer = 0f;
        }

        Debug.Log("Auto Meditation Enabled = " + meditationState.autoMeditationEnabled);
    }

    public void Tick(float deltaTime)
    {
        if (meditationState == null) return;
        if (!meditationState.autoMeditationUnlocked) return;
        if (!meditationState.autoMeditationEnabled) return;

        meditationState.autoMeditationTimer += deltaTime;

        if (meditationState.autoMeditationTimer >= meditationState.autoMeditationInterval)
        {
            meditationState.autoMeditationTimer = 0f;
            StartMeditation();
        }
    }
}