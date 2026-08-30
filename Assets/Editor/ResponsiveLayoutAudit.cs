using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ResponsiveLayoutAudit
{
    private const string TargetScene = "Assets/Scenes/MainScene_Rebuild.unity";

    private readonly struct DeviceProfile
    {
        public readonly string name;
        public readonly int width;
        public readonly int height;
        public readonly int safeLeft;
        public readonly int safeRight;
        public readonly int safeTop;
        public readonly int safeBottom;

        public DeviceProfile(
            string name,
            int width,
            int height,
            int safeLeft = 0,
            int safeRight = 0,
            int safeTop = 0,
            int safeBottom = 0)
        {
            this.name = name;
            this.width = width;
            this.height = height;
            this.safeLeft = safeLeft;
            this.safeRight = safeRight;
            this.safeTop = safeTop;
            this.safeBottom = safeBottom;
        }
    }

    public static void RunBatch()
    {
        Scene scene = EditorSceneManager.OpenScene(TargetScene, OpenSceneMode.Single);
        RectTransform safeArea = FindRect(scene, "SafeArea");

        if (safeArea == null)
        {
            throw new InvalidOperationException("SafeArea was not found.");
        }

        DeviceProfile[] profiles =
        {
            new DeviceProfile("iPhone 15 Pro Max", 1290, 2796, safeTop: 177, safeBottom: 102),
            new DeviceProfile("iPhone SE", 750, 1334),
            new DeviceProfile("Narrow Android", 720, 1600, safeTop: 60, safeBottom: 60),
            new DeviceProfile("Tall Android", 1080, 2400, safeTop: 72, safeBottom: 72),
            new DeviceProfile("Portrait iPad", 2048, 2732, safeTop: 48, safeBottom: 40)
        };

        int failures = 0;

        foreach (DeviceProfile profile in profiles)
        {
            float scale = profile.height / 932f;
            float safeWidth =
                (profile.width - profile.safeLeft - profile.safeRight) / scale;
            float safeHeight =
                (profile.height - profile.safeTop - profile.safeBottom) / scale;
            Rect logicalSafeRect = new Rect(
                -safeWidth * 0.5f,
                -safeHeight * 0.5f,
                safeWidth,
                safeHeight
            );

            List<string> profileFailures = new List<string>();
            AuditSafeAreaChildren(safeArea, logicalSafeRect, profileFailures);

            if (profileFailures.Count == 0)
            {
                Debug.Log(
                    $"RESPONSIVE UI PASS: {profile.name} " +
                    $"safe logical size {safeWidth:F1}x{safeHeight:F1}."
                );
            }
            else
            {
                failures += profileFailures.Count;
                foreach (string failure in profileFailures)
                {
                    Debug.LogError($"RESPONSIVE UI FAIL: {profile.name} - {failure}");
                }
            }
        }

        if (failures > 0)
        {
            throw new InvalidOperationException(
                $"Responsive layout audit found {failures} out-of-bounds elements."
            );
        }

        Debug.Log("RESPONSIVE LAYOUT AUDIT: PASS");
    }

    private static void AuditSafeAreaChildren(
        RectTransform safeArea,
        Rect safeBounds,
        List<string> failures)
    {
        foreach (Transform childTransform in safeArea)
        {
            RectTransform child = childTransform as RectTransform;
            if (child == null) continue;

            if (child.name.EndsWith("Panel"))
            {
                AuditContainer(child, safeBounds, failures, child.name);
            }
            else if (!IsStretch(child))
            {
                AuditRect(child, safeBounds, failures, child.name);
            }
        }
    }

    private static void AuditContainer(
        RectTransform container,
        Rect safeBounds,
        List<string> failures,
        string path)
    {
        foreach (Transform childTransform in container)
        {
            RectTransform child = childTransform as RectTransform;
            if (child == null) continue;

            if (IsStretch(child))
            {
                AuditContainer(child, safeBounds, failures, path + "/" + child.name);
            }
            else
            {
                AuditRect(child, safeBounds, failures, path + "/" + child.name);
            }
        }
    }

    private static void AuditRect(
        RectTransform rect,
        Rect safeBounds,
        List<string> failures,
        string path)
    {
        Vector2 position = rect.anchoredPosition;
        Vector2 size = Vector2.Scale(rect.rect.size, rect.localScale);
        Rect elementBounds = new Rect(
            position.x - size.x * rect.pivot.x,
            position.y - size.y * rect.pivot.y,
            size.x,
            size.y
        );

        const float tolerance = 1f;
        if (elementBounds.xMin < safeBounds.xMin - tolerance ||
            elementBounds.xMax > safeBounds.xMax + tolerance ||
            elementBounds.yMin < safeBounds.yMin - tolerance ||
            elementBounds.yMax > safeBounds.yMax + tolerance)
        {
            failures.Add(
                $"{path} bounds {elementBounds} exceed safe bounds {safeBounds}."
            );
        }
    }

    private static bool IsStretch(RectTransform rect)
    {
        return rect.anchorMin != rect.anchorMax;
    }

    private static RectTransform FindRect(Scene scene, string objectName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (RectTransform rect in root.GetComponentsInChildren<RectTransform>(true))
            {
                if (rect.name == objectName) return rect;
            }
        }

        return null;
    }
}
