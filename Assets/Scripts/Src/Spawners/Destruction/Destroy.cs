using game.assets;
using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public GameObject leaveBehind;
    public void destroy()
    {
        if (leaveBehind != null)
        {
            game.assets.player.Player player = GetComponent<Ownership>().owner;
            InstantiatorFactory.getInstantiator(false).InstantiateAsPlayer(leaveBehind, transform.position, transform.rotation, player);
        }
        Destroy(gameObject);
    }
}
