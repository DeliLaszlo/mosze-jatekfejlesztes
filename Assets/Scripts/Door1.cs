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
        _animator.SetTrigger(name:"Close");
    }
}
