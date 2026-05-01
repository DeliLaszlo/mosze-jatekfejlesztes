using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerScoreText;

    private void Awake()
    {
        if (playerScoreText == null)
        {
            playerScoreText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        ScoreManager.ScoreChanged += HandleScoreChanged;
        HandleScoreChanged(ScoreManager.CurrentScore);
    }

    private void OnDisable()
    {
        ScoreManager.ScoreChanged -= HandleScoreChanged;
    }

    private void HandleScoreChanged(int score)
    {
        if (playerScoreText == null)
        {
            return;
        }

        playerScoreText.text = score.ToString();
    }
}
