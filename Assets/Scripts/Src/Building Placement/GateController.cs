using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;
using game.assets.ai;
using game.assets.player;
using game.assets.utilities;

namespace game.assets
{
	[RequireComponent(typeof(NavMeshObstacle))]
	[RequireComponent(typeof(Ownership))]
	public class GateController : MonoBehaviour
	{
		private int noOfUnitsNearby = 0;
		private int noOfEnemiesNearby = 0;
		private Animator animator;
		private NavMeshObstacle obstacle;
		public bool locked = false;
		private AudioSource[] sources = new AudioSource[2];

		private player.Player player;

		[Tooltip("Invoked when Gate is opened")]
		public UnityEvent opened;

		[Tooltip("Invoked when Gate is closed")]
		public UnityEvent closed;

		private void Start()
		{
			sources = this.GetComponents<AudioSource>();
			animator = transform.Find("Model").GetComponent<Animator>();
			obstacle = GetComponent<NavMeshObstacle>();
			obstacle.enabled = false;
			player = GetComponent<Ownership>().owner;
		}

		public void OnTriggerExit(Collider other)
		{
			if (other.gameObject.isUnit()
				&& other.gameObject.BelongsTo(player))
			{
				noOfUnitsNearby--;

				if (!locked && noOfUnitsNearby == 0)
				{
					closed.Invoke();
				}
			}
			else if (other.gameObject.isUnit() && other.gameObject.IsEnemyOf(this.gameObject))
			{
				noOfEnemiesNearby--;
			}

			if (!locked)
			{
				if (noOfUnitsNearby > 0 || noOfEnemiesNearby == 0)
				{
					obstacle.enabled = false;
				}
				else
				{
					obstacle.enabled = true;
				}
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.isUnit() && other.gameObject.IsFriendOf(this.gameObject))
			{
				noOfUnitsNearby++;

				if (!locked && noOfUnitsNearby == 1)
				{
					opened.Invoke();
				}
			}
			else if (other.gameObject.isUnit() && other.gameObject.IsEnemyOf(this.gameObject))
			{
				noOfEnemiesNearby++;
			}

			if (!locked && animator != null)
			{
				if (noOfUnitsNearby == 0 && noOfEnemiesNearby > 0)
				{
					obstacle.enabled = true;
				}
				else
				{
					obstacle.enabled = false;
				}
			}
		}

		public void toggleLock()
		{
			if (locked)
			{
				unlockGate();
			}
			else
			{
				lockGate();
			}
		}

		private void lockGate()
		{
			animator.SetBool("locked", true);

			obstacle.enabled = true;
			obstacle.carving = true;

			locked = true;
			transform.parent.Find("Info").Find("Lock selector").Find("Text").gameObject.GetComponent<Text>().text = "Locked";
		}

		private void unlockGate()
		{
			animator.SetBool("locked", false);

			if (noOfUnitsNearby == 0 && noOfEnemiesNearby > 0)
			{
				obstacle.enabled = true;
			}
			else
			{
				obstacle.enabled = false;
			}

			obstacle.carving = false;
			locked = false;

			transform.parent.Find("Info").Find("Lock selector").Find("Text").gameObject.GetComponent<Text>().text = "Unlocked";
		}
	}
}
