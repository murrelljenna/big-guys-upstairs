using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets
{
    public interface IInstantiator
    {
        GameObject Instantiate(GameObject gameObject, Vector3 location, Quaternion rotation);
        GameObject InstantiateAsMine(GameObject gameObject, Vector3 location, Quaternion rotation);
        GameObject InstantiateAsPlayer(GameObject gameObject, Vector3 location, Quaternion rotation, game.assets.player.Player player);
    }
}
