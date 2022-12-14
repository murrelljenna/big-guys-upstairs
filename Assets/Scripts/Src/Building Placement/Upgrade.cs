using game.assets;
using game.assets.player;
using game.assets.utilities.resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{

    [Tooltip("Building to upgrade to")]
    public GameObject upgradeTo;
    [Tooltip("Cost of upgrading")]
    public ResourceSet price;
    public Player player;

    public void upgrade()
    {
        if (player.canAfford(price))
        {
            player.takeResources(price);

            spawnUpgradeTo();
            Destroy(gameObject);
        }
    }

    private void spawnUpgradeTo()
    {
        if (upgradeTo != null)
        {
            game.assets.player.Player player = GetComponent<Ownership>().owner;
            GameObject go = InstantiatorFactory.getInstantiator(false).InstantiateAsPlayer(upgradeTo, transform.position, transform.rotation, player);
        }
    }
}
