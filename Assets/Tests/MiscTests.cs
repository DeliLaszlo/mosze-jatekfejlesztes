using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class CutsceneManagerTests
{
    [Test]
    public void Start_ActivatesFirstPanelAndSpacePrompt()
    {
        GameObject managerObject = new GameObject("Cutscene");
        CutsceneManager manager = managerObject.AddComponent<CutsceneManager>();

        CanvasGroup panelA = CreatePanel("PanelA");
        CanvasGroup panelB = CreatePanel("PanelB");
        manager.panels = new[] { panelA, panelB };
        manager.spaceButtonUI = new GameObject("SpaceUI");
        manager.spaceButtonUI.SetActive(false);

        TestUtilities.InvokePrivateMethod(manager, "Start");

        Assert.IsTrue(manager.spaceButtonUI.activeSelf);
        Assert.IsTrue(panelA.gameObject.activeSelf);
        Assert.AreEqual(1f, panelA.alpha);
        Assert.IsFalse(panelB.gameObject.activeSelf);
        Assert.AreEqual(0f, panelB.alpha);

        Object.DestroyImmediate(manager.spaceButtonUI);
        Object.DestroyImmediate(panelA.gameObject);
        Object.DestroyImmediate(panelB.gameObject);
        Object.DestroyImmediate(managerObject);
    }

    [Test]
    public void CrossfadePanel_CompletesWithZeroDuration()
    {
        GameObject managerObject = new GameObject("Cutscene");
        CutsceneManager manager = managerObject.AddComponent<CutsceneManager>();

        CanvasGroup panelA = CreatePanel("PanelA");
        CanvasGroup panelB = CreatePanel("PanelB");
        manager.panels = new[] { panelA, panelB };
        manager.spaceButtonUI = new GameObject("SpaceUI");
        manager.spaceButtonUI.SetActive(true);

        TestUtilities.SetPrivateField(manager, "fadeDuration", 0f);

        IEnumerator routine = (IEnumerator)TestUtilities.InvokePrivateMethod(manager, "CrossfadePanel", 0, 1);
        while (routine.MoveNext()) { }

        int currentIndex = TestUtilities.GetPrivateField<int>(manager, "currentIndex");
        bool isFading = TestUtilities.GetPrivateField<bool>(manager, "isFading");

        Assert.AreEqual(1, currentIndex);
        Assert.IsFalse(isFading);
        Assert.IsFalse(panelA.gameObject.activeSelf);
        Assert.IsTrue(panelB.gameObject.activeSelf);
        Assert.IsFalse(manager.spaceButtonUI.activeSelf);

        Object.DestroyImmediate(manager.spaceButtonUI);
        Object.DestroyImmediate(panelA.gameObject);
        Object.DestroyImmediate(panelB.gameObject);
        Object.DestroyImmediate(managerObject);
    }

    private static CanvasGroup CreatePanel(string name)
    {
        GameObject panelObject = new GameObject(name);
        CanvasGroup group = panelObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        panelObject.SetActive(false);
        return group;
    }
}

public class GateHandlerTests
{
    [SetUp]
    public void Setup()
    {
        TestUtilities.ResetSceneTransitionLevelStateManager();
    }

    [Test]
    public void Awake_DisablesGateWhenStateIsSaved()
    {
        GameObject gateObject = new GameObject("Gate");
        GateHandler gate = gateObject.AddComponent<GateHandler>();

        string key = SceneTransitionLevelStateManager.BuildStateKey(gateObject, "Gate");
        SceneTransitionLevelStateManager.MarkGateDisabled(key);

        TestUtilities.InvokePrivateMethod(gate, "Awake");

        Assert.IsFalse(gateObject.activeSelf);

        Object.DestroyImmediate(gateObject);
    }

    [Test]
    public void DisableSelf_MarksGateDisabledAndDeactivates()
    {
        GameObject gateObject = new GameObject("Gate");
        GateHandler gate = gateObject.AddComponent<GateHandler>();

        TestUtilities.InvokePrivateMethod(gate, "Awake");

        gate.disableSelf();

        string key = TestUtilities.GetPrivateField<string>(gate, "gateStateKey");
        Assert.IsFalse(gateObject.activeSelf);
        Assert.IsTrue(SceneTransitionLevelStateManager.IsGateDisabled(key));

        Object.DestroyImmediate(gateObject);
    }
}

public class SceneTeleporterTests
{
    [Test]
    public void DefaultSceneToLoad_IsLevel1()
    {
        GameObject teleporterObject = new GameObject("Teleporter");
        SceneTeleporter teleporter = teleporterObject.AddComponent<SceneTeleporter>();

        Assert.AreEqual("level1", teleporter.sceneToLoad);

        Object.DestroyImmediate(teleporterObject);
    }

    [Test]
    public void OnTriggerEnter_IgnoresNonPlayerCollider()
    {
        GameObject teleporterObject = new GameObject("Teleporter");
        SceneTeleporter teleporter = teleporterObject.AddComponent<SceneTeleporter>();

        GameObject other = new GameObject("Other");
        other.tag = "Untagged";
        BoxCollider2D otherCollider = other.AddComponent<BoxCollider2D>();

        bool loaded = false;
        void SceneLoaded(Scene scene, LoadSceneMode mode) { loaded = true; }
        SceneManager.sceneLoaded += SceneLoaded;

        TestUtilities.InvokePrivateMethod(teleporter, "OnTriggerEnter2D", otherCollider);

        SceneManager.sceneLoaded -= SceneLoaded;
        Assert.IsFalse(loaded);

        Object.DestroyImmediate(other);
        Object.DestroyImmediate(teleporterObject);
    }
}
