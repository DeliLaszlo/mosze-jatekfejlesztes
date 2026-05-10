using UnityEngine;

public class PressurePlate2D : MonoBehaviour
{
    public SlidingDoor door;

    [Header("Plate Animation")]
    public Transform plateVisual;
    public Vector3 pressedOffset = new Vector3(0, -0.1f, 0);
    public float pressSpeed = 8f;

    [Header("Audio")]
    [SerializeField] private AudioSource openAudioPrefab;
    [SerializeField] private AudioSource closeAudioPrefab;

    private AudioSource openAudioInstance;
    private AudioSource closeAudioInstance;

    private Vector3 releasedLocalPosition;
    private Vector3 pressedLocalPosition;
    private int boxesOnPlate = 0;

    private void Start()
    {
        if (plateVisual == null)
        {
            plateVisual = transform;
        }

        releasedLocalPosition = plateVisual.localPosition;
        pressedLocalPosition = releasedLocalPosition + pressedOffset;

        if (openAudioPrefab != null)
        {
            openAudioInstance = Instantiate(openAudioPrefab, transform);
        }

        if (closeAudioPrefab != null)
        {
            closeAudioInstance = Instantiate(closeAudioPrefab, transform);
        }
    }

    private void Update()
    {
        Vector3 targetPosition = boxesOnPlate > 0 ? pressedLocalPosition : releasedLocalPosition;

        plateVisual.localPosition = Vector3.Lerp(
            plateVisual.localPosition,
            targetPosition,
            Time.deltaTime * pressSpeed
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            if (boxesOnPlate == 0)
            {
                if (openAudioInstance != null) openAudioInstance.Play();
            }

            boxesOnPlate++;
            door.OpenDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            boxesOnPlate--;

            if (boxesOnPlate <= 0)
            {
                boxesOnPlate = 0;

                if (closeAudioInstance != null) closeAudioInstance.Play();

                door.CloseDoor();
            }
        }
    }
}