using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Militia : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        this.atk = 1;
        this.hp = 5;
        this.rng = 0.02f;
    }
}
