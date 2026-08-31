using System;

[Serializable]
public class DailyChallengeState
{
    public string utcDateKey = string.Empty;
    public int remainingAttempts = DailyChallengeSystem.MaxDailyAttempts;
}
