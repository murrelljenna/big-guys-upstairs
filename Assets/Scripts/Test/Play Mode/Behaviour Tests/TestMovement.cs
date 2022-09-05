using game.assets.ai;
using System.Collections;
using UnityEngine;

using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;


namespace Tests
{
    public class TestMovement
    {
        private string sceneName = "TestMovement.unity";

        private Movement mover;
        private Vector3 destination;

        private bool halted = false;
        private bool reachedDestination = false;

        private void onHaltedCallback()
        {
            halted = true;
        }

        private void onReachDestinationCallback()
        {
            reachedDestination = true;
        }

        private void resetMoverPosition(Vector3 position)
        {
            mover.transform.position = position;
        }

        public bool equalish(Vector3 me, Vector3 other, float percentage = 0.01f)
        {
            var dx = me.x - other.x;
            if (Mathf.Abs(dx) > me.x * percentage)
                return false;

            var dy = me.y - other.y;
            if (Mathf.Abs(dy) > me.y * percentage)
                return false;

            var dz = me.z - other.z;

            return Mathf.Abs(dz) >= me.z * percentage;
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                Test.TestUtils.testSceneDirPath + sceneName,
                new LoadSceneParameters(LoadSceneMode.Single)
            );

            yield return null;

            mover = GameObject.Find("Movement").GetComponent<Movement>();
            destination = GameObject.Find("Destination").transform.position;

            halted = false;
            reachedDestination = false;

            yield return new EnterPlayMode();
        }

        [UnityTest, Order(1)]
        public IEnumerator testGoToDestination()
        {
            mover.goTo(destination);
            yield return new WaitForSeconds(2);
            Assert.True(equalish(mover.transform.position, destination));
        }

        [UnityTest, Order(2)]
        public IEnumerator testStopMovement()
        {
            mover.goTo(destination);
            Vector3 startingPosition = mover.transform.position;
            mover.stop();
            yield return new WaitForSeconds(2);

            Assert.True(equalish(startingPosition, mover.transform.position));
        }

        [UnityTest, Order(3)]
        public IEnumerator testStopInvokesHalt()
        {
            mover.halted.AddListener(onHaltedCallback);
            mover.goTo(destination);
            mover.stop();
            yield return new WaitForSeconds(2);

            Assert.True(halted);
        }

        [UnityTest, Order(4)]
        public IEnumerator testInvokeEventOnReachDestination()
        {
            mover.reachedDestination.AddListener(onReachDestinationCallback);
            mover.goTo(destination);
            yield return new WaitForSeconds(2);

            Assert.True(reachedDestination);
        }
    }
}
