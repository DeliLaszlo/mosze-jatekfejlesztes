using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShowInteractUI : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject interactUI;

    private void Awake()
    {
        if (interactUI != null)
        {
            interactUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag(playerTag))
        {
            if (interactUI != null)
            {
                interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && other.CompareTag(playerTag))
        {
            if (interactUI != null)
            {
                interactUI.SetActive(false);
            }
        }
    }
}