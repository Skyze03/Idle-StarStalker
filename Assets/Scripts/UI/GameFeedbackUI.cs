using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFeedbackUI : MonoBehaviour
{
    [SerializeField] private GameObject toastPanel;
    [SerializeField] private TMP_Text toastText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultTitleText;
    [SerializeField] private TMP_Text resultBodyText;
    [SerializeField] private Button resultContinueButton;
    [SerializeField] private float toastHoldDuration = 0.3f;
    [SerializeField] private float toastFadeDuration = 0.8f;
    [SerializeField] private float toastRiseDistance = 80f;
    [SerializeField] private float toastSpacing = 38f;
    [SerializeField] private int maxVisibleToasts = 4;

    private sealed class ToastEntry
    {
        public GameObject gameObject;
        public RectTransform rect;
        public CanvasGroup canvasGroup;
        public float elapsed;
        public Vector2 startPosition;
    }

    private readonly List<ToastEntry> activeToasts = new List<ToastEntry>();
    private string latestToast = string.Empty;

    public bool IsToastVisible => activeToasts.Count > 0;
    public int ActiveToastCount => activeToasts.Count;
    public bool IsResultVisible => resultPanel != null && resultPanel.activeSelf;
    public string CurrentToast => latestToast;
    public string CurrentResult => resultBodyText != null ? resultBodyText.text : string.Empty;

    public void Setup(
        MeditationSystem meditationSystem,
        CollectionSystem collectionSystem,
        MainStageSystem mainStageSystem,
        UpgradeSystem upgradeSystem,
        CombinerSystem combinerSystem,
        EquipmentSystem equipmentSystem,
        UltimateSystem ultimateSystem)
    {
        if (meditationSystem != null)
        {
            meditationSystem.FeedbackRequested -= ShowToast;
            meditationSystem.FeedbackRequested += ShowToast;
        }
        if (collectionSystem != null)
        {
            collectionSystem.FeedbackRequested -= ShowToast;
            collectionSystem.FeedbackRequested += ShowToast;
        }
        if (mainStageSystem != null)
        {
            mainStageSystem.ToastRequested -= ShowToast;
            mainStageSystem.ToastRequested += ShowToast;
            mainStageSystem.ResultRequested -= ShowResult;
            mainStageSystem.ResultRequested += ShowResult;
        }
        Subscribe(upgradeSystem);
        Subscribe(combinerSystem);
        Subscribe(equipmentSystem);
        Subscribe(ultimateSystem);

        if (resultContinueButton != null)
        {
            resultContinueButton.onClick.RemoveAllListeners();
            resultContinueButton.onClick.AddListener(HideResult);
        }

        if (toastPanel != null) toastPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void Subscribe(UpgradeSystem system)
    {
        if (system == null) return;
        system.FeedbackRequested -= ShowToast;
        system.FeedbackRequested += ShowToast;
    }

    private void Subscribe(CombinerSystem system)
    {
        if (system == null) return;
        system.FeedbackRequested -= ShowToast;
        system.FeedbackRequested += ShowToast;
    }

    private void Subscribe(EquipmentSystem system)
    {
        if (system == null) return;
        system.FeedbackRequested -= ShowToast;
        system.FeedbackRequested += ShowToast;
    }

    private void Subscribe(UltimateSystem system)
    {
        if (system == null) return;
        system.FeedbackRequested -= ShowToast;
        system.FeedbackRequested += ShowToast;
    }

    private void Update()
    {
        for (int i = activeToasts.Count - 1; i >= 0; i--)
        {
            ToastEntry toast = activeToasts[i];
            toast.elapsed += Time.unscaledDeltaTime;
            if (toast.elapsed <= toastHoldDuration) continue;

            float progress = Mathf.Clamp01(
                (toast.elapsed - toastHoldDuration) / Mathf.Max(0.01f, toastFadeDuration)
            );
            toast.rect.anchoredPosition = toast.startPosition +
                Vector2.up * (toastRiseDistance * progress);
            toast.canvasGroup.alpha = 1f - progress;

            if (progress < 1f) continue;
            Destroy(toast.gameObject);
            activeToasts.RemoveAt(i);
        }
    }

    public void ShowToast(string message)
    {
        if (toastPanel == null || toastText == null || string.IsNullOrWhiteSpace(message))
            return;
        latestToast = message;
        for (int i = activeToasts.Count - 1; i >= 0; i--)
        {
            activeToasts[i].startPosition += Vector2.up * toastSpacing;
            activeToasts[i].rect.anchoredPosition += Vector2.up * toastSpacing;
        }
        while (activeToasts.Count >= Mathf.Max(1, maxVisibleToasts))
        {
            ToastEntry oldest = activeToasts[0];
            if (oldest.gameObject != null) Destroy(oldest.gameObject);
            activeToasts.RemoveAt(0);
        }

        GameObject instance = Instantiate(toastPanel, toastPanel.transform.parent);
        instance.name = "FeedbackToastInstance";
        TMP_Text instanceText = instance.GetComponentInChildren<TMP_Text>(true);
        if (instanceText != null) instanceText.text = message;
        RectTransform rect = instance.GetComponent<RectTransform>();
        Vector2 start = toastPanel.GetComponent<RectTransform>().anchoredPosition;
        rect.anchoredPosition = start;
        CanvasGroup canvasGroup = instance.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = instance.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
        instance.SetActive(true);
        instance.transform.SetAsLastSibling();
        activeToasts.Add(new ToastEntry
        {
            gameObject = instance,
            rect = rect,
            canvasGroup = canvasGroup,
            startPosition = start
        });
    }

    public void ShowResult(string title, string body)
    {
        if (resultPanel == null) return;
        if (resultTitleText != null) resultTitleText.text = title;
        if (resultBodyText != null) resultBodyText.text = body;
        ClearToasts();
        resultPanel.SetActive(true);
        resultPanel.transform.SetAsLastSibling();
    }

    public void HideResult()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void ClearToasts()
    {
        foreach (ToastEntry toast in activeToasts)
            if (toast.gameObject != null) Destroy(toast.gameObject);
        activeToasts.Clear();
    }
}
