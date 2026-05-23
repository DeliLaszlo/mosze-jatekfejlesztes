using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using System.Collections;
using System.Reflection;
using Object = UnityEngine.Object;

public class PlayerHealthManagerTests
{
    [SetUp]
    public void Setup()
    {
        TestUtilities.ResetPlayerHealthManagerStaticState();
    }

    [Test]
    public void Awake_SetsCurrentHealthToMax()
    {
        TestUtilities.ResetPlayerHealthManagerStaticState();
        GameObject player = new GameObject("Player");
        PlayerHealthManager health = player.AddComponent<PlayerHealthManager>();
        TestUtilities.InvokePrivateMethod(health, "Awake");

        Assert.AreEqual(health.MaxHealth, health.CurrentHealth);

        Object.DestroyImmediate(player);
    }

    [Test]
    public void TakeDamage_DecreasesHealthAndRaisesEvent()
    {
        TestUtilities.ResetPlayerHealthManagerStaticState();
        GameObject player = new GameObject("Player");
        PlayerHealthManager health = player.AddComponent<PlayerHealthManager>();
        TestUtilities.InvokePrivateMethod(health, "Awake");

        int lastCurrent = -1;
        int lastMax = -1;
        health.HealthChanged += (current, max) =>
        {
            lastCurrent = current;
            lastMax = max;
        };

        health.TakeDamage();

        Assert.AreEqual(health.CurrentHealth, lastCurrent);
        Assert.AreEqual(health.MaxHealth, lastMax);

        Object.DestroyImmediate(player);
    }

    [Test]
    public void TakeDamage_TriggersDeathAndDisablesComponents()
    {
        TestUtilities.ResetPlayerHealthManagerStaticState();
        GameObject player = new GameObject("Player");
        PlayerController controller = player.AddComponent<PlayerController>();
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        BoxCollider2D colliderA = player.AddComponent<BoxCollider2D>();
        CircleCollider2D colliderB = player.AddComponent<CircleCollider2D>();
        PlayerHealthManager health = player.AddComponent<PlayerHealthManager>();
        
        TestUtilities.InvokePrivateMethod(health, "Awake");

        FieldInfo invincibleField = typeof(PlayerHealthManager).GetField("isInvincible", BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < health.MaxHealth; i++)
        {
            health.TakeDamage();
            invincibleField.SetValue(health, false);
        }

        Assert.IsTrue(health.IsDead);
        Assert.IsFalse(controller.enabled);

        MethodInfo routineMethod = typeof(PlayerHealthManager).GetMethod("HandleDeathPhysicsRoutine", BindingFlags.NonPublic | BindingFlags.Instance);
        IEnumerator routine = (IEnumerator)routineMethod.Invoke(health, null);
        
        while (routine.MoveNext()) { }

        Assert.IsFalse(colliderA.enabled);
        Assert.IsFalse(colliderB.enabled);
        Assert.IsFalse(rb.simulated);

        Object.DestroyImmediate(player);
    }

    [Test]
    public void PersistedHealth_AppliesToNewInstance()
    {
        TestUtilities.ResetPlayerHealthManagerStaticState();
        GameObject firstPlayer = new GameObject("PlayerA");
        PlayerHealthManager firstHealth = firstPlayer.AddComponent<PlayerHealthManager>();
        TestUtilities.InvokePrivateMethod(firstHealth, "Awake");
        firstHealth.TakeDamage();
        int expected = firstHealth.CurrentHealth;

        Object.DestroyImmediate(firstPlayer);

        GameObject secondPlayer = new GameObject("PlayerB");
        PlayerHealthManager secondHealth = secondPlayer.AddComponent<PlayerHealthManager>();
        TestUtilities.InvokePrivateMethod(secondHealth, "Awake");

        Assert.AreEqual(expected, secondHealth.CurrentHealth);

        Object.DestroyImmediate(secondPlayer);
    }
}

