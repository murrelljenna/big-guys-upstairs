using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.ai
{
    public class RangedAttack : Attack
    {
        [Tooltip("Projectile launched by unit")]
        public GameObject projectile;

        override protected void doDamage()
        {
            onAttack.Invoke();
            faceTarget(attackee.transform.position);
            launchProjectileAt(attackee);
        }

        private void launchProjectileAt(Health attackee)
        {
            GameObject arrow = Instantiate(projectile, transform.position, Quaternion.LookRotation((attackee.gameObject.transform.position - transform.position).normalized));
            //arrow.transform.Rotate(-90, 0, 0); // Can't figure out how to get this fucking thing to face the right way.

            
            arrow.GetComponent<Projectile>().setDmg(attackPower);

            arrow.SetAsPlayer(this.GetComponent<Ownership>().owner);

            arrow.GetComponent<Rigidbody>().AddForce((attackee.transform.position - transform.position).normalized * 400);
        }
    }
}