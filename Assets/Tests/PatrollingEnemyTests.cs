using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class PatrollingEnemyTests
{
    [SetUp]
    public void Setup()
    {
        ScoreManager.ResetScore();
        TestUtilities.ResetSceneTransitionLevelStateManager();
    }

    [Test]
    public void Start_DisablesWhenPointsMissing()
    {
        GameObject enemyObject = new GameObject("Enemy");
        PatrollingEnemy enemy = enemyObject.AddComponent<PatrollingEnemy>();

        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("Assign both patrol points"));
        TestUtilities.InvokePrivateMethod(enemy, "Start");

        Assert.IsFalse(enemy.enabled);

        Object.DestroyImmediate(enemyObject);
    }

    [Test]
    public void Start_SetsMoveDirectionBasedOnStartPosition()
    {
        GameObject enemyObject = new GameObject("Enemy");
        enemyObject.transform.position = new Vector3(-1f, 0f, 0f);
        enemyObject.AddComponent<Rigidbody2D>();
        PatrollingEnemy enemy = enemyObject.AddComponent<PatrollingEnemy>();

        Transform pointA = new GameObject("PointA").transform;
        Transform pointB = new GameObject("PointB").transform;
        pointA.position = Vector3.zero;
        pointB.position = new Vector3(5f, 0f, 0f);
        enemy.pointA = pointA;
        enemy.pointB = pointB;

        TestUtilities.InvokePrivateMethod(enemy, "Start");

        int moveDirection = TestUtilities.GetPrivateField<int>(enemy, "moveDirection");
        Assert.AreEqual(1, moveDirection);

        Object.DestroyImmediate(pointA.gameObject);
        Object.DestroyImmediate(pointB.gameObject);
        Object.DestroyImmediate(enemyObject);
    }

    [Test]
    public void HandleAttackRangeTrigger_IgnoresNonPlayer()
    {
        GameObject enemyObject = new GameObject("Enemy");
        PatrollingEnemy enemy = enemyObject.AddComponent<PatrollingEnemy>();
        TestUtilities.SetPrivateField(enemy, "playerTag", "Player");

        GameObject other = new GameObject("Other");
        other.tag = "Untagged";
        BoxCollider2D otherCollider = other.AddComponent<BoxCollider2D>();

        enemy.HandleAttackRangeTrigger(otherCollider);

        bool isAttacking = TestUtilities.GetPrivateField<bool>(enemy, "isAttacking");
        Assert.IsFalse(isAttacking);

        Object.DestroyImmediate(other);
        Object.DestroyImmediate(enemyObject);
    }

    [Test]
    public void HandleAttackRangeTrigger_StartsAttackAgainstPlayer()
    {
        GameObject enemyObject = new GameObject("Enemy");
        PatrollingEnemy enemy = enemyObject.AddComponent<PatrollingEnemy>();
        TestUtilities.SetPrivateField(enemy, "playerTag", "Player");
        TestUtilities.SetPrivateField(enemy, "destroyPlayerOnAttack", false);
        TestUtilities.SetPrivateField(enemy, "attackDuration", 0f);
        TestUtilities.ResetPlayerHealthManagerStaticState();

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();
        player.AddComponent<PlayerHealthManager>();

        enemy.HandleAttackRangeTrigger(playerCollider);

        bool isAttacking = TestUtilities.GetPrivateField<bool>(enemy, "isAttacking");
        Assert.IsTrue(isAttacking);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemyObject);
    }

    [Test]
    public void DieFromStomp_DisablesComponentsAndScores()
    {
        GameObject enemyObject = new GameObject("Enemy");
        PatrollingEnemy enemy = enemyObject.AddComponent<PatrollingEnemy>();
        Rigidbody2D rb = enemyObject.AddComponent<Rigidbody2D>();
        BoxCollider2D collider = enemyObject.AddComponent<BoxCollider2D>();

        TestUtilities.SetPrivateField(enemy, "rb", rb);

        enemy.DieFromStomp();

        Assert.IsFalse(enemy.enabled);
        Assert.IsFalse(collider.enabled);
        Assert.IsFalse(rb.simulated);
        Assert.AreEqual(ScoreManager.PatrollingEnemyKillPoints, ScoreManager.CurrentScore);

        Object.DestroyImmediate(enemyObject);
    }
}

