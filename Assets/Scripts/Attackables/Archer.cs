using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Archer : Unit
{
	public GameObject missile;

    // Start is called before the first frame update
    void Start()
    {
    	this.movable = true;
    	this.responseRange = 3f;
        this.woodCost = 2;
        this.foodCost = 10;

        this.atk = 2;
        this.hp = 8;
        this.lastHP = this.hp;
        this.rng = 3f;
        this.attackRate = 1.4f;

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

    public override void attack() {
    	Invoke("shootArrow", 0.6f);

        AudioSource[] sources = this.gameObject.transform.Find("AttackSounds1").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play(0);

		sources = this.gameObject.transform.Find("AttackSounds2").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play(1);

    	base.attack();
    }

    private void shootArrow() {
    	if (attackee != null) {
    		GameObject bow = this.transform.Find("Model").Find("Bip001").Find("Bip001 Pelvis").Find("Bip001 Spine").Find("Bip001 L Clavicle").Find("Bip001 L UpperArm").Find("Bip001 L Forearm").Find("Bip001 L Hand").Find("L_hand_container").Find("w_recurve_bow").gameObject;
			GameObject arrow = Instantiate(missile, bow.transform.position, Quaternion.LookRotation((attackee.gameObject.transform.position - bow.transform.position).normalized));
			arrow.transform.Rotate(-90, 0, 0); // Can't figure out how to get this fucking thing to face the right way.

			arrow.GetComponent<ownership>().localCapture(GetComponent<ownership>().getPlayer());

			arrow.GetComponent<Rigidbody>().AddForce((attackee.gameObject.transform.position - bow.transform.position).normalized * 500);
		}
    }
}
