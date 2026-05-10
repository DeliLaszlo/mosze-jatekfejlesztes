using System;
using UnityEngine;

public static class ScoreManager
{
    public const int PatrollingEnemyKillPoints = 200;
    public const int MageEnemyKillPoints = 1000;
    public const int GoldUrnBreakPoints = 50;

    private static int currentScore;

    public static event Action<int> ScoreChanged;

    public static int CurrentScore => currentScore;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStateForNewPlaySession()
    {
        currentScore = 0;
        ScoreChanged = null;
    }

    public static void ResetScore()
    {
        currentScore = 0;
        ScoreChanged?.Invoke(currentScore);
    }

    public static void SetScore(int score)
    {
        currentScore = Mathf.Max(0, score);
        ScoreChanged?.Invoke(currentScore);
    }

    public static void AddPoints(int points)
    {
        if (points <= 0)
        {
            return;
        }

        if (currentScore > int.MaxValue - points)
        {
            currentScore = int.MaxValue;
        }
        else
        {
            currentScore += points;
        }

        ScoreChanged?.Invoke(currentScore);
    }

    public static void AddPatrollingEnemyKillScore()
    {
        AddPoints(PatrollingEnemyKillPoints);
    }

    public static void AddMageKillScore()
    {
        AddPoints(MageEnemyKillPoints);
    }

    public static void AddGoldUrnScore()
    {
        AddPoints(GoldUrnBreakPoints);
    }
}
