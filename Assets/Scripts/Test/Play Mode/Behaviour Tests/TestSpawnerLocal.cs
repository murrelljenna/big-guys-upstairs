using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

using game.assets;
using game.assets.spawners;
using game.assets.utilities.resources;

namespace Tests
{
    public class TestSpawnerLocal
    {
        Spawner spawner;
        GameObject prefab;
        PlayerDepositor playerWallet;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                Test.TestUtils.testSceneDirPath + "TestSpawnerLocal.unity", 
                new LoadSceneParameters(LoadSceneMode.Single)
            );

            yield return null;

            spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
            prefab = GameObject.Find("SpawnerCopiesThis");
            playerWallet = GameObject.Find("ClientSingleton").GetComponent<PlayerDepositor>();

            yield return new EnterPlayMode();
        }

        [UnityTest]
        public IEnumerator TestSpawnerSpawnNotNull()
        {
            GameObject copy = spawner.Spawn();
            yield return null;
            Assert.NotNull(copy);
        }

        [UnityTest]
        public IEnumerator TestSpawnerSpawnsJustOne()
        {
            spawner.Spawn();
            yield return null;
            GameObject[] sceneObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            int cloneCounter = 0;

            foreach (GameObject gameObject in sceneObjects)
            {
                if (gameObject.name == (prefab.name + "(Clone)"))
                    cloneCounter++;
            }

            Assert.AreEqual(cloneCounter, 1);
        }

        [UnityTest]
        public IEnumerator TestSpawnerCantAffordFails()
        {
            spawner.price.wood = 10; // Too much to afford.
            playerWallet.store.wood = 0;

            GameObject copy = spawner.Spawn();
            yield return null;
            Assert.Null(copy);
        }

        [UnityTest]
        public IEnumerator TestSpawnerDeductsPrice()
        {
            spawner.price.wood = 10;
            playerWallet.store.wood = 20;

            GameObject copy = spawner.Spawn();
            yield return null;
            Assert.AreEqual(playerWallet.store.wood, 10);
        }

        [UnityTest]
        public IEnumerator TestSpawnerInvokeSpawn()
        {
            spawner.InvokeSpawn();
            yield return null;
            GameObject[] sceneObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            int cloneCounter = 0;

            foreach (GameObject gameObject in sceneObjects)
            {
                if (gameObject.name == (prefab.name + "(Clone)"))
                    cloneCounter++;
            }

            Assert.AreEqual(cloneCounter, 1);
        }
    }
}
