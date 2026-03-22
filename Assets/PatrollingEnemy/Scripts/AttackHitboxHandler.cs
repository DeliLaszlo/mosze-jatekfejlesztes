using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAttackRange : MonoBehaviour
{
    [SerializeField] private PatrollingEnemy owner;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float checkInterval = 0.2f;

    private readonly HashSet<Collider2D> trackedPlayers = new HashSet<Collider2D>();
    private Coroutine monitorRoutine;

    private void Reset()
    {
        owner = GetComponentInParent<PatrollingEnemy>();

        Collider2D trigger = GetComponent<Collider2D>();
        if (trigger != null)
        {
            trigger.isTrigger = true;
        }
    }

    private void Awake()
    {
        if (owner == null)
        {
            owner = GetComponentInParent<PatrollingEnemy>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner == null || other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        trackedPlayers.Add(other);
        owner.HandleAttackRangeTrigger(other);

        if (monitorRoutine == null)
        {
            monitorRoutine = StartCoroutine(MonitorPlayersInRange());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        trackedPlayers.Remove(other);

        if (trackedPlayers.Count == 0 && monitorRoutine != null)
        {
            StopCoroutine(monitorRoutine);
            monitorRoutine = null;
        }
    }

    private IEnumerator MonitorPlayersInRange()
    {
        while (trackedPlayers.Count > 0)
        {
            
            Collider2D[] snapshot = new Collider2D[trackedPlayers.Count];
            trackedPlayers.CopyTo(snapshot);

            for (int i = 0; i < snapshot.Length; i++)
            {
                Collider2D playerCollider = snapshot[i];
                if (playerCollider == null)
                {
                    trackedPlayers.Remove(playerCollider);
                    continue;
                }

                if (owner != null)
                {
                    owner.HandleAttackRangeTrigger(playerCollider);
                }
            }

            if (trackedPlayers.Count == 0)
            {
                break;
            }

            yield return new WaitForSeconds(checkInterval);
        }

        monitorRoutine = null;
    }
}
