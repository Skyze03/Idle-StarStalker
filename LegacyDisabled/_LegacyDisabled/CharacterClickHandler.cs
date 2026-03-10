using UnityEngine;

public class CharacterClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("CharacterClickHandler: GameManager.Instance is null.");
            return;
        }

        MeditationSystem meditationSystem = GameManager.Instance.GetMeditationSystem();

        if (meditationSystem == null)
        {
            Debug.LogWarning("CharacterClickHandler: MeditationSystem is null.");
            return;
        }

        meditationSystem.MeditateOnce();
    }
}