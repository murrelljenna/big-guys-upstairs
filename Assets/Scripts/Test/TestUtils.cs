using game.assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public static class TestUtils
    {
        public static string testSceneDirPath = "Assets/Scenes/TestScenes/";

        public static GameObject FakeClientSingleton()
        {
            GameObject cs = new GameObject("ClientSingleton");
            cs.AddComponent<PlayerDepositor>();
            return cs;
        }

        public static void ClearGameObjects()
        {
            foreach (GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                GameObject.DestroyImmediate(obj);
            }
        }
    }
}
