using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class JumpKingSceneTransitionStateTests
{
    [SetUp]
    public void Setup()
    {
        TestUtilities.ResetJumpKingSceneTransitionState();
    }

    [Test]
    public void TryConsumeTransition_NoPending_ReturnsFalseAndDefaults()
    {
        bool consumed = JumpKingSceneTransitionState.TryConsumeTransition(out string entryPointId, out Vector2 velocity);

        Assert.IsFalse(consumed);
        Assert.AreEqual(SceneTransitionSpawnPoint.DefaultEntryPointId, entryPointId);
        Assert.AreEqual(Vector2.zero, velocity);
    }

    [Test]
    public void BeginTransition_StoresValuesAndConsumesOnce()
    {
        JumpKingSceneTransitionState.BeginTransition(string.Empty, new Vector2(1f, -2f));

        bool consumed = JumpKingSceneTransitionState.TryConsumeTransition(out string entryPointId, out Vector2 velocity);
        Assert.IsTrue(consumed);
        Assert.AreEqual(SceneTransitionSpawnPoint.DefaultEntryPointId, entryPointId);
        Assert.AreEqual(new Vector2(1f, -2f), velocity);

        bool consumedAgain = JumpKingSceneTransitionState.TryConsumeTransition(out _, out _);
        Assert.IsFalse(consumedAgain);
    }

    [Test]
    public void LockTriggers_DisablesTriggerWhenDurationPositive()
    {
        Assert.IsTrue(JumpKingSceneTransitionState.CanTriggerTransition);

        JumpKingSceneTransitionState.LockTriggers(1f);
        Assert.IsFalse(JumpKingSceneTransitionState.CanTriggerTransition);

        TestUtilities.ResetJumpKingSceneTransitionState();
        Assert.IsTrue(JumpKingSceneTransitionState.CanTriggerTransition);
    }
}

public class SceneTransitionLevelStateManagerTests
{
    [SetUp]
    public void Setup()
    {
        TestUtilities.ResetSceneTransitionLevelStateManager();
    }

    [Test]
    public void BuildStateKey_InvalidInput_ReturnsEmpty()
    {
        GameObject temp = new GameObject("Temp");
        Assert.AreEqual(string.Empty, SceneTransitionLevelStateManager.BuildStateKey(null, "Gate"));
        Assert.AreEqual(string.Empty, SceneTransitionLevelStateManager.BuildStateKey(temp, string.Empty));
        Object.DestroyImmediate(temp);
    }

    [Test]
    public void BuildStateKey_IncludesCategoryAndObjectName()
    {
        GameObject temp = new GameObject("GateRoot");
        string key = SceneTransitionLevelStateManager.BuildStateKey(temp, "Gate");

        Assert.IsTrue(key.Contains("Gate|"));
        Assert.IsTrue(key.Contains("GateRoot"));

        Object.DestroyImmediate(temp);
    }

    [Test]
    public void MarkAndCheckUrn_BehavesAsExpected()
    {
        string key = "urn-key";
        Assert.IsFalse(SceneTransitionLevelStateManager.IsUrnBroken(key));
        SceneTransitionLevelStateManager.MarkUrnBroken(key);
        Assert.IsTrue(SceneTransitionLevelStateManager.IsUrnBroken(key));
    }

    [Test]
    public void MarkAndCheckEnemy_BehavesAsExpected()
    {
        string key = "enemy-key";
        Assert.IsFalse(SceneTransitionLevelStateManager.IsEnemyDefeated(key));
        SceneTransitionLevelStateManager.MarkEnemyDefeated(key);
        Assert.IsTrue(SceneTransitionLevelStateManager.IsEnemyDefeated(key));
    }

    [Test]
    public void MarkAndCheckGate_BehavesAsExpected()
    {
        string key = "gate-key";
        Assert.IsFalse(SceneTransitionLevelStateManager.IsGateDisabled(key));
        SceneTransitionLevelStateManager.MarkGateDisabled(key);
        Assert.IsTrue(SceneTransitionLevelStateManager.IsGateDisabled(key));
    }

