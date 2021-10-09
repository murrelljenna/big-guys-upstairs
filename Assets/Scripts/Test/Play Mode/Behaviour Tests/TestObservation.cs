using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

using game.assets.interaction;

namespace Tests
{
    public class TestObservation
    {
        private Scene scene;

        private bool onObserveFired = false;
        private bool onBreakObserveFired = false;

        private ObservationAgent obsAgent;
        private ObservationEvents obsEvents;

        public void onObserveTestCallback() {
            onObserveFired = true;
        }

        public void onBreakObserveTestCallback()
        {
            onBreakObserveFired = true;
        }

        private void agentLookAway()
        {
            Vector3 tmp = obsAgent.gameObject.transform.eulerAngles;
            tmp.y = 180;
            obsAgent.gameObject.transform.eulerAngles = tmp;
        }

        private void agentLook()
        {

            Vector3 tmp = obsAgent.gameObject.transform.eulerAngles;
            tmp.y = 0;
            obsAgent.gameObject.transform.eulerAngles = tmp;
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            onBreakObserveFired = false;
            onObserveFired = false;

            scene = EditorSceneManager.LoadSceneInPlayMode(
                Test.TestUtils.testSceneDirPath + "TestObservation.unity",
                new LoadSceneParameters(LoadSceneMode.Single)
            );

            yield return null;

            obsAgent = GameObject.Find("Observer").GetComponent<ObservationAgent>();
            obsEvents = GameObject.Find("Observed").GetComponent<ObservationEvents>();

            yield return new EnterPlayMode();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new ExitPlayMode();
        }

        /*
         * Test the trigger discipline of our agent events. 
         * The scene spawns the agent looking away. The agent should not be firing any events prior to
         * looking at our ObservationEvents gameobject.
         */
        [UnityTest, Order(1)]
        public IEnumerator testAgentObsCallbackTrigDiscipline()
        {
            obsAgent.onObserve.AddListener(onObserveTestCallback);
            yield return null;
            Assert.False(onObserveFired);
        }

        /*
         * Ibid.
         */

        [UnityTest, Order(2)]
        public IEnumerator testAgentBrkObsCallbackTrigDiscipline()
        {
            obsAgent.onObserve.AddListener(onBreakObserveTestCallback);
            yield return null;
            Assert.False(onBreakObserveFired);
        }

        /*
         * Turn the agent to look at our ObservationEvent gameobject. UnityEvent onObserve event should fire.
         */

        // Test observer onObserve event
        [UnityTest, Order(3)]
        public IEnumerator testAgentObserveCallback()
        {
            obsAgent.onObserve.AddListener(onObserveTestCallback);
            agentLook();
            yield return null;
            Assert.True(onObserveFired);
        }

        /*
         * Turn the agent to look at our ObservationEvent gameobject, then look away. 
         * UnityEvent onBreakObserve event should fire.
         */

        [UnityTest, Order(4)]
        public IEnumerator testAgentBreakObserveCallback()
        {
            obsAgent.onBreakObserve.AddListener(onBreakObserveTestCallback);
            agentLook();
            yield return null;
            agentLookAway();
            yield return null;
            Assert.True(onBreakObserveFired);
        }

        /*
         * Turn the agent to look at our ObservationEvent gameobject.
         * UnityEvent onBreakObserve should NOT fire.
         */

        [UnityTest, Order(5)]
        public IEnumerator testAgentBreakObserveDiscipline()
        {
            obsAgent.onObserve.AddListener(onObserveTestCallback);
            obsAgent.onBreakObserve.AddListener(onBreakObserveTestCallback);

            agentLook();
            yield return null;
            Assert.False(onBreakObserveFired);
        }

        /*
         * Turn the agent to look at our ObservationEvent gameobject.
         * UnityEvent onBreakObserve should NOT fire.
         */

        [UnityTest, Order(6)]
        public IEnumerator testAgentObserveDiscipline()
        {
            obsAgent.onObserve.AddListener(onObserveTestCallback);
            obsAgent.onBreakObserve.AddListener(onBreakObserveTestCallback);

            agentLook();
            yield return null;

            /*
             * Set our indicator for onObserve event to false. Trigger our onBreakObserve event.
             * onObserve event should not fire (thus it should not set onObserveFired back to true)
             */

            onObserveFired = false;
            agentLookAway();
            yield return null;
            Assert.False(onObserveFired);
        }

        /*
         * Test our ObservationEvents script - the observed gameObject.
         */

        /*
         * Test the trigger discipline of our agent events. 
         * The scene spawns the agent looking away. The agent should not be firing any events prior to
         * looking at our ObservationEvents gameobject.
         */
        [UnityTest, Order(1)]
        public IEnumerator testEventsObsCallbackTrigDiscipline()
        {
            obsEvents.onObserve.AddListener(onObserveTestCallback);
            yield return null;
            Assert.False(onObserveFired);
        }

        /*
         * Ibid.
         */

        [UnityTest, Order(2)]
        public IEnumerator testEventsBrkObsCallbackTrigDiscipline()
        {
            obsEvents.onObserve.AddListener(onBreakObserveTestCallback);
            yield return null;
            Assert.False(onBreakObserveFired);
        }

        /*
         * Turn the agent to look at our ObservationEvent gameobject. UnityEvent onObserve event should fire.
         */

        // Test observer onObserve event
        [UnityTest, Order(3)]
        public IEnumerator testEventsObserveCallback()
        {
            obsEvents.onObserve.AddListener(onObserveTestCallback);
            agentLook();
            yield return null;
            Assert.True(onObserveFired);
        }

        /*
         * Turn the agent to look at our ObservationEvent gameobject, then look away. 
         * UnityEvent onBreakObserve event should fire.
         */

        [UnityTest, Order(4)]
        public IEnumerator testEventsBreakObserveCallback()
        {
            obsEvents.onBreakObserve.AddListener(onBreakObserveTestCallback);
            agentLook();
            yield return null;
            agentLookAway();
            yield return null;
            Assert.True(onBreakObserveFired);
        }

        /*
         * Turn the agent to look at our ObservationEvent gameobject.
         * UnityEvent onBreakObserve should NOT fire.
         */

        [UnityTest, Order(5)]
        public IEnumerator testEventsBreakObserveDiscipline()
        {
            obsEvents.onObserve.AddListener(onObserveTestCallback);
            obsEvents.onBreakObserve.AddListener(onBreakObserveTestCallback);

            agentLook();
            yield return null;
            Assert.False(onBreakObserveFired);
        }

        /*
         * Turn the agent to look at our ObservationEvent gameobject.
         * UnityEvent onBreakObserve should NOT fire.
         */

        [UnityTest, Order(6)]
        public IEnumerator testEventsObserveDiscipline()
        {
            obsEvents.onObserve.AddListener(onObserveTestCallback);
            obsEvents.onBreakObserve.AddListener(onBreakObserveTestCallback);

            agentLook();
            yield return null;

            /*
             * Set our indicator for onObserve event to false. Trigger our onBreakObserve event.
             * onObserve event should not fire (thus it should not set onObserveFired back to true)
             */

            onObserveFired = false;
            agentLookAway();
            yield return null;
            Assert.False(onObserveFired);
        }
    }
}