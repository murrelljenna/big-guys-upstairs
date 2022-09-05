using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources; 
using game.assets.utilities;

public class Depositor : MonoBehaviour {
    private ResourceSet store = new ResourceSet();
    private Depositor upstream;

    public bool isFinal;
    private game.assets.Player player;

    void Start() {
        if (isFinal) {
            player = GetComponent<ownership>().getPlayer();
        }
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void deposit(ResourceSet yield) {
        if (isFinal) {
            player.resources = player.resources + yield;
        } else {
            store = store + yield;
        }
    }
}
