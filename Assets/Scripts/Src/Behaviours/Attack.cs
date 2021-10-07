using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using game.assets.utilities;

namespace game.assets.ai
{
    public interface IAttack
    {
        void attack(Health attackee);
    }

    public class Attack : MonoBehaviour, IAttack
    {
        [Tooltip("Damage per attack")]
        public int attackPower;

        [Tooltip("Attack Rate")]
        public int attackRate;

        [Tooltip("Attack Range")]
        public int attackRange;

        [Tooltip("Range at which unit will automatically engage")]
        public int responseRange;

        [Tooltip("Invoked on each attack")]
        public UnityEvent onAttack;

        [Tooltip("Invoked when attack is ordered")]
        public UnityEvent onAttackOrdered;

        [Tooltip("Invoked when attack is selected")]
        public UnityEvent onSelect;


        private Health attackee;

        private bool isAttacking = false;
        
        private Movement movement;
        private bool canMove;

        private bool updateTargetLive = true;

        private int lastNoEnemies = 0;

        void Start()
        {
            movement = gameObject.GetComponent<Movement>();
            canMove = (movement != null);

            if (canMove)
            {
                InvokeRepeating("checkEnemiesInRange", attackRange, attackRange);
            }
        }

        private void checkEnemiesInRange()
        {
            if (isAttacking || (canMove && movement.moveOrdered)) {
                return;
            }

            Health[] units = GameUtils.findEnemyUnitsInRange(GetComponent<Collider>().bounds.center, responseRange);

            if (units.Length == lastNoEnemies)
            {
                return;
            }

            lastNoEnemies = units.Length;

            Health candidateEnemy = firstWithReasonablePath(units);

            if (candidateEnemy != null)
            {
                attack(candidateEnemy);
            }
        }

        private Health firstWithReasonablePath(Health[] units) {
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].IsEnemyOf(this) && units[i].HP > 0)
                {
                    float dist = movement.pathLength(units[i].GetComponent<Transform>().position);
                    if (isInRange(units[i].GetComponent<Health>()) || dist < 3f)
                    {
                        return units[i];
                    }
                }
            }

            return null;
        }

        public void attack(Health attackee)
        {
            if (!attackee.IsEnemyOf(this))
            {
                return;
            }
            cancelOrders();
            attackee.onZeroHP.AddListener(cancelOrders);
            onAttackOrdered.Invoke();
            if (canMove)
            {
                StartCoroutine(moveUntilInRangeAndAttack(attackee));
            }
            else
            {
                StartCoroutine(waitUntilInRangeAndAttack(attackee));
            }
        }

        private IEnumerator moveUntilInRangeAndAttack(Health attackee)
        {
             if (attackee.HP > 0) {
                if (attackee.tag == "unit") {
                    updateTargetLive = true;
                }

                this.attackee = attackee;
                isAttacking = true;

                movement.goTo(attackee.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
                yield return new WaitUntil (() => isInRange(attackee));
                movement.stop();

                InvokeRepeating("doDamage", this.attackRate, this.attackRate);
            }

        }

        private IEnumerator waitUntilInRangeAndAttack(Health attackee) {
            if (attackee.HP > 0) {
                if (attackee.tag == "unit") {
                    updateTargetLive = true;
                }

                this.attackee = attackee;
                isAttacking = true;

                yield return new WaitUntil (() => isInRange(attackee));

                InvokeRepeating("doDamage", this.attackRate, this.attackRate);
            }
        }

        private void doDamage()
        {
            if (isAttackable(attackee))
            {

                onAttack.Invoke();
                faceTarget(this.attackee.transform.position);
                this.attackee.lowerHP(this.attackPower);
            }
            else
            {
                cancelOrders();
            }
        }

        private bool isAttackable(Health attackee) {
            return (attackee != null && attackee.HP > 0);
        }

        private bool isInRange(Health thingToAttack) {
            Vector3 closestPoint = thingToAttack.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);

            float deltaX = this.gameObject.transform.position.x - closestPoint.x;
            float deltaZ = this.gameObject.transform.position.z - closestPoint.z;
            float distance = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);

            return (distance < this.attackRange);
        }

        private void faceTarget(Vector3 position)
        {
            if (canMove)
            {
                movement.faceTowards(position);
            }
        }

        public void cancelOrders(Health health)
        {
            cancelOrders();
        }

        public void cancelOrders()
        {
            updateTargetLive = false;
            CancelInvoke("doDamage");
            // Cancel animation stuff - CancelInvoke("fireProjectile");
            if (canMove)
            {
                movement.stop();
            }
            isAttacking = false;
            attackee = null;
        }

        public void select()
        {
            onSelect.Invoke();
        }

        public void OnDestroy()
        {
            CancelInvoke();
        }

        public void OnDisable()
        {
            CancelInvoke();
        }
    }
}
