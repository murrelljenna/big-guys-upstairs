using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using game.assets;
using static game.assets.utilities.GameUtils;

namespace Tests
{
    public class TestGameManager
    {
        private string sceneName = "TestGameManager.unity";
        private LocalGameManager gameManager;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                Test.TestUtils.testSceneDirPath + sceneName,
                new LoadSceneParameters(LoadSceneMode.Single)
            );

            yield return new WaitForSeconds(1);

            gameManager = GameObject.Find("GameManager").GetComponent<LocalGameManager>();
            yield return new EnterPlayMode();
        }

        [UnityTest, Order(1)]
        public IEnumerator testGameManagerPersists()
        {
            Vector3[] spawnPoints = new Vector3[] { new Vector3(0, 0, 0) };
            gameManager.Initialize(Test.TestUtils.testSceneDirPath + sceneName, spawnPoints);
            yield return new WaitForSeconds(1);

            Assert.True(GameObject.Find(MagicWords.GameObjectNames.Player) != null);
        }

        [UnityTest, Order(2)]
        public IEnumerator testGameManagerSpawnsPlayer()
        {
            Vector3[] spawnPoints = new Vector3[] { new Vector3(0, 0, 0) };
            gameManager.Initialize(Test.TestUtils.testSceneDirPath + sceneName, spawnPoints);
            yield return new WaitForSeconds(1);

            Assert.True(GameObject.Find(MagicWords.GameObjectNames.Player) != null);
        }

        [UnityTest, Order(3)]
        public IEnumerator testGameManagerInstantiatesClientSingleton()
        {
            Vector3[] spawnPoints = new Vector3[] { new Vector3(0, 0, 0) };
            gameManager.Initialize(Test.TestUtils.testSceneDirPath + sceneName, spawnPoints);
            yield return new WaitForSeconds(1);

            Assert.True(GameObject.Find(MagicWords.GameObjectNames.ClientSingleton) != null);
        }

        [UnityTest, Order(4)]
        public IEnumerator testGameManagerSpawnsPlayerStartingCity()
        {
            Vector3[] spawnPoints = new Vector3[] { new Vector3(0, 0, 0) };
            gameManager.Initialize(Test.TestUtils.testSceneDirPath + sceneName, spawnPoints);
            yield return new WaitForSeconds(1);

            Assert.True(GameObject.Find(MagicWords.GameObjectNames.StartingCity) != null);
        }
    }
}
