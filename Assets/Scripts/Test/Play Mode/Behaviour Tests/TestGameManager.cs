using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using game.assets;
using static game.assets.utilities.GameUtils;

namespace InsaneTests
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

            gameManager = GameObject.Find(MagicWords.GameObjectNames.GameManager).GetComponent<LocalGameManager>();
            yield return new EnterPlayMode();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                Object.Destroy(g);
            }
            yield return new ExitPlayMode();
        }

        [UnityTest, Order(1)]
        public IEnumerator testGameManagerPersists()
        {
            Vector3[] spawnPoints = new Vector3[] { new Vector3(0, 0, 0) };
            Scene scene = gameManager.Initialize(Test.TestUtils.testSceneDirPath + "Empty.unity", spawnPoints);
            yield return new WaitForSeconds(1);

            Assert.True(GameObject.Find(MagicWords.GameObjectNames.GameManager) != null);
            foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                g.SetActive(false);
            }
        }

        [UnityTest, Order(2)]
        public IEnumerator testGameManagerSpawnsPlayer()
        {
            Vector3[] spawnPoints = new Vector3[] { new Vector3(0, 0, 0) };
            Scene scene = gameManager.Initialize(Test.TestUtils.testSceneDirPath + "Empty.unity", spawnPoints);
            yield return new WaitForSeconds(1);

            Assert.True(GameObject.Find(MagicWords.GameObjectNames.Player) != null);
            foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                g.SetActive(false);
            }
        }

        [UnityTest, Order(3)]
        public IEnumerator testGameManagerInstantiatesClientSingleton()
        {
            Vector3[] spawnPoints = new Vector3[] { new Vector3(0, 0, 0) };
            Scene scene = gameManager.Initialize(Test.TestUtils.testSceneDirPath + "Empty.unity", spawnPoints);
            yield return new WaitForSeconds(1);

            Assert.True(GameObject.Find(MagicWords.GameObjectNames.ClientSingleton) != null);
            foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                g.SetActive(false);
            }
        }

        [UnityTest, Order(4)]
        public IEnumerator testGameManagerSpawnsPlayerStartingCity()
        {
            Vector3[] spawnPoints = new Vector3[] { new Vector3(0, 0, 0) };
            Scene scene = gameManager.Initialize(Test.TestUtils.testSceneDirPath + "Empty.unity", spawnPoints);
            yield return new WaitForSeconds(1);

            Assert.True(GameObject.Find(MagicWords.GameObjectNames.StartingCity) != null);
            foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                g.SetActive(false);
            }
        }
    }
}