    [Test]
    public void ApplySavedStateSilently_TogglesFlag()
    {
        bool wasApplyingInside = false;

        SceneTransitionLevelStateManager.ApplySavedStateSilently(() =>
        {
            wasApplyingInside = SceneTransitionLevelStateManager.IsApplyingSavedState;
        });

        Assert.IsTrue(wasApplyingInside);
        Assert.IsFalse(SceneTransitionLevelStateManager.IsApplyingSavedState);
    }

    [Test]
    public void DisableForSavedState_DeactivatesObject()
    {
        GameObject target = new GameObject("Target");
        target.SetActive(true);

        SceneTransitionLevelStateManager.DisableForSavedState(target);

        Assert.IsFalse(target.activeSelf);
        Assert.IsFalse(SceneTransitionLevelStateManager.IsApplyingSavedState);

        Object.DestroyImmediate(target);
    }
}

public class SceneTransitionSpawnPointTests
{
    [Test]
    public void EntryPointId_DefaultsWhenBlank()
    {
        GameObject spawnObject = new GameObject("Spawn");
        SceneTransitionSpawnPoint spawn = spawnObject.AddComponent<SceneTransitionSpawnPoint>();

        TestUtilities.SetPrivateField(spawn, "entryPointId", "  ");

        Assert.AreEqual(SceneTransitionSpawnPoint.DefaultEntryPointId, spawn.EntryPointId);

        Object.DestroyImmediate(spawnObject);
    }
}

public class PlayerSceneTransitionHandlerTests
{
    [SetUp]
    public void Setup()
    {
        TestUtilities.ResetJumpKingSceneTransitionState();
    }

    [Test]
    public void Start_MovesToSpawnPointAndAppliesVelocity()
    {
        GameObject spawnObject = new GameObject("SpawnPoint");
        SceneTransitionSpawnPoint spawn = spawnObject.AddComponent<SceneTransitionSpawnPoint>();
        TestUtilities.SetPrivateField(spawn, "entryPointId", "EntryA");
        spawn.transform.position = new Vector3(5f, 2f, 0f);

        GameObject player = new GameObject("Player");
        player.AddComponent<PlayerController>();
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        PlayerSceneTransitionHandler handler = player.AddComponent<PlayerSceneTransitionHandler>();
        TestUtilities.SetPrivateField(handler, "applyCarryVelocity", true);

        JumpKingSceneTransitionState.BeginTransition("EntryA", new Vector2(2f, 3f));
        TestUtilities.InvokePrivateMethod(handler, "Start");

        Assert.AreEqual(spawn.transform.position, player.transform.position);
        Assert.AreEqual(new Vector2(2f, 3f), rb.linearVelocity);

        bool consumed = JumpKingSceneTransitionState.TryConsumeTransition(out _, out _);
        Assert.IsFalse(consumed);

        Object.DestroyImmediate(spawnObject);
        Object.DestroyImmediate(player);
    }

    [Test]
    public void Start_LogsWarningWhenSpawnPointMissing()
    {
        GameObject player = new GameObject("Player");
        player.AddComponent<PlayerController>();
        player.AddComponent<Rigidbody2D>();
        PlayerSceneTransitionHandler handler = player.AddComponent<PlayerSceneTransitionHandler>();

        JumpKingSceneTransitionState.BeginTransition("Missing", Vector2.zero);

        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("No SceneTransitionSpawnPoint found with id 'Missing'"));
        TestUtilities.InvokePrivateMethod(handler, "Start");

        Object.DestroyImmediate(player);
    }

    [Test]
    public void Start_NoPendingTransition_DoesNothing()
    {
        GameObject spawnObject = new GameObject("SpawnPoint");
        SceneTransitionSpawnPoint spawn = spawnObject.AddComponent<SceneTransitionSpawnPoint>();
        TestUtilities.SetPrivateField(spawn, "entryPointId", "EntryA");
        spawn.transform.position = new Vector3(3f, 1f, 0f);

        GameObject player = new GameObject("Player");
        player.AddComponent<PlayerController>();
        PlayerSceneTransitionHandler handler = player.AddComponent<PlayerSceneTransitionHandler>();
        Vector3 originalPosition = player.transform.position;

        TestUtilities.InvokePrivateMethod(handler, "Start");

        Assert.AreEqual(originalPosition, player.transform.position);

        Object.DestroyImmediate(spawnObject);
        Object.DestroyImmediate(player);
    }
}

