using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.AI;

using UnityEngine.UI;

public class gateController : MonoBehaviour
{
	private int noOfUnitsNearby = 0;
	private int noOfEnemiesNearby = 0;
	private Animator animator;
	private NavMeshObstacle obstacle;
	public bool locked;
	private AudioSource[] sources = new AudioSource[2];

	private void Start() {
		sources = this.GetComponents<AudioSource>();
		animator = this.transform.parent.Find("Model").GetComponent<Animator>();
		obstacle = this.transform.parent.gameObject.GetComponent<NavMeshObstacle>();
		obstacle.enabled = false;
	}

	public void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "unit" && other.gameObject.GetComponent<Attackable>().prefabName != "GuardTower" && this.transform.parent.gameObject.GetComponent<ownership>().owner == other.gameObject.gameObject.GetComponent<ownership>().owner) {
			noOfUnitsNearby--;

			if (!locked && noOfUnitsNearby == 0) {
		        AudioSource.PlayClipAtPoint(sources[1].clip, this.transform.position);
				animator.SetTrigger("close");
			}
		} else if (other.gameObject.tag == "unit" && this.transform.parent.gameObject.GetComponent<ownership>().owner != other.gameObject.gameObject.GetComponent<ownership>().owner) {
			noOfEnemiesNearby--;
		}

		if (!locked) {
			if (noOfUnitsNearby > 0 || noOfEnemiesNearby == 0) {
				obstacle.enabled = false;
			} else {
				obstacle.enabled = true;
			}
		}
	}

	public void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "unit" && other.gameObject.GetComponent<Attackable>().prefabName != "GuardTower" && this.transform.parent.gameObject.GetComponent<ownership>().owner == other.gameObject.gameObject.GetComponent<ownership>().owner) {
			noOfUnitsNearby++;

			if (!locked && noOfUnitsNearby == 1) {
				AudioSource.PlayClipAtPoint(sources[0].clip, this.transform.position);
				animator.SetTrigger("open");
			}
		} else if (other.gameObject.tag == "unit" && this.transform.parent.gameObject.GetComponent<ownership>().owner != other.gameObject.gameObject.GetComponent<ownership>().owner) {
			noOfEnemiesNearby++;
		}

		if (!locked && animator != null) {
			if (noOfUnitsNearby == 0 && noOfEnemiesNearby > 0) {
				obstacle.enabled = true;
			} else {
				obstacle.enabled = false;
			}
		}
	}

	public void lockGate() {
		animator.SetBool("locked", true);

		obstacle.enabled = true;
		obstacle.carving = true;

		this.locked = true;
		this.transform.parent.Find("Info").Find("Lock selector").Find("Text").gameObject.GetComponent<Text>().text = "Locked";
	}

	public void unlockGate() {
		animator.SetBool("locked", false);

		if (noOfUnitsNearby == 0 && noOfEnemiesNearby > 0) {
			obstacle.enabled = true;
		} else {
			obstacle.enabled = false;
		}

		obstacle.carving = false;
		this.locked = false;

		this.transform.parent.Find("Info").Find("Lock selector").Find("Text").gameObject.GetComponent<Text>().text = "Unlocked";
	}
}