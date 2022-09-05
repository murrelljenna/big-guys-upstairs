using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources; 

public class FoodTile : ResourceTile
{
    // Start is called before the first frame update
    protected override void Start()
    {
    	  base.Start();
        resourceSetYield = new ResourceSet(0, 1);
    }
}