public class SceneEdgeTransitionTriggerTests
{
    [SetUp]
    public void Setup()
    {
        TestUtilities.ResetJumpKingSceneTransitionState();
    }

    [Test]
    public void OnTriggerEnter_IgnoresNonPlayer()
    {
        GameObject triggerObject = new GameObject("Trigger");
        triggerObject.AddComponent<BoxCollider2D>();
        SceneEdgeTransitionTrigger trigger = triggerObject.AddComponent<SceneEdgeTransitionTrigger>();

        GameObject other = new GameObject("Other");
        other.tag = "Untagged";
        BoxCollider2D otherCollider = other.AddComponent<BoxCollider2D>();

        TestUtilities.SetPrivateField(trigger, "targetSceneName", "Any");
        TestUtilities.InvokePrivateMethod(trigger, "OnTriggerEnter2D", otherCollider);

        bool consumed = JumpKingSceneTransitionState.TryConsumeTransition(out _, out _);
        Assert.IsFalse(consumed);

        Object.DestroyImmediate(other);
        Object.DestroyImmediate(triggerObject);
    }

    [Test]
    public void OnTriggerEnter_LogsWarningWhenTargetSceneMissing()
    {
        GameObject triggerObject = new GameObject("Trigger");
        triggerObject.AddComponent<BoxCollider2D>();
        SceneEdgeTransitionTrigger trigger = triggerObject.AddComponent<SceneEdgeTransitionTrigger>();

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.AddComponent<PlayerController>();
        player.AddComponent<Rigidbody2D>();
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();

        TestUtilities.SetPrivateField(trigger, "targetSceneName", string.Empty);

        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("Target scene is not set"));
        TestUtilities.InvokePrivateMethod(trigger, "OnTriggerEnter2D", playerCollider);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(triggerObject);
    }

    [Test]
    public void OnTriggerEnter_RespectsAscendingVelocityRequirement()
    {
        GameObject triggerObject = new GameObject("Trigger");
        triggerObject.AddComponent<BoxCollider2D>();
        SceneEdgeTransitionTrigger trigger = triggerObject.AddComponent<SceneEdgeTransitionTrigger>();

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.AddComponent<PlayerController>();
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();

        rb.linearVelocity = new Vector2(0f, -1f);

        TestUtilities.SetPrivateField(trigger, "targetSceneName", "Any");
        TestUtilities.SetPrivateField(trigger, "requireAscendingVelocity", true);

        TestUtilities.InvokePrivateMethod(trigger, "OnTriggerEnter2D", playerCollider);

        bool consumed = JumpKingSceneTransitionState.TryConsumeTransition(out _, out _);
        Assert.IsFalse(consumed);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(triggerObject);
    }

    [Test]
    public void OnTriggerEnter_IgnoresWhenTriggerLocked()
    {
        GameObject triggerObject = new GameObject("Trigger");
        triggerObject.AddComponent<BoxCollider2D>();
        SceneEdgeTransitionTrigger trigger = triggerObject.AddComponent<SceneEdgeTransitionTrigger>();

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.AddComponent<PlayerController>();
        player.AddComponent<Rigidbody2D>();
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();

        TestUtilities.SetPrivateStaticField(typeof(JumpKingSceneTransitionState), "nextTriggerAllowedAt", Time.unscaledTime + 10f);
        TestUtilities.SetPrivateField(trigger, "targetSceneName", "Any");

        TestUtilities.InvokePrivateMethod(trigger, "OnTriggerEnter2D", playerCollider);

        bool consumed = JumpKingSceneTransitionState.TryConsumeTransition(out _, out _);
        Assert.IsFalse(consumed);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(triggerObject);
    }
}
