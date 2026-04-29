using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class GoldUrnInteractableTests
{
    [SetUp]
    public void Setup()
    {
        ScoreManager.ResetScore();
        TestUtilities.ResetSceneTransitionLevelStateManager();
    }

    [Test]
    public void Reset_SetsTriggerAndRootWhenMissing()
    {
        GameObject urnObject = new GameObject("Urn");
        BoxCollider2D collider = urnObject.AddComponent<BoxCollider2D>();
        GoldUrnInteractable urn = urnObject.AddComponent<GoldUrnInteractable>();

        TestUtilities.InvokePrivateMethod(urn, "Reset");

        Assert.IsTrue(collider.isTrigger);
        GameObject root = TestUtilities.GetPrivateField<GameObject>(urn, "urnRootToDisable");
        Assert.AreEqual(urnObject, root);

        Object.DestroyImmediate(urnObject);
    }

    [Test]
    public void OnTriggerEnterExit_TogglesUIAndCount()
    {
        GameObject urnObject = new GameObject("Urn");
        urnObject.AddComponent<BoxCollider2D>();
        GoldUrnInteractable urn = urnObject.AddComponent<GoldUrnInteractable>();

        GameObject ui = new GameObject("UI");
        ui.SetActive(false);

        TestUtilities.SetPrivateField(urn, "interactUI", ui);
        TestUtilities.SetPrivateField(urn, "playerTag", "Player");
        TestUtilities.SetPrivateField(urn, "isBroken", false);
        TestUtilities.SetPrivateField(urn, "playersInRange", 0);

        GameObject player = CreatePlayerCollider();
        Collider2D playerCollider = player.GetComponent<Collider2D>();

        TestUtilities.InvokePrivateMethod(urn, "OnTriggerEnter2D", playerCollider);
        Assert.AreEqual(1, TestUtilities.GetPrivateField<int>(urn, "playersInRange"));
        Assert.IsTrue(ui.activeSelf);

        TestUtilities.InvokePrivateMethod(urn, "OnTriggerExit2D", playerCollider);
        Assert.AreEqual(0, TestUtilities.GetPrivateField<int>(urn, "playersInRange"));
        Assert.IsFalse(ui.activeSelf);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(ui);
        Object.DestroyImmediate(urnObject);
    }

    [Test]
    public void BreakUrn_DisablesRootAndAddsScore()
    {
        GameObject urnObject = new GameObject("Urn");
        urnObject.AddComponent<BoxCollider2D>();
        GoldUrnInteractable urn = urnObject.AddComponent<GoldUrnInteractable>();

        GameObject ui = new GameObject("UI");
        ui.SetActive(true);

        GameObject root = new GameObject("Root");
        root.SetActive(true);

        string key = "TestUrnKey_" + System.Guid.NewGuid().ToString("N");

        TestUtilities.SetPrivateField(urn, "interactUI", ui);
        TestUtilities.SetPrivateField(urn, "urnRootToDisable", root);
        TestUtilities.SetPrivateField(urn, "urnStateKey", key);

        TestUtilities.InvokePrivateMethod(urn, "BreakUrn");

        Assert.IsTrue(SceneTransitionLevelStateManager.IsUrnBroken(key));
        Assert.IsFalse(root.activeSelf);
        Assert.IsFalse(ui.activeSelf);
        Assert.AreEqual(ScoreManager.GoldUrnBreakPoints, ScoreManager.CurrentScore);

        Object.DestroyImmediate(root);
        Object.DestroyImmediate(ui);
        Object.DestroyImmediate(urnObject);
    }

    [Test]
    public void Awake_DisablesIfAlreadyBroken()
    {
        GameObject urnObject = new GameObject("Urn");
        urnObject.AddComponent<BoxCollider2D>();

        string key = SceneTransitionLevelStateManager.BuildStateKey(urnObject, "Urn");
        SceneTransitionLevelStateManager.MarkUrnBroken(key);

        GoldUrnInteractable urn = urnObject.AddComponent<GoldUrnInteractable>();

        TestUtilities.InvokePrivateMethod(urn, "Awake");

        Assert.IsFalse(urnObject.activeSelf);

        Object.DestroyImmediate(urnObject);
    }

    private static GameObject CreatePlayerCollider()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.AddComponent<BoxCollider2D>();
        return player;
    }
}
