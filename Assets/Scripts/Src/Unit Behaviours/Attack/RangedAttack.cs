using Fusion;
using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.ai
{
    public class RangedAttack : Attack
    {
        [Tooltip("Projectile launched by unit")]
        public NetworkPrefabRef projectile;

        override protected void doDamage()
        {
            faceTarget(attackee.transform.position);
            launchProjectileAt(attackee);
        }

        private void launchProjectileAt(Health attackee)
        {
            NetworkObject projectileInstance = Runner.Spawn(
                projectile, transform.position, 
                Quaternion.LookRotation(
                    (attackee.gameObject.transform.position - transform.position)
                    .normalized
                    ),
                null,
                (runner, o) =>
                {
                    o.GetComponent<Projectile>().setDmg(attackPower);
                    o.GetComponent<Projectile>().setOwner(this);
                    o.SetAsPlayer(this.GetComponent<Ownership>().owner);
                    var direction = (attackee.transform.position - transform.position).normalized * 400;
                    o.GetComponent<Projectile>().setDirection(direction);
                }
                );
            //arrow.transform.Rotate(-90, 0, 0); // Can't figure out how to get this fucking thing to face the right way.

            
            RPC_ShootProjectile();

        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_ShootProjectile()
        {
            onAttack.Invoke();
        }
    }
}