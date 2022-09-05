using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightInfantry : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        this.prefabName = "Light Infantry";
    	this.movable = true;
    	this.responseRange = 2f;
        this.woodCost = 2;
        this.foodCost = 10;

        this.atk = 3;
        this.hp = 12;
        this.lastHP = this.hp;
        this.rng = 0.5f;
        this.attackRate = 1.2f;

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
}
