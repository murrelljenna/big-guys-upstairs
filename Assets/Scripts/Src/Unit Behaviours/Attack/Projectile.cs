using UnityEngine;

using game.assets.ai;
using UnityEngine.Events;
using Fusion;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    private int dmg;
    [Tooltip("Invoked when projectile hits an enemy")]
    public UnityEvent onCollision;

    private Attack owner;

    public void OnTriggerEnter(Collider collision)
    {
        if (Object == null || !Object.HasStateAuthority)
        {
            return;
        }

        Health collidingEnemy = collision.gameObject.GetComponent<Health>();
        if (collidingEnemy != null && collidingEnemy.IsEnemyOf(this))
        {
            onCollision.Invoke();

            collidingEnemy.lowerHP(dmg, owner);

            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }

    public void setDmg(int dmg)
    {
        this.dmg = dmg;
    }

    public void setOwner(Attack attacker)
    {
        owner = attacker;
    }
}