using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class JumpKingSceneTransitionState
{
    private static bool hasPendingTransition;
    private static string pendingEntryPointId = SceneTransitionSpawnPoint.DefaultEntryPointId;
    private static Vector2 pendingVelocity;
    private static float nextTriggerAllowedAt;

    public static bool CanTriggerTransition => Time.unscaledTime >= nextTriggerAllowedAt;

    public static void BeginTransition(string entryPointId, Vector2 carryVelocity)
    {
        hasPendingTransition = true;
        pendingEntryPointId = string.IsNullOrWhiteSpace(entryPointId)
            ? SceneTransitionSpawnPoint.DefaultEntryPointId
            : entryPointId;
        pendingVelocity = carryVelocity;
    }

    public static bool TryConsumeTransition(out string entryPointId, out Vector2 carryVelocity)
    {
        if (!hasPendingTransition)
        {
            entryPointId = SceneTransitionSpawnPoint.DefaultEntryPointId;
            carryVelocity = Vector2.zero;
            return false;
        }

        entryPointId = pendingEntryPointId;
        carryVelocity = pendingVelocity;

        hasPendingTransition = false;
        pendingEntryPointId = SceneTransitionSpawnPoint.DefaultEntryPointId;
        pendingVelocity = Vector2.zero;
        return true;
    }

    public static void LockTriggers(float duration)
    {
        if (duration <= 0f)
        {
            return;
        }

        nextTriggerAllowedAt = Time.unscaledTime + duration;
    }

    public static void ResetState()
    {
        hasPendingTransition = false;
        pendingEntryPointId = SceneTransitionSpawnPoint.DefaultEntryPointId;
        pendingVelocity = Vector2.zero;
        nextTriggerAllowedAt = 0f;
    }
}

public static class SceneTransitionLevelStateManager
{
    private static readonly HashSet<string> brokenUrnKeys = new HashSet<string>();
    private static readonly HashSet<string> defeatedEnemyKeys = new HashSet<string>();
    private static readonly HashSet<string> disabledGateKeys = new HashSet<string>();
    private static int savedStateApplyDepth;

    public static bool IsApplyingSavedState => savedStateApplyDepth > 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetForNewPlaySession()
    {
        brokenUrnKeys.Clear();
        defeatedEnemyKeys.Clear();
        disabledGateKeys.Clear();
        savedStateApplyDepth = 0;
    }

    public static void ResetState()
    {
        brokenUrnKeys.Clear();
        defeatedEnemyKeys.Clear();
        disabledGateKeys.Clear();
        savedStateApplyDepth = 0;
    }

    public static void ApplySavedStateSilently(Action applyAction)
    {
        if (applyAction == null)
        {
            return;
        }

        savedStateApplyDepth++;
        try
        {
            applyAction();
        }
        finally
        {
            savedStateApplyDepth--;
            if (savedStateApplyDepth < 0)
            {
                savedStateApplyDepth = 0;
            }
        }
    }

    public static void DisableForSavedState(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        ApplySavedStateSilently(() =>
        {
            AudioSource[] audioSources = target.GetComponentsInChildren<AudioSource>(true);
            for (int i = 0; i < audioSources.Length; i++)
            {
                AudioSource audioSource = audioSources[i];
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }

            target.SetActive(false);
        });
    }

    public static string BuildStateKey(GameObject target, string category)
    {
        if (target == null || string.IsNullOrWhiteSpace(category))
        {
            return string.Empty;
        }

        string scenePath = string.IsNullOrWhiteSpace(target.scene.path)
            ? target.scene.name
            : target.scene.path;

        return category + "|" + scenePath + "|" + BuildHierarchyPath(target.transform);
    }

    public static bool IsUrnBroken(string key)
    {
        return IsValidKey(key) && brokenUrnKeys.Contains(key);
    }

    public static void MarkUrnBroken(string key)
    {
        if (!IsValidKey(key))
        {
            return;
        }

        brokenUrnKeys.Add(key);
    }

    public static bool IsEnemyDefeated(string key)
    {
        return IsValidKey(key) && defeatedEnemyKeys.Contains(key);
    }

    public static void MarkEnemyDefeated(string key)
    {
        if (!IsValidKey(key))
        {
            return;
        }

        defeatedEnemyKeys.Add(key);
    }

    public static bool IsGateDisabled(string key)
    {
        return IsValidKey(key) && disabledGateKeys.Contains(key);
    }

    public static void MarkGateDisabled(string key)
    {
        if (!IsValidKey(key))
        {
            return;
        }

        disabledGateKeys.Add(key);
    }

    private static bool IsValidKey(string key)
    {
        return !string.IsNullOrWhiteSpace(key);
    }

    private static string BuildHierarchyPath(Transform node)
    {
        if (node == null)
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder(128);
        Transform current = node;

        while (current != null)
        {
            builder.Insert(0, "/" + current.name + "[" + current.GetSiblingIndex().ToString() + "]");
            current = current.parent;
        }

        return builder.ToString();
    }
}
