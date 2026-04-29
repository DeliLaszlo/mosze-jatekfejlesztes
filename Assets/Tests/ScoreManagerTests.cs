using System;
using NUnit.Framework;

public class ScoreManagerTests
{
    [SetUp]
    public void Setup()
    {
        ScoreManager.ResetScore();
    }

    [Test]
    public void ResetScore_InvokesScoreChangedWithZero()
    {
        int lastScore = -1;
        Action<int> handler = score => lastScore = score;
        ScoreManager.ScoreChanged += handler;

        ScoreManager.AddPoints(25);
        ScoreManager.ResetScore();

        ScoreManager.ScoreChanged -= handler;
        Assert.AreEqual(0, ScoreManager.CurrentScore);
        Assert.AreEqual(0, lastScore);
    }

    [Test]
    public void AddPoints_NonPositive_DoesNotChangeOrInvoke()
    {
        int callCount = 0;
        Action<int> handler = _ => callCount++;
        ScoreManager.ScoreChanged += handler;

        ScoreManager.AddPoints(0);
        ScoreManager.AddPoints(-5);

        ScoreManager.ScoreChanged -= handler;
        Assert.AreEqual(0, ScoreManager.CurrentScore);
        Assert.AreEqual(0, callCount);
    }

    [Test]
    public void AddPoints_InvokesScoreChanged()
    {
        int lastScore = 0;
        Action<int> handler = score => lastScore = score;
        ScoreManager.ScoreChanged += handler;

        ScoreManager.AddPoints(10);

        ScoreManager.ScoreChanged -= handler;
        Assert.AreEqual(10, lastScore);
    }

    [Test]
    public void AddPoints_ClampsToIntMax()
    {
        ScoreManager.AddPoints(int.MaxValue - 5);
        ScoreManager.AddPoints(10);

        Assert.AreEqual(int.MaxValue, ScoreManager.CurrentScore);
    }

    [Test]
    public void AddEnemyAndUrnScores_AddExpectedPoints()
    {
        ScoreManager.AddPatrollingEnemyKillScore();
        ScoreManager.AddMageKillScore();
        ScoreManager.AddGoldUrnScore();

        int expected = ScoreManager.PatrollingEnemyKillPoints
            + ScoreManager.MageEnemyKillPoints
            + ScoreManager.GoldUrnBreakPoints;

        Assert.AreEqual(expected, ScoreManager.CurrentScore);
    }
}
