using UnityEngine;

public class LeverSwitch2D : MonoBehaviour
{
    public MovingPlatform2D platform;

    [Header("Lever Visuals")]
    public GameObject leverOffVisual;
    public GameObject leverOnVisual;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Audio")]
    [SerializeField] private AudioSource switchAudioPrefab;

    private AudioSource switchAudioInstance;

    private bool playerInRange = false;
    private bool isOn = false;

    private void Start()
    {
        if (switchAudioPrefab != null)
        {
            switchAudioInstance = Instantiate(switchAudioPrefab, transform);
        }

        UpdateVisual();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            ToggleLever();
        }
    }

    private void ToggleLever()
    {
        isOn = !isOn;

        if (switchAudioInstance != null) switchAudioInstance.Play();

        UpdateVisual();

        if (platform != null)
        {
            platform.TogglePlatform();
        }
    }

    private void UpdateVisual()
    {
        if (leverOffVisual != null)
        {
            leverOffVisual.SetActive(!isOn);
        }

        if (leverOnVisual != null)
        {
            leverOnVisual.SetActive(isOn);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}