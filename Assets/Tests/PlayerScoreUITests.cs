using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerScoreUITests
{
    private GameObject uiObject;
    private PlayerScoreUI playerScoreUI;
    private Component tmpText;

    [SetUp]
    public void Setup()
    {
        ScoreManager.ResetScore();
        uiObject = new GameObject("ScoreUI");
        tmpText = CreateTMPText(uiObject);
        playerScoreUI = uiObject.AddComponent<PlayerScoreUI>();
        TestUtilities.SetPrivateField(playerScoreUI, "playerScoreText", tmpText);
        TestUtilities.InvokePrivateMethod(playerScoreUI, "OnDisable");
    }

    [TearDown]
    public void TearDown()
    {
        if (playerScoreUI != null)
        {
            TestUtilities.InvokePrivateMethod(playerScoreUI, "OnDisable");
        }

        if (uiObject != null)
        {
            Object.DestroyImmediate(uiObject);
        }
    }

    [Test]
    public void Awake_AssignsTextComponentWhenMissing()
    {
        TestUtilities.SetPrivateField<Component>(playerScoreUI, "playerScoreText", null);
        TestUtilities.InvokePrivateMethod(playerScoreUI, "Awake");

        Component assigned = TestUtilities.GetPrivateField<Component>(playerScoreUI, "playerScoreText");
        Assert.IsNotNull(assigned);
    }

    [Test]
    public void OnEnable_UpdatesTextAndSubscribes()
    {
        ScoreManager.AddPoints(25);

        TestUtilities.InvokePrivateMethod(playerScoreUI, "OnEnable");
        Assert.AreEqual("25", GetTMPText(tmpText));

        ScoreManager.AddPoints(5);
        Assert.AreEqual("30", GetTMPText(tmpText));
    }

    [Test]
    public void OnDisable_UnsubscribesFromScoreChanged()
    {
        TestUtilities.InvokePrivateMethod(playerScoreUI, "OnEnable");

        ScoreManager.AddPoints(10);
        string beforeDisable = GetTMPText(tmpText);

        TestUtilities.InvokePrivateMethod(playerScoreUI, "OnDisable");
        ScoreManager.AddPoints(5);

        Assert.AreEqual(beforeDisable, GetTMPText(tmpText));
    }

    private static Component CreateTMPText(GameObject owner)
    {
        Type textType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
        if (textType == null)
        {
            Assert.Ignore("TextMeshProUGUI type not found.");
        }

        return owner.AddComponent(textType);
    }

    private static string GetTMPText(Component textComponent)
    {
        PropertyInfo property = textComponent.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
        Assert.IsNotNull(property, "TextMeshPro text property not found.");
        return property.GetValue(textComponent, null) as string;
    }
}
