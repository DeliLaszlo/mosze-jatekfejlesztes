using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SceneEdgeTransitionTrigger : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetEntryPointId = SceneTransitionSpawnPoint.DefaultEntryPointId;

    [Header("Velocity")]
    [SerializeField] private bool carryHorizontalVelocity = true;
    [SerializeField] private bool carryVerticalVelocity = true;

    [Header("Direction Gate")]
    [SerializeField] private bool requireAscendingVelocity;
    [SerializeField] private bool requireDescendingVelocity;

    [Header("Safety")]
    [SerializeField] private float triggerLockDuration = 0.2f;

    private bool transitionTriggered;

    private void Reset()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (transitionTriggered || !JumpKingSceneTransitionState.CanTriggerTransition)
        {
            return;
        }

        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning($"{name}: Target scene is not set.", this);
            return;
        }

        Rigidbody2D playerRb = playerController.GetComponent<Rigidbody2D>();
        Vector2 carryVelocity = playerRb != null ? playerRb.linearVelocity : Vector2.zero;

        if (requireAscendingVelocity && carryVelocity.y <= 0f)
        {
            return;
        }

        if (requireDescendingVelocity && carryVelocity.y >= 0f)
        {
            return;
        }

        if (!carryHorizontalVelocity)
        {
            carryVelocity.x = 0f;
        }

        if (!carryVerticalVelocity)
        {
            carryVelocity.y = 0f;
        }

        transitionTriggered = true;
        JumpKingSceneTransitionState.BeginTransition(targetEntryPointId, carryVelocity);
        JumpKingSceneTransitionState.LockTriggers(triggerLockDuration);
        MusicManager.instance.PlayMusicForScene(targetSceneName);
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }
}
