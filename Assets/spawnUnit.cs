﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnUnit : MonoBehaviour
{
	res wallet;
	public Camera cam;
	private int layerMask;
    // Start is called before the first frame update
    void Start()
    {
        wallet = GameObject.Find("Player").GetComponent<res>();
        layerMask = 1 << 11;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
