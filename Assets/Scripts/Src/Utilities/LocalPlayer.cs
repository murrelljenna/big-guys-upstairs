using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }
}
