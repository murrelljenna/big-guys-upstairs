using UnityEngine;

using game.assets.ai;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private int dmg;
    [Tooltip("Invoked when projectile hits an enemy")]
    public UnityEvent onCollision;

    private Attack owner;

    public void OnTriggerEnter(Collider collision)
    {
        Health collidingEnemy = collision.gameObject.GetComponent<Health>();
        if (collidingEnemy != null && collidingEnemy.IsEnemyOf(this))
        {
            onCollision.Invoke();

            collidingEnemy.lowerHP(dmg, owner);

            Destroy(gameObject, 0.2f);
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