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
}
