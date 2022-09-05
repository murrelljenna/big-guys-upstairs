using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Militia : Unit
{

    // Start is called before the first frame update
    void Start()
    {
    	this.movable = true;
    	this.responseRange = 2f;
        this.woodCost = 1;
        this.foodCost = 5;

        this.atk = 1;
        this.hp = 5;
        this.lastHP = this.hp;
        this.rng = 0.3f;

        base.Start();
    }

    void Update() {
        base.Update();
    }
}
