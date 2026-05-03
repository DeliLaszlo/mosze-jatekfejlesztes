using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerSceneTransitionHandler : MonoBehaviour
{
    [SerializeField] private bool applyCarryVelocity = true;
    [SerializeField] private float postSpawnTriggerLockDuration = 0.15f;

    private void Start()
    {
        if (!JumpKingSceneTransitionState.TryConsumeTransition(out string entryPointId, out Vector2 carryVelocity))
        {
            return;
        }

        SceneTransitionSpawnPoint[] spawnPoints = Object.FindObjectsByType<SceneTransitionSpawnPoint>(FindObjectsInactive.Exclude);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].EntryPointId != entryPointId)
            {
                continue;
            }

            transform.position = spawnPoints[i].transform.position;

            if (applyCarryVelocity)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = carryVelocity;
                }
            }

            JumpKingSceneTransitionState.LockTriggers(postSpawnTriggerLockDuration);
            return;
        }

        Debug.LogWarning($"{name}: No SceneTransitionSpawnPoint found with id '{entryPointId}'.", this);
        JumpKingSceneTransitionState.LockTriggers(postSpawnTriggerLockDuration);
    }
}
