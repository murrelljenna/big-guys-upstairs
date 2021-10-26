﻿using game.assets;
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
            GameObject go = InstantiatorFactory.getInstantiator(false).InstantiateAsPlayer(leaveBehind, transform.position, transform.rotation, player);
            go.GetComponent<Destroy>().destroyAfterAMinute(); // Clean yoself up
        }
        Destroy(gameObject);
    }

    public void destroyAfterAMinute()
    {
        Invoke("destroy", 60f);
    }

    public void destroyAfterTenSeconds()
    {
        Invoke("destroy", 10f);
    }
}
