using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Type the exact name of your Main Menu scene here.")]
    [SerializeField] private string menuSceneName = "MainMenu";

    [Tooltip("Type the exact name of your Level 1 / Restart scene here.")]
    [SerializeField] private string restartSceneName = "Level1";

    public void LoadMenu()
    {
        Debug.Log("Loading Menu Scene: " + menuSceneName);
        SceneManager.LoadScene(menuSceneName);
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting Level: " + restartSceneName);

        PlayerHealthManager playerHealthManager = FindAnyObjectByType<PlayerHealthManager>();

        if (playerHealthManager != null)
        {
            playerHealthManager.ResetHealth();
        }
        else
        {
            Debug.LogWarning("PlayerHealthManager not found in scene. Cannot reset health.");
            return;
        }


        SceneManager.LoadScene(restartSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}