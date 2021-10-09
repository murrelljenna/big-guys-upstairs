using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.economy;

using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

using static game.assets.utilities.GameUtils;
using game.assets;
using game.assets.utilities.resources;

namespace Tests
{
    public class TestWorkerGathering : MonoBehaviour
    {
        private string sceneName = "TestWorkerGathering.unity";

        private Worker worker;
        private Resource resource;
        private Depositor town;

        private bool callBackInvoked = false;
        public void callbackInvoked()
        {
            callBackInvoked = true;
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                Test.TestUtils.testSceneDirPath + sceneName,
                new LoadSceneParameters(LoadSceneMode.Single)
            );

            yield return null;

            callBackInvoked = false;

            worker = GameObject.Find("Worker").GetComponent<Worker>();
            resource = GameObject.Find("Resource").GetComponent<Resource>();
            town = GameObject.Find("Town").GetComponent<Depositor>();

            LocalGameManager gameManager = GameObject.Find(MagicWords.GameObjectNames.GameManager).GetComponent<LocalGameManager>();
            gameManager.players = new game.assets.player.Player[2] {
                    new game.assets.player.Player(),
                    new game.assets.player.Player()
                };
            Debug.Log(town.IsMine());

            town.SetAsMine();
            worker.SetAsMine();
            resource.SetAsMine();

            yield return new EnterPlayMode();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new ExitPlayMode();
        }

        [UnityTest, Order(1)]
        public IEnumerator testWorkerCollectsResource()
        {
            resource.addWorker(worker);
            yield return new WaitForSeconds(3);
            Assert.True(worker.inventory.anyValOver(0));
        }

        [UnityTest, Order(2)]
        public IEnumerator testWorkerReturnsResource()
        {
            worker.inventory = new ResourceSet(wood: 6);
            resource.addWorker(worker);
            yield return new WaitForSeconds(2);
            Assert.False(worker.inventory.anyValOver(0));
        }
    }
}
