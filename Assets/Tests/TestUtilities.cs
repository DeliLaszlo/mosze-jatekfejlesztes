using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public static class TestUtilities
{
    private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

    public static void SetPrivateField<T>(object target, string fieldName, T value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, InstanceFlags);
        Assert.IsNotNull(field, "Field not found: " + fieldName);
        field.SetValue(target, value);
    }

    public static T GetPrivateField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, InstanceFlags);
        Assert.IsNotNull(field, "Field not found: " + fieldName);
        return (T)field.GetValue(target);
    }

    public static void SetPrivateStaticField<T>(Type type, string fieldName, T value)
    {
        FieldInfo field = type.GetField(fieldName, StaticFlags);
        Assert.IsNotNull(field, "Static field not found: " + fieldName);
        field.SetValue(null, value);
    }

    public static T GetPrivateStaticField<T>(Type type, string fieldName)
    {
        FieldInfo field = type.GetField(fieldName, StaticFlags);
        Assert.IsNotNull(field, "Static field not found: " + fieldName);
        return (T)field.GetValue(null);
    }

    public static object InvokePrivateMethod(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, InstanceFlags);
        Assert.IsNotNull(method, "Method not found: " + methodName);
        return method.Invoke(target, args);
    }

    public static object InvokePrivateStaticMethod(Type type, string methodName, params object[] args)
    {
        MethodInfo method = type.GetMethod(methodName, StaticFlags);
        Assert.IsNotNull(method, "Static method not found: " + methodName);
        return method.Invoke(null, args);
    }

    public static void ResetPlayerHealthManagerStaticState()
    {
        SetPrivateStaticField(typeof(PlayerHealthManager), "hasPersistedHealth", false);
        SetPrivateStaticField(typeof(PlayerHealthManager), "persistedHealth", 0);
    }

    public static void ResetJumpKingSceneTransitionState()
    {
        SetPrivateStaticField(typeof(JumpKingSceneTransitionState), "hasPendingTransition", false);
        SetPrivateStaticField(typeof(JumpKingSceneTransitionState), "pendingEntryPointId", SceneTransitionSpawnPoint.DefaultEntryPointId);
        SetPrivateStaticField(typeof(JumpKingSceneTransitionState), "pendingVelocity", Vector2.zero);
        SetPrivateStaticField(typeof(JumpKingSceneTransitionState), "nextTriggerAllowedAt", 0f);
    }

    public static void ResetSceneTransitionLevelStateManager()
    {
        ClearHashSet(typeof(SceneTransitionLevelStateManager), "brokenUrnKeys");
        ClearHashSet(typeof(SceneTransitionLevelStateManager), "defeatedEnemyKeys");
        ClearHashSet(typeof(SceneTransitionLevelStateManager), "disabledGateKeys");
        SetPrivateStaticField(typeof(SceneTransitionLevelStateManager), "savedStateApplyDepth", 0);
    }

    private static void ClearHashSet(Type type, string fieldName)
    {
        FieldInfo field = type.GetField(fieldName, StaticFlags);
        Assert.IsNotNull(field, "HashSet field not found: " + fieldName);
        HashSet<string> set = field.GetValue(null) as HashSet<string>;
        Assert.IsNotNull(set, "HashSet field not available: " + fieldName);
        set.Clear();
    }
}
