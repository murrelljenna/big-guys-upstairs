using UnityEngine;

using Photon.Pun;

namespace game.assets
{
    /*
     * Instantiate on the Photon network.
     */

    public class NetworkedInstantiator : IInstantiator {
        public GameObject Instantiate(GameObject gameObject, Vector3 location, Quaternion rotation)
        {
            /* This will throw an error if the gameObject provided is not a prefab in the Resources folder */
            return PhotonNetwork.Instantiate(gameObject.name, location, rotation, 0);
        }

        GameObject IInstantiator.InstantiateAsMine(GameObject gameObject, Vector3 location, Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }

        GameObject IInstantiator.InstantiateAsPlayer(GameObject gameObject, Vector3 location, Quaternion rotation, player.Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}