public class EnemyAttackRangeTests
{
    [Test]
    public void Reset_AssignsOwnerAndTrigger()
    {
        GameObject enemyObject = new GameObject("Enemy");
        PatrollingEnemy enemy = enemyObject.AddComponent<PatrollingEnemy>();

        GameObject rangeObject = new GameObject("Range");
        rangeObject.transform.SetParent(enemyObject.transform, false);
        BoxCollider2D rangeCollider = rangeObject.AddComponent<BoxCollider2D>();
        EnemyAttackRange range = rangeObject.AddComponent<EnemyAttackRange>();

        TestUtilities.InvokePrivateMethod(range, "Reset");

        PatrollingEnemy owner = TestUtilities.GetPrivateField<PatrollingEnemy>(range, "owner");
        Assert.AreEqual(enemy, owner);
        Assert.IsTrue(rangeCollider.isTrigger);

        Object.DestroyImmediate(rangeObject);
        Object.DestroyImmediate(enemyObject);
    }

    [Test]
    public void OnTriggerEnter_IgnoresNonPlayer()
    {
        GameObject enemyObject = new GameObject("Enemy");
        PatrollingEnemy enemy = enemyObject.AddComponent<PatrollingEnemy>();

        GameObject rangeObject = new GameObject("Range");
        rangeObject.transform.SetParent(enemyObject.transform, false);
        rangeObject.AddComponent<BoxCollider2D>();
        EnemyAttackRange range = rangeObject.AddComponent<EnemyAttackRange>();

        TestUtilities.SetPrivateField(range, "owner", enemy);
        TestUtilities.SetPrivateField(range, "playerTag", "Player");

        GameObject other = new GameObject("Other");
        other.tag = "Untagged";
        BoxCollider2D otherCollider = other.AddComponent<BoxCollider2D>();

        TestUtilities.InvokePrivateMethod(range, "OnTriggerEnter2D", otherCollider);

        System.Collections.Generic.HashSet<Collider2D> tracked =
            TestUtilities.GetPrivateField<System.Collections.Generic.HashSet<Collider2D>>(range, "trackedPlayers");
        Assert.AreEqual(0, tracked.Count);

        Object.DestroyImmediate(other);
        Object.DestroyImmediate(rangeObject);
        Object.DestroyImmediate(enemyObject);
    }

    [Test]
    public void OnTriggerEnterExit_TracksPlayersAndStopsMonitor()
    {
        GameObject enemyObject = new GameObject("Enemy");
        PatrollingEnemy enemy = enemyObject.AddComponent<PatrollingEnemy>();

        GameObject rangeObject = new GameObject("Range");
        rangeObject.transform.SetParent(enemyObject.transform, false);
        rangeObject.AddComponent<BoxCollider2D>();
        EnemyAttackRange range = rangeObject.AddComponent<EnemyAttackRange>();

        TestUtilities.SetPrivateField(range, "owner", enemy);
        TestUtilities.SetPrivateField(range, "playerTag", "Player");
        TestUtilities.SetPrivateField(enemy, "isDead", true);

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();

        TestUtilities.InvokePrivateMethod(range, "OnTriggerEnter2D", playerCollider);

        System.Collections.Generic.HashSet<Collider2D> tracked =
            TestUtilities.GetPrivateField<System.Collections.Generic.HashSet<Collider2D>>(range, "trackedPlayers");
        Coroutine monitorRoutine = TestUtilities.GetPrivateField<Coroutine>(range, "monitorRoutine");

        Assert.AreEqual(1, tracked.Count);
        Assert.IsNotNull(monitorRoutine);

        TestUtilities.InvokePrivateMethod(range, "OnTriggerExit2D", playerCollider);

        tracked = TestUtilities.GetPrivateField<System.Collections.Generic.HashSet<Collider2D>>(range, "trackedPlayers");
        monitorRoutine = TestUtilities.GetPrivateField<Coroutine>(range, "monitorRoutine");

        Assert.AreEqual(0, tracked.Count);
        Assert.IsNull(monitorRoutine);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(rangeObject);
        Object.DestroyImmediate(enemyObject);
    }
}

public class StompTests
{
    [SetUp]
    public void Setup()
    {
        ScoreManager.ResetScore();
        TestUtilities.ResetSceneTransitionLevelStateManager();
    }

    [Test]
    public void OnTriggerEnter_Player_KillsPatrollingEnemy()
    {
        GameObject enemyRoot = new GameObject("EnemyRoot");
        PatrollingEnemy enemy = enemyRoot.AddComponent<PatrollingEnemy>();

        GameObject stompObject = new GameObject("Stomp");
        stompObject.transform.SetParent(enemyRoot.transform, false);
        Stomp stomp = stompObject.AddComponent<Stomp>();
        TestUtilities.SetPrivateField(stomp, "enemyRoot", enemyRoot.transform);
        TestUtilities.InvokePrivateMethod(stomp, "Awake");

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();

        TestUtilities.InvokePrivateMethod(stomp, "OnTriggerEnter2D", playerCollider);

        Assert.IsFalse(enemy.enabled);
        Assert.AreEqual(ScoreManager.PatrollingEnemyKillPoints, ScoreManager.CurrentScore);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemyRoot);
    }
}
