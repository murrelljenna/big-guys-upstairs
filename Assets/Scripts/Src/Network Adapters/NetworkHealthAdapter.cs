using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using game.assets.ai;

namespace game.assets.network.adapters {
    /* Under construction. Model for what networking might look like after refactoring */
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(PhotonView))]
    public class NetworkHealthAdapter : MonoBehaviour
    {
        public Health health;
        public PhotonView photonView;

        private void Start()
        {
            health = GetComponent<Health>();
            photonView = GetComponent<PhotonView>();
        }

        [PunRPC]
        public void lowerHP(int amt) {
            health.lowerHP(amt);
        }

        public void NetworkDestroy()
        {
            if (photonView.IsMine) {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
}
