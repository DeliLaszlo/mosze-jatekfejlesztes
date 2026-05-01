using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
    public Vector3 moveOffset = new Vector3(0, 3, 0);
    public float speed = 3f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isUp = false;

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * speed
        );
    }

    public void TogglePlatform()
    {
        isUp = !isUp;
        targetPosition = isUp ? startPosition + moveOffset : startPosition;
    }

    public void MoveUp()
    {
        isUp = true;
        targetPosition = startPosition + moveOffset;
    }

    public void MoveDown()
    {
        isUp = false;
        targetPosition = startPosition;
    }
}