using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameWinHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreDisplayText;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private string menuSceneName = "TempMainMenu";

    private void OnEnable()
    {
        int finalScore = ScoreManager.CurrentScore;
        scoreDisplayText.text = finalScore.ToString();
    }

    public void SaveScoreAndReturnToMenu()
    {
        int finalScore = ScoreManager.CurrentScore;
        string playerName = nameInputField.text;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Ismeretlen hős";
        }

        SaveScore(playerName, finalScore);
        LoadMenuScene();
    }

    private void SaveScore(string name, int score)
    {
        string json = PlayerPrefs.GetString("Leaderboard", "{}");
        ScoreData data = JsonUtility.FromJson<ScoreData>(json);

        if (data == null)
        {
            data = new ScoreData();
        }

        if (data.scores == null)
        {
            data.scores = new List<ScoreEntry>();
        }

        data.scores.Add(new ScoreEntry { playerName = name, score = score });

        data.scores.Sort((x, y) => y.score.CompareTo(x.score));

        if (data.scores.Count > 10)
        {
            data.scores.RemoveAt(data.scores.Count - 1);
        }

        string newJson = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Leaderboard", newJson);
        PlayerPrefs.Save();
    }

    public void ReturnToMenuWithoutSaving()
    {
        LoadMenuScene();
    }

    private void LoadMenuScene()
    {
        ScoreManager.ResetScore();
        SceneManager.LoadScene(menuSceneName);
    }
}