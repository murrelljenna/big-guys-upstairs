using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources; 

public class StoneTile : ResourceTile
{
    protected override void Start() {
        base.Start();
        resourceSetYield = new ResourceSet(0, 0, 0, 1);
    }
}
