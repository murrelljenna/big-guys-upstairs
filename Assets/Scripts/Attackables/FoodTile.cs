using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTile : ResourceTile
{
    // Start is called before the first frame update
    protected override void Start()
    {
    	base.Start();
		resType = "food";
    }
}
