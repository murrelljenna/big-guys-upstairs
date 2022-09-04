using UnityEngine;
using game.assets.player;

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
                playerCamera = LocalPlayer.getPlayerCamera();
            }
        }
    }
}