using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static game.assets.utilities.GameUtils.MagicWords;

namespace game.assets.player {
    public static class LocalPlayer
    {
        public static Player get()
        {
            return GameObject.Find(GameObjectNames.GameManager)?.GetComponent<LocalGameManager>()?.getLocalPlayer();
        }

        public static Camera getPlayerCamera()
        {
            return GameObject.Find(GameObjectNames.FirstPersonCharacter)?.GetComponent<Camera>();
        }

        public static PlayerDepositor getPlayerDepositor()
        {
            return GameObject.Find("ClientSingleton")?.GetComponent<PlayerDepositor>();
        }

        private static CharacterViewHandler cache;

        public static CharacterViewHandler getView()
        {
            if (cache != null)
                return cache;

            List<GameObject> rootObjectsInScene = new List<GameObject>();
            Scene scene = SceneManager.GetActiveScene();
            scene.GetRootGameObjects(rootObjectsInScene);

            for (int i = 0; i < rootObjectsInScene.Count; i++)
            {
                CharacterViewHandler[] allComponents = rootObjectsInScene[i].GetComponentsInChildren<CharacterViewHandler>(true);
                for (int j = 0; j < allComponents.Length; j++)
                {
                    if (allComponents[j].isLocal)
                    {
                        cache = allComponents[j];
                        return allComponents[j];
                    }
                }
            }

            return null;
        }
    }
}
