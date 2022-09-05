using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Building : Attackable
{
    protected bool canBeRecycled = true;

    protected override void Start()
    {
        info = this.gameObject.transform.Find("Info").gameObject;
        info.SetActive(false);

        base.Start(); 
    } 

    public override void Update() {
        if (playerCamera != null && info.active == true) {
            info.transform.LookAt(playerCamera.transform);   
        } else {
            playerCamera = getLocalCamera();
        }

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