public class PlayerHealthBlocksUITests
{
    [SetUp]
    public void Setup()
    {
        TestUtilities.ResetPlayerHealthManagerStaticState();
    }

    [Test]
    public void Start_BuildsBlocksForMaxHealth()
    {
        GameObject player = new GameObject("Player");
        PlayerHealthManager health = player.AddComponent<PlayerHealthManager>();
        TestUtilities.SetPrivateField(health, "maxHealth", 4);
        TestUtilities.SetPrivateField(health, "currentHealth", 4);

        GameObject uiObject = new GameObject("HealthUI", typeof(RectTransform));
        PlayerHealthBlocksUI ui = uiObject.AddComponent<PlayerHealthBlocksUI>();

        TestUtilities.SetPrivateField(ui, "playerHealth", health);
        TestUtilities.SetPrivateField(ui, "blocksContainer", uiObject.GetComponent<RectTransform>());

        TestUtilities.InvokePrivateMethod(ui, "Awake");
        TestUtilities.InvokePrivateMethod(ui, "Start");

        Assert.AreEqual(4, uiObject.transform.childCount);

        Object.DestroyImmediate(uiObject);
        Object.DestroyImmediate(player);
    }

    [Test]
    public void RefreshBlocks_UsesExpectedColors()
    {
        GameObject player = new GameObject("Player");
        PlayerHealthManager health = player.AddComponent<PlayerHealthManager>();
        TestUtilities.SetPrivateField(health, "maxHealth", 4);
        TestUtilities.SetPrivateField(health, "currentHealth", 4);

        GameObject uiObject = new GameObject("HealthUI", typeof(RectTransform));
        PlayerHealthBlocksUI ui = uiObject.AddComponent<PlayerHealthBlocksUI>();

        TestUtilities.SetPrivateField(ui, "playerHealth", health);
        TestUtilities.SetPrivateField(ui, "blocksContainer", uiObject.GetComponent<RectTransform>());

        TestUtilities.InvokePrivateMethod(ui, "Awake");
        TestUtilities.InvokePrivateMethod(ui, "Start");

        TestUtilities.InvokePrivateMethod(ui, "RefreshBlocks", 2, 4);

        Color mediumColor = TestUtilities.GetPrivateField<Color>(ui, "mediumHealthColor");
        Color emptyColor = TestUtilities.GetPrivateField<Color>(ui, "emptyColor");

        Image first = uiObject.transform.GetChild(0).GetComponent<Image>();
        Image last = uiObject.transform.GetChild(3).GetComponent<Image>();

        Assert.AreEqual(mediumColor, first.color);
        Assert.AreEqual(emptyColor, last.color);

        Object.DestroyImmediate(uiObject);
        Object.DestroyImmediate(player);
    }

    [Test]
    public void Awake_AddsHorizontalLayoutGroup()
    {
        GameObject uiObject = new GameObject("HealthUI", typeof(RectTransform));
        PlayerHealthBlocksUI ui = uiObject.AddComponent<PlayerHealthBlocksUI>();

        TestUtilities.InvokePrivateMethod(ui, "Awake");

        HorizontalLayoutGroup layout = uiObject.GetComponent<HorizontalLayoutGroup>();
        Assert.IsNotNull(layout);

        Object.DestroyImmediate(uiObject);
    }
}

public class PlayerControllerTests
{
    [Test]
    public void ApplyFacing_UsesVisualRootScaleWhenAssigned()
    {
        GameObject player = new GameObject("Player");
        GameObject visualRoot = new GameObject("VisualRoot");
        visualRoot.transform.SetParent(player.transform, false);
        visualRoot.AddComponent<SpriteRenderer>();

        PlayerController controller = player.AddComponent<PlayerController>();
        TestUtilities.SetPrivateField(controller, "visualRoot", visualRoot.transform);

        TestUtilities.InvokePrivateMethod(controller, "Awake");

        TestUtilities.InvokePrivateMethod(controller, "ApplyFacing", -1);
        Assert.Less(visualRoot.transform.localScale.x, 0f);

        TestUtilities.InvokePrivateMethod(controller, "ApplyFacing", 1);
        Assert.Greater(visualRoot.transform.localScale.x, 0f);

        Object.DestroyImmediate(player);
    }

