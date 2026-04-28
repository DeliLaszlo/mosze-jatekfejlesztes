using UnityEngine;

public class Doors : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    [ContextMenu(itemName: "Close")]
    public void Close()
    {
        _animator.SetTrigger("Close");
    }
}
