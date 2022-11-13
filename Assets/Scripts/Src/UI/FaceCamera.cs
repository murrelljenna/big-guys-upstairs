using UnityEngine;
using game.assets.player;
using game.assets.utilities;

namespace game.assets.ui
{
    public class FaceCamera : MonoBehaviour
    {

        private Camera playerCamera;
        void FixedUpdate()
        {
            if (playerCamera != null)
            {
                this.transform.LookAt(playerCamera.transform);
            }
            else
            {
                playerCamera = GameUtils.GetLocalPlayerCamera();
            }
        }
    }
}