using System;

[Serializable]
public class MainStageState
{
    public int selectedStage;
    public int highestUnlockedStage;
    public int highestClearedStage;
    public int battleStamina;
    public long lastStaminaRefreshUtcTicks;

    public MainStageState()
    {
        ResetProgress();
    }

    public void ResetProgress()
    {
        selectedStage = 1;
        highestUnlockedStage = 1;
        highestClearedStage = 0;
        battleStamina = MainStageSystem.MaxStamina;
        lastStaminaRefreshUtcTicks = DateTime.UtcNow.Ticks;
    }

    public void Normalize()
    {
        selectedStage = Math.Max(1, Math.Min(MainStageSystem.TotalStages, selectedStage));
        highestClearedStage = Math.Max(0, Math.Min(MainStageSystem.TotalStages, highestClearedStage));
        highestUnlockedStage = Math.Max(
            1,
            Math.Min(MainStageSystem.TotalStages, highestUnlockedStage)
        );
        highestUnlockedStage = Math.Max(
            highestUnlockedStage,
            Math.Min(MainStageSystem.TotalStages, highestClearedStage + 1)
        );
        selectedStage = Math.Min(selectedStage, highestUnlockedStage);
        battleStamina = Math.Max(0, Math.Min(MainStageSystem.MaxStamina, battleStamina));

        if (lastStaminaRefreshUtcTicks <= 0 ||
            lastStaminaRefreshUtcTicks > DateTime.MaxValue.Ticks)
        {
            lastStaminaRefreshUtcTicks = DateTime.UtcNow.Ticks;
        }
    }
}
