using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources; 
using game.assets.utilities;
using game.assets.player;

[RequireComponent(typeof(Ownership))]
public class Depositor : MonoBehaviour {
    [Tooltip("When checked, all deposits to this depositor are immediately forwarded to the player")]
    public bool isFinal;

    [Tooltip("Starting resources")]
    public ResourceSet store = new ResourceSet();
    private Depositor upstream;
    private Player player;

    public void Start()
    {
        player = GetComponent<Ownership>().owner;
    }

    public void deposit(ResourceSet yield) {
        if (isFinal) {
            player.giveResources(yield);
        } else {
            store = store + yield;
        }
    }
}
