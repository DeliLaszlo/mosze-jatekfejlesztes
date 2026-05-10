using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ResetManager : MonoBehaviour
{
    [SerializeField] private KeyCode fallbackResetKey = KeyCode.R;

    private bool isResetting;
    private int scoreAtSceneStart;

    private void Start()
    {
        scoreAtSceneStart = ScoreManager.CurrentScore;
    }

    private void Update()
    {
        if (isResetting)
        {
            return;
        }

        if (WasResetPressedThisFrame())
        {
            ResetScene();
        }
    }

    public void ResetScene()
    {
        if (isResetting)
        {
            return;
        }

        isResetting = true;
        StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        ScreenFader.FadeOut(0.25f);

        yield return new WaitForSeconds(0.25f);

        PlayerHealthManager playerHealth = FindAnyObjectByType<PlayerHealthManager>();
        if (playerHealth != null)
        {
            int nextHealth = Mathf.Max(1, playerHealth.CurrentHealth - 1);
            PlayerHealthManager.OverrideNextSpawnHealth(nextHealth);
        }

        int scoreGained = Mathf.Max(0, ScoreManager.CurrentScore - scoreAtSceneStart);
        int nextScore = Mathf.Max(0, ScoreManager.CurrentScore - scoreGained);
        ScoreManager.SetScore(nextScore);

        JumpKingSceneTransitionState.ResetState();
        SceneTransitionLevelStateManager.ResetState();

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    private bool WasResetPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

        return Input.GetKeyDown(fallbackResetKey);
    }
}