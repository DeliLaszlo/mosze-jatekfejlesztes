using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(0, 3, 0);
    public float speed = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource openAudioPrefab;

    private AudioSource openAudioInstance;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + openOffset;

        if (openAudioPrefab != null)
        {
            openAudioInstance = Instantiate(openAudioPrefab, transform);
        }
    }

    private void Update()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
    }

    public void OpenDoor()
    {
        if (!isOpen && openAudioInstance != null)
        {
            openAudioInstance.Play();
        }
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }
}