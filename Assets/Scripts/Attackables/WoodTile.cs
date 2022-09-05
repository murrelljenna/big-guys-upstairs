using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources; 

public class WoodTile : ResourceTile
{
    protected override void Start() {
    	base.Start();
        resourceSetYield = new ResourceSet(1);
    }
}
