﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    // Start is called before the first frame update
    void Start()
    {
    	this.movable = true;
    	this.responseRange = 2f;
        this.woodCost = 2;
        this.foodCost = 10;

        this.atk = 2;
        this.hp = 10;
        this.lastHP = this.hp;
        this.rng = 3f;

        base.Start();
    }

    void Update() {
        base.Update();
    }

    public override void onSelect() {
        AudioSource[] sources = this.transform.Find("SelectionSounds").GetComponents<AudioSource>();
        AudioSource source = sources[UnityEngine.Random.Range(0, sources.Length)];
        AudioSource.PlayClipAtPoint(source.clip, this.transform.position);

        base.onSelect();
    }

    public override void move(Vector3 destination) {
        StartCoroutine(delayMovement(destination));
    }

    private IEnumerator delayMovement(Vector3 destination) {
        yield return new WaitForSeconds(Random.Range(0.05f, 0.4f));
        base.move(destination);
    }
}