    [Test]
    public void ApplyFacing_UsesSpriteRendererWhenVisualRootIsTransform()
    {
        GameObject player = new GameObject("Player");
        SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
        PlayerController controller = player.AddComponent<PlayerController>();

        TestUtilities.SetPrivateField(controller, "visualRoot", player.transform);
        TestUtilities.InvokePrivateMethod(controller, "Awake");

        TestUtilities.InvokePrivateMethod(controller, "ApplyFacing", -1);
        Assert.IsTrue(renderer.flipX);

        Object.DestroyImmediate(player);
    }

    [Test]
    public void UpdateChargePose_ModifiesVisualRootAndTint()
    {
        GameObject player = new GameObject("Player");
        GameObject visualRoot = new GameObject("VisualRoot");
        visualRoot.transform.SetParent(player.transform, false);
        SpriteRenderer renderer = visualRoot.AddComponent<SpriteRenderer>();

        PlayerController controller = player.AddComponent<PlayerController>();
        TestUtilities.SetPrivateField(controller, "visualRoot", visualRoot.transform);

        TestUtilities.InvokePrivateMethod(controller, "Awake");

        Vector3 baseScale = TestUtilities.GetPrivateField<Vector3>(controller, "baseVisualLocalScale");
        Color baseColor = TestUtilities.GetPrivateField<Color>(controller, "baseSpriteColor");

        TestUtilities.SetPrivateField(controller, "useProceduralChargePose", true);
        TestUtilities.SetPrivateField(controller, "isGrounded", true);
        TestUtilities.SetPrivateField(controller, "isCharging", true);
        TestUtilities.SetPrivateField(controller, "jumpCharge", 1f);
        TestUtilities.SetPrivateField(controller, "chargePoseWeight", 1f);

        TestUtilities.InvokePrivateMethod(controller, "UpdateChargePose");

        Assert.Less(visualRoot.transform.localScale.y, baseScale.y);
        Assert.AreNotEqual(baseColor, renderer.color);

        Object.DestroyImmediate(player);
    }

    [Test]
    public void OnDisable_ResetsChargePoseVisuals()
    {
        GameObject player = new GameObject("Player");
        GameObject visualRoot = new GameObject("VisualRoot");
        visualRoot.transform.SetParent(player.transform, false);
        SpriteRenderer renderer = visualRoot.AddComponent<SpriteRenderer>();

        PlayerController controller = player.AddComponent<PlayerController>();
        TestUtilities.SetPrivateField(controller, "visualRoot", visualRoot.transform);

        TestUtilities.InvokePrivateMethod(controller, "Awake");

        Vector3 baseScale = TestUtilities.GetPrivateField<Vector3>(controller, "baseVisualLocalScale");
        Vector3 basePosition = TestUtilities.GetPrivateField<Vector3>(controller, "baseVisualLocalPosition");
        Color baseColor = TestUtilities.GetPrivateField<Color>(controller, "baseSpriteColor");

        visualRoot.transform.localScale = baseScale * 1.3f;
        visualRoot.transform.localPosition = basePosition + Vector3.up * 0.2f;
        renderer.color = Color.red;
        TestUtilities.SetPrivateField(controller, "chargePoseWeight", 1f);

        TestUtilities.InvokePrivateMethod(controller, "OnDisable");

        Assert.AreEqual(baseScale, visualRoot.transform.localScale);
        Assert.AreEqual(basePosition, visualRoot.transform.localPosition);
        Assert.AreEqual(baseColor, renderer.color);

        Object.DestroyImmediate(player);
    }
}
