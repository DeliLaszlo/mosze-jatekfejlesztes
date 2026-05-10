using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator _animator;

    [Header("Audio")]
    [SerializeField] private AudioSource openAudioPrefab;

    private AudioSource openAudioInstance;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (openAudioPrefab != null)
        {
            openAudioInstance = Instantiate(openAudioPrefab, transform);
        }
    }

    [ContextMenu(itemName:"Open")]
    public void Open()
    {
        if (openAudioInstance != null) openAudioInstance.Play();

        BoxCollider2D[] colliders = GetComponentsInChildren<BoxCollider2D>(true);
        foreach (BoxCollider2D col in colliders)
        {
            if (!col.enabled)
            {
                col.enabled = true;
            }
        }
        _animator.SetTrigger(name:"Open");
    }
}