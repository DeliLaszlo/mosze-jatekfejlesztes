using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
    [SerializeField] private PatrollingEnemy owner;

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
        if (owner == null)
        {
            return;
        }

        owner.HandleAttackRangeTrigger(other);
    }
}
