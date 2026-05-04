using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Collider2D))]
public class GoldUrnInteractable : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode fallbackInteractKey = KeyCode.E;
    [SerializeField] private GameObject urnRootToDisable;
    [SerializeField] private GameObject interactUI;

    [Header("Audio")]
    [SerializeField] private AudioSource breakAudioPrefab;

    private int playersInRange;
    private bool isBroken;
    private string urnStateKey;

    private void Reset()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        if (trigger != null)
        {
            trigger.isTrigger = true;
        }

        if (urnRootToDisable == null)
        {
            urnRootToDisable = gameObject;
        }
    }

    private void Awake()
    {
        if (interactUI != null)
        {
            interactUI.SetActive(false);
        }

        if (urnRootToDisable == null)
        {
            urnRootToDisable = gameObject;
        }

        urnStateKey = SceneTransitionLevelStateManager.BuildStateKey(gameObject, "Urn");
        if (SceneTransitionLevelStateManager.IsUrnBroken(urnStateKey))
        {
            isBroken = true;
            playersInRange = 0;

            GameObject rootToDisable = urnRootToDisable != null ? urnRootToDisable : gameObject;
            SceneTransitionLevelStateManager.DisableForSavedState(rootToDisable);
        }
    }

    private void Update()
    {
        if (isBroken || playersInRange <= 0)
        {
            return;
        }

        if (!WasInteractPressedThisFrame())
        {
            return;
        }

        BreakUrn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBroken || other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        if (interactUI != null)
        {
            interactUI.SetActive(true);
        }

        playersInRange++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null || !other.CompareTag(playerTag))
        {
            return;
        }

        playersInRange = Mathf.Max(0, playersInRange - 1);

        if (interactUI != null)
        {
            interactUI.SetActive(false);
        }
    }

    private bool WasInteractPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

        return Input.GetKeyDown(fallbackInteractKey);
    }

    private void BreakUrn()
    {
        isBroken = true;
        playersInRange = 0;
        SceneTransitionLevelStateManager.MarkUrnBroken(urnStateKey);

        ScoreManager.AddGoldUrnScore();

        if (breakAudioPrefab != null)
        {
            AudioSource breakSfx = Instantiate(breakAudioPrefab, transform.position, Quaternion.identity);
            breakSfx.Play();
            Destroy(breakSfx.gameObject, breakSfx.clip != null ? breakSfx.clip.length : 2f);
        }

        if (interactUI != null)
        {
            interactUI.SetActive(false);
        }

        if (urnRootToDisable != null)
        {
            urnRootToDisable.SetActive(false);
        }
    }
}