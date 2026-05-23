using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject highScorePanel;
    public TextMeshProUGUI leaderboardText;

    public void PlayGame()
    {
        PlayerHealthManager.ClearPersistedHealth();

        StartCoroutine(PlayGameWithFade());
    }

    private System.Collections.IEnumerator PlayGameWithFade()
    {
        ScreenFader.FadeOut(0.5f);

        yield return new WaitForSeconds(0.5f);

        MusicManager.instance.PlayMusicForScene("Intro");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Settings Panel is not assigned in the Inspector!");
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OpenHighScores()
    {
        if (highScorePanel != null)
        {
            UpdateLeaderboardDisplay();
            highScorePanel.SetActive(true);
        }
    }

    private void UpdateLeaderboardDisplay()
    {
        if (leaderboardText == null) return;

        string json = PlayerPrefs.GetString("Leaderboard", "{}");
        ScoreData data = JsonUtility.FromJson<ScoreData>(json);

        if (data == null || data.scores == null || data.scores.Count == 0)
        {
            leaderboardText.text = "Még nincs mentett eredmény!";
            return;
        }

        leaderboardText.text = "";
        for (int i = 0; i < data.scores.Count; i++)
        {
            leaderboardText.text += $"{i + 1}. {data.scores[i].playerName} - {data.scores[i].score}\n";
        }
    }

    public void CloseHighScores()
    {
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
    }
}