﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using game.assets.utilities.resources;

public class Building : Attackable
{
    protected bool canBeRecycled = true;
    protected int upgradeLevel = 1;
    protected int maxUpgrade = 3;

    protected ResourceSet upgradeCost = new ResourceSet();

    protected int upgradeCostStone = 0;
    protected int upgradeCostGold = 0;
    protected int upgradeCostWood = 0;
    protected int upgradeCostFood = 0;
    protected int upgradeCostIron = 0;

    public bool underConstruction = true;

    GameObject[] buildModels = new GameObject[2];

    protected override void Start()
    {
        base.Start();

        info = this.gameObject.transform.Find("Info").gameObject;
    } 

    public void setToConstruction() {
        getBuildingModels();
        this.hp = maxHP / 10;

        underConstruction = true;
        buildModels[0].SetActive(true);
        buildModels[1].SetActive(false);
        this.transform.Find("Model").gameObject.SetActive(false);
        healthBar.UpdateBar(this.hp, this.maxHP);
    }

    private void getBuildingModels() {
        buildModels[0] = this.transform.Find("buildModel1").gameObject;
        buildModels[1] = this.transform.Find("buildModel2").gameObject;

        buildModels[0].SetActive(true);
        buildModels[1].SetActive(false);
    }

    public virtual void build() {
        photonView.RPC("buildRPC", RpcTarget.AllBuffered);
    }

    
    public void build(int amt) {
        photonView.RPC("buildRPC", RpcTarget.AllBuffered, amt);
    }

    [PunRPC]
    public void buildRPC(int amt)
    {
        this.hp += amt;
        healthBar.UpdateBar(this.hp, this.maxHP);

        if (this.hp > maxHP / 2 && this.hp < maxHP)
        {
            buildModels[0].SetActive(false);
            buildModels[1].SetActive(true);
        }
        else if (this.hp >= maxHP)
        {
            build();
        }
    }

    [PunRPC]
    public void buildRPC()
    {
        buildModels[0].SetActive(false);
        buildModels[1].SetActive(false);
        this.transform.Find("Model").gameObject.SetActive(true);
        underConstruction = false;
        this.hp = this.maxHP;
        healthBar.UpdateBar(this.hp, this.maxHP);
    }

    public override void Update() {
        base.Update();
    }

    public override void takeDamage(int damage) {
        AudioSource[] sources = this.transform.Find("DamageSounds").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

        base.takeDamage(damage);
    }

    protected static Vector2 RandomPointOnUnitCircle(float radius) {
        float angle = Random.Range (0f, Mathf.PI * 2);
        float x = Mathf.Sin (angle) * radius;
        float y = Mathf.Cos (angle) * radius;

        return new Vector2(x, y);
    }

    public override void onCapture() {
        string colorName = GetComponent<ownership>().getPlayer().colorName;
        this.transform.Find("Model").gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));

        Transform model2 = this.transform.Find("Model2");

        if (model2 != null) {
            model2.gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
            model2.gameObject.SetActive(false);
        }

        Transform model3 = this.transform.Find("Model3");

        if (model3 != null) {
            model3.gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
            model3.gameObject.SetActive(false);
        }

        base.onCapture();
    }

    public override void destroyObject() {
        if (this.photonView.IsMine) {
            photonView.RPC("playDestructionEffect", RpcTarget.All);
        }

        if (this.prefabName != "Town") {
            getTownInRange(this.transform.position, 10f, this.owner.owner).removeBuilding();
        }

        if (this.photonView.IsMine && this.prefabName != "Wall") {
            for (int i = 0; i < Random.Range(2, 5); i++) {
                Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);
                Vector3 spawnLocation = new Vector3(randomInCircle.x+this.transform.position.x, this.transform.position.y, randomInCircle.y+this.transform.position.z);

                GameObject militia = PhotonNetwork.Instantiate("Militia", spawnLocation, Quaternion.identity, 0);

                militia.GetComponent<ownership>().capture(this.gameObject.GetComponent<ownership>().getPlayer());
            }
        }

        base.destroyObject();
    }

    public override void interactionOptions(game.assets.Player player) {
        if (canBeRecycled && Input.GetKeyDown(KeyCode.X)) {
            up3.SetActive(false);
            down3.SetActive(true);
            midAnimation = true;
            Invoke("releaseButton3", 0.2f);

            owner.getPlayer().giveResources("wood", this.woodCost/2);
            owner.getPlayer().giveResources("food", this.foodCost/2);
            base.destroyObject();
        }
    }

    private Town getTownInRange(Vector3 location, float range, int ownerID) {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].tag == "town" && hitColliders[i].gameObject.GetComponent<ownership>().owner == ownerID) { // If there is a town in range that belongs to the player.
                return hitColliders[i].gameObject.GetComponent<Town>();
            }
        }

        return null;
    }

    public virtual void upgrade() {
        healthBar.UpdateBar(this.hp, this.maxHP);
        this.transform.Find("UpgradeSounds").GetComponent<AudioSource>().Play(0);
        upgradeLevel++;
    }
    
    [PunRPC]
    protected void playDestructionEffect() {
        AudioSource[] sources = this.transform.Find("DestroySounds").GetComponents<AudioSource>();
        AudioSource source = sources[UnityEngine.Random.Range(0, sources.Length)];
        AudioSource.PlayClipAtPoint(source.clip, this.transform.position);

        GameObject explosion = Instantiate(this.transform.Find("FX_Building_Destroyed_mid").gameObject as GameObject);
        explosion.transform.position = this.transform.position;
        ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
        effect.Play();
        Destroy(effect.gameObject, effect.duration);
    }

    /* Called by buildingPlacement.cs. Returns error code interpreted by buildingPlacement.cs explaining reason why can't build */ 
/*
    public int constructionOptions() {
    Debug.Log("Not put in place yet")
    }
    */
}
