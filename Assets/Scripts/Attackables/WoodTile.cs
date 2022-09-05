using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodTile : ResourceTile
{
    protected override void Start() {
    	base.Start();
        resType = "wood";
    }
}
