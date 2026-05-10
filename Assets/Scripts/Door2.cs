using Unity.VisualScripting;
using UnityEngine;

public class Door2 : MonoBehaviour
{
    private Animator _animator;

    [Header("Audio")]
    [SerializeField] private AudioSource closeAudioPrefab;

    private AudioSource closeAudioInstance;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (closeAudioPrefab != null)
        {
            closeAudioInstance = Instantiate(closeAudioPrefab, transform);
        }
    }

    [ContextMenu(itemName:"Close")]
    public void Close()
    {
        if (closeAudioInstance != null) closeAudioInstance.Play();

        BoxCollider2D[] colliders = GetComponentsInChildren<BoxCollider2D>(true);
        foreach (BoxCollider2D col in colliders)
        {
            if (!col.enabled)
            {
                col.enabled = true;
            }
        }

        if (_animator != null)
        {
            _animator.SetTrigger(name:"Close");
        }
    }
}