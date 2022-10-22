using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using game.assets.ai;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;
using game.assets.interaction;
using game.assets.utilities;
using game.assets;

namespace Tests {
    public static class Helpers
    {
        public static void lookAt(this Camera camera, Vector3 point)
        {
            camera.transform.LookAt(point);
        }

        public static MouseEvents mouse(this CommandTool commandTool)
        {
            return commandTool.GetComponent<MouseEvents>();
        }

        public static IEnumerator selectAllAttackers(this CommandTool commandTool, List<Attack> attackers)
        {
            for (int i = 0; i < attackers.Count; i++)
            {
                commandTool.cam.lookAt(attackers[i].transform.position);
                yield return null;
                yield return null;
                commandTool.mouse().leftClick.Invoke();
                yield return null;
                yield return null;
            }
        }
        public static IEnumerator selectAttackersAroundUnit(this CommandTool commandTool, Attack attacker)
        {
            commandTool.cam.lookAt(attacker.transform.position);
            yield return null;
            yield return null;
            yield return new WaitForSeconds(2);
            commandTool.mouse().leftDoubleClick.Invoke();
            yield return new WaitForSeconds(2);
            yield return null;
            yield return null;
        }

    }

    public class TestCommandTool
    {
        private string sceneName = "TestCommandTool.unity";
        private bool callBackTriggered = false;

        private void callback(float hp, float maxhp)
        {
            callBackTriggered = true;
        }

        private void setOwnershipToPlayer(List<Attack> attackers)
        {
            attackers.ForEach(attack => attack.SetAsMine());
        }

        private List<Attack> getAttackers()
        {
            List<Attack> ret = new List<Attack>();
            GameObject[] units = GameObject.FindGameObjectsWithTag("unit");
            for (int i = 0; i < units.Length; i++)
            {
                Attack attack = units[i].GetComponent<Attack>();
                if (attack != null)
                {
                    ret.Add(attack);
                }
            }

            return ret;
        }

        private List<Movement> getMovers()
        {
            List<Movement> ret = new List<Movement>();
            GameObject[] units = GameObject.FindGameObjectsWithTag("unit");
            for (int i = 0; i < units.Length; i++)
            {
                Movement movement = units[i].GetComponent<Movement>();
                if (movement != null)
                {
                    ret.Add(movement);
                }
            }

            return ret;
        }

        Health attackee;
        CommandTool commandTool;
        List<Attack> attackers;
        private Vector3 destination;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                Test.TestUtils.testSceneDirPath + sceneName,
                new LoadSceneParameters(LoadSceneMode.Single)
            );
            GameObject gameObject = new GameObject("CommandTool");

            yield return null;

            attackers = getAttackers();
            commandTool = GameObject.Find("CommandTool").GetComponent<CommandTool>();
            attackee = GameObject.Find("Attackee").GetComponent<Health>();
            destination = GameObject.Find("Destination").transform.position;
            setOwnershipToPlayer(attackers);
            attackee.SetAsPlayer(LocalGameManager.Get().barbarianPlayer);

            callBackTriggered = false;

            yield return new EnterPlayMode();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new ExitPlayMode();
        }

        [UnityTest, Order(1)]
        public IEnumerator TestCommandToolSelectAndAttack()
        {
            attackee.onLowerHP.AddListener(callback);
            yield return commandTool.StartCoroutine(commandTool.selectAllAttackers(attackers));

            commandTool.cam.lookAt(attackee.transform.position);
            commandTool.mouse().rightClick.Invoke();
            yield return new WaitForSeconds(3);
            Assert.True(callBackTriggered);
        }

        [UnityTest, Order(2)]
        public IEnumerator TestCommandToolSelectAndMove()
        {
            attackee.onLowerHP.AddListener(callback);
            yield return commandTool.StartCoroutine(commandTool.selectAllAttackers(attackers));

            commandTool.cam.lookAt(destination);
            commandTool.mouse().rightClick.Invoke();
            yield return new WaitForSeconds(2);
            Attack[] inRange = GameUtils.findGameObjectsInRange(destination, 1f).GetComponents<Attack>();
            Assert.True(inRange.Length == 3);
        }

        [UnityTest, Order(3)]
        public IEnumerator TestCommandToolClearOnDisable()
        {
            attackee.onLowerHP.AddListener(callback);
            yield return commandTool.StartCoroutine(commandTool.selectAllAttackers(attackers));

            commandTool.gameObject.SetActive(false);
            commandTool.gameObject.SetActive(true);
            yield return null;

            commandTool.cam.lookAt(destination);
            commandTool.mouse().rightClick.Invoke();

            yield return new WaitForSeconds(2);

            Attack[] inRange = GameUtils.findGameObjectsInRange(destination, 1f).GetComponents<Attack>();
            Assert.True(inRange.Length == 0);
        }

        [UnityTest, Order(5)]
        public IEnumerator TestCommandToolSelectInRadius()
        {
            attackee.onLowerHP.AddListener(callback);
            yield return commandTool.StartCoroutine(commandTool.selectAttackersAroundUnit(attackers[0]));

            commandTool.cam.lookAt(destination);
            commandTool.mouse().rightClick.Invoke();
            yield return new WaitForSeconds(2);
            Attack[] inRange = GameUtils.findGameObjectsInRange(destination, 1f).GetComponents<Attack>();
            Assert.True(inRange.Length == 3);
        }
    }
}
