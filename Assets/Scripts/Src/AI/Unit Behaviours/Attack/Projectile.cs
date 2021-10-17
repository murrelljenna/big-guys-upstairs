using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using game.assets.ai;
using game.assets.player;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private int dmg;
    [Tooltip("Invoked when projectile hits an enemy")]
    public UnityEvent onCollision;

    public void OnTriggerEnter(Collider collision)
    {
        Health collidingEnemy = collision.gameObject.GetComponent<Health>();
        if (collidingEnemy != null && collidingEnemy.IsEnemyOf(this))
        {
            onCollision.Invoke();

            collidingEnemy.lowerHP(dmg);

            Destroy(gameObject, 0.2f);
        }
    }

    public void setDmg(int dmg)
    {
        this.dmg = dmg;
    }
}