using UnityEngine;

public class SpikeHitbox : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            PlayerHealthManager healthManager = collision.GetComponent<PlayerHealthManager>();

            if (healthManager != null)
            {
                healthManager.SimulateDeath();
            }
            else
            {
                Debug.LogWarning("Player tag detected, but no PlayerHealthManager found on the object!");
            }
        }
    }
}