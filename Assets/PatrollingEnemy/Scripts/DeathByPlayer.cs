using UnityEngine;

public class Stomp : MonoBehaviour
{
    [SerializeField] private Transform enemyRoot;
    private PatrollingEnemy enemy;

    private void Awake()
    {
        
        if (enemyRoot == null)
        {
            enemyRoot = transform.parent;
        }

        if (enemyRoot == null)
        {
            Debug.LogWarning($"{name}: No enemy root assigned and no parent found.", this);
        }

        if (enemyRoot != null)
        {
            enemy = enemyRoot.GetComponent<PatrollingEnemy>();
        }

        if (enemy == null)
        {
            enemy = GetComponentInParent<PatrollingEnemy>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (enemy != null)
        {
            enemy.DieFromStomp();
            return;
        }

        Transform rootToDestroy = enemyRoot != null ? enemyRoot : transform;
        Destroy(rootToDestroy.gameObject);
    }
}
