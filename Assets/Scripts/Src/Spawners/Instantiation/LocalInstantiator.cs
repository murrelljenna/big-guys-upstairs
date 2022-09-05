using UnityEngine;

namespace game.assets
{
    public class LocalInstantiator : IInstantiator
    {
        public GameObject Instantiate(GameObject gameObject, Vector3 location, Quaternion rotation)
        {
            return Object.Instantiate(gameObject, location, rotation);
        }
        public GameObject InstantiateAsPlayer(GameObject gameObject, Vector3 location, Quaternion rotation, player.Player player)
        {
            GameObject gameObj = Object.Instantiate(gameObject, location, rotation);
            gameObj.SetAsPlayer(player);
            return gameObj;
        }

        public GameObject InstantiateAsMine(GameObject gameObject, Vector3 location, Quaternion rotation)
        {
            GameObject gameObj = Object.Instantiate(gameObject, location, rotation);
            gameObj.SetAsMine();
            return gameObj;
        }
    }
}
