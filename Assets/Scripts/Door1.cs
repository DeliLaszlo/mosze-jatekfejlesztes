using Unity.VisualScripting;
using UnityEngine;

public class Door2 : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

    }

    [ContextMenu(itemName:"Close")]
    public void Close()
    {
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
