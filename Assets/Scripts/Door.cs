using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

    }

    [ContextMenu(itemName:"Open")]
    public void Open()
    {
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
