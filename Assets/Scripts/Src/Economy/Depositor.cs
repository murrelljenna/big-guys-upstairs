using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources; 
using game.assets.utilities;
using game.assets.player;

public class Depositor : MonoBehaviour {
    [Tooltip("When checked, all deposits to this depositor are immediately forwarded to the player")]
    public bool isFinal;

    [Tooltip("Starting resources")]
    public ResourceSet store = new ResourceSet();
    private Depositor upstream;

    public void deposit(ResourceSet yield) {
        if (isFinal) {
            LocalPlayer.getPlayerDepositor().giveResources(yield);
        } else {
            store = store + yield;
        }
    }
}
