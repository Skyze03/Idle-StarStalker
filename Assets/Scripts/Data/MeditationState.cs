using System;

[Serializable]
public class MeditationState
{
    public bool autoMeditationUnlocked = false;
    public bool autoMeditationEnabled = false;
    public float autoMeditationInterval = 1f;
    public float autoMeditationTimer = 0f;
}