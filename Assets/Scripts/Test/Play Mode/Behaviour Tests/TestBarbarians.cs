using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using game.assets;
using static game.assets.utilities.GameUtils;

public class TestBarbarians
{
    private string sceneName = "TestBarbarians.unity";

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        EditorSceneManager.LoadSceneInPlayMode(
            Test.TestUtils.testSceneDirPath + sceneName,
            new LoadSceneParameters(LoadSceneMode.Single)
        );


        yield return new EnterPlayMode();
    }
}
