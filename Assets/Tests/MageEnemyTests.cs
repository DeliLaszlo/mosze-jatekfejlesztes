using System.Collections;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class MageEnemyControllerTests
{
    [SetUp]
    public void Setup()
    {
        ScoreManager.ResetScore();
        TestUtilities.ResetSceneTransitionLevelStateManager();
        TestUtilities.ResetPlayerHealthManagerStaticState();
    }

    [Test]
    public void Start_InitializesStateAndHealth()
    {
        GameObject mageObject = new GameObject("Mage");
        MageEnemyController mage = mageObject.AddComponent<MageEnemyController>();
        mage.maxHealth = 5;

        Transform pointA = new GameObject("PointA").transform;
        Transform pointB = new GameObject("PointB").transform;
        pointA.position = Vector3.zero;
        pointB.position = new Vector3(5f, 0f, 0f);
        mage.teleportPoints = new[] { pointA, pointB };
        mage.rootTransform = mageObject.transform;

        BoxCollider2D slamHitbox = mageObject.AddComponent<BoxCollider2D>();
        TestUtilities.SetPrivateField(mage, "slamHitbox", slamHitbox);

        TestUtilities.InvokePrivateMethod(mage, "Start");

        Assert.AreEqual(MageEnemyController.MageState.Shielded, mage.currentState);
        Assert.IsTrue(mage.isInvulnerable);

        int currentHealth = TestUtilities.GetPrivateField<int>(mage, "currentHealth");
        int currentPointIndex = TestUtilities.GetPrivateField<int>(mage, "currentPointIndex");
        Assert.AreEqual(mage.maxHealth, currentHealth);
        Assert.AreEqual(0, currentPointIndex);

        Object.DestroyImmediate(pointA.gameObject);
        Object.DestroyImmediate(pointB.gameObject);
        Object.DestroyImmediate(mageObject);
    }

    [Test]
    public void HandleStomp_Vulnerable_KillsMage()
    {
        GameObject mageObject = new GameObject("Mage");
        MageEnemyController mage = mageObject.AddComponent<MageEnemyController>();

        BoxCollider2D slamHitbox = mageObject.AddComponent<BoxCollider2D>();
        TestUtilities.SetPrivateField(mage, "slamHitbox", slamHitbox);
        TestUtilities.SetPrivateField(mage, "enemyStateKey", "MageKey");

        mage.currentState = MageEnemyController.MageState.Vulnerable;
        TestUtilities.SetPrivateField(mage, "currentHealth", 1);

        GameObject player = new GameObject("Player");
        player.AddComponent<PlayerHealthManager>();

        mage.HandleStomp(player);

        Assert.AreEqual(MageEnemyController.MageState.Dead, mage.currentState);
        Assert.IsTrue(mage.isInvulnerable);
        Assert.IsFalse(slamHitbox.enabled);
        Assert.AreEqual(ScoreManager.MageEnemyKillPoints, ScoreManager.CurrentScore);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(mageObject);
    }

    [Test]
    public void HandleStomp_Shielded_DamagesPlayer()
    {
        TestUtilities.ResetPlayerHealthManagerStaticState();
        GameObject mageObject = new GameObject("Mage");
        MageEnemyController mage = mageObject.AddComponent<MageEnemyController>();
        mage.currentState = MageEnemyController.MageState.Shielded;

        Transform pointA = new GameObject("PointA").transform;
        Transform pointB = new GameObject("PointB").transform;
        mage.teleportPoints = new[] { pointA, pointB };
        TestUtilities.SetPrivateField(mage, "currentPointIndex", 0);

        GameObject player = new GameObject("Player");
        PlayerHealthManager health = player.AddComponent<PlayerHealthManager>();
        TestUtilities.SetPrivateField(health, "maxHealth", 3);
        TestUtilities.SetPrivateField(health, "currentHealth", 3);
        TestUtilities.SetPrivateField(health, "isDead", false);
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(5f, 0f);

        int before = health.CurrentHealth;

        IEnumerator routine = (IEnumerator)TestUtilities.InvokePrivateMethod(mage, "PunishPlayerSequence", player);
        while (routine.MoveNext()) { }

        Assert.AreEqual(before - 1, health.CurrentHealth);
        Assert.AreEqual(Vector2.zero, rb.linearVelocity);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(pointA.gameObject);
        Object.DestroyImmediate(pointB.gameObject);
        Object.DestroyImmediate(mageObject);
    }

    [Test]
    public void OnStaffHitGround_DamagesPlayerInHitbox()
    {
        TestUtilities.ResetPlayerHealthManagerStaticState();
        GameObject mageObject = new GameObject("Mage");
        MageEnemyController mage = mageObject.AddComponent<MageEnemyController>();

        BoxCollider2D slamHitbox = mageObject.AddComponent<BoxCollider2D>();
        slamHitbox.size = new Vector2(2f, 2f);
        slamHitbox.offset = Vector2.zero;
        TestUtilities.SetPrivateField(mage, "slamHitbox", slamHitbox);

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = mageObject.transform.position;
        player.AddComponent<BoxCollider2D>();
        PlayerHealthManager health = player.AddComponent<PlayerHealthManager>();
        TestUtilities.SetPrivateField(health, "maxHealth", 3);
        TestUtilities.SetPrivateField(health, "currentHealth", 3);
        TestUtilities.SetPrivateField(health, "isDead", false);

        int before = health.CurrentHealth;
        mage.OnStaffHitGround();

        Assert.AreEqual(before - 1, health.CurrentHealth);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(mageObject);
    }
}

public class MageStompReceiverTests
{
    [SetUp]
    public void Setup()
    {
        ScoreManager.ResetScore();
        TestUtilities.ResetSceneTransitionLevelStateManager();
        TestUtilities.ResetPlayerHealthManagerStaticState();
    }

    [Test]
    public void OnTriggerEnter_Player_ForwardsToMage()
    {
        GameObject mageObject = new GameObject("Mage");
        MageEnemyController mage = mageObject.AddComponent<MageEnemyController>();
        mage.currentState = MageEnemyController.MageState.Vulnerable;
        TestUtilities.SetPrivateField(mage, "currentHealth", 1);

        GameObject receiverObject = new GameObject("Receiver");
        MageStompReciever receiver = receiverObject.AddComponent<MageStompReciever>();
        receiver.mageController = mage;

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();

        TestUtilities.InvokePrivateMethod(receiver, "OnTriggerEnter2D", playerCollider);

        Assert.AreEqual(MageEnemyController.MageState.Dead, mage.currentState);
        Assert.AreEqual(ScoreManager.MageEnemyKillPoints, ScoreManager.CurrentScore);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(receiverObject);
        Object.DestroyImmediate(mageObject);
    }
}
