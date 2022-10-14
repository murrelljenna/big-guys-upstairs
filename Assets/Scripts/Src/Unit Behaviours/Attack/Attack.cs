using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using game.assets.utilities;
using game.assets.ai.units;

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
        public float attackRate;

        [Tooltip("Attack Range")]
        public float attackRange;

        [Tooltip("Range at which unit will automatically engage")]
        public float responseRange;

        [Tooltip("Invoked on each attack")]
        public UnityEvent onAttack;

        [Tooltip("Invoked when attack is ordered")]
        public UnityEvent onAttackOrdered;

        [Tooltip("Invoked when attack is selected")]
        public UnityEvent onSelect;

        [Tooltip("Invoked when enemy is killed")]
        public UnityEvent<Health> enemyKilled = new UnityEvent<Health>();


        protected Health attackee;

        private bool isAttacking = false;
        
        private Movement movement;
        private bool canMove;
        private bool inFight = false;

        private bool updateTargetLive = true;

        private int lastNoEnemies = 0;
        private int frameCount = 0;

        void Start()
        {
            movement = gameObject.GetComponent<Movement>();
            canMove = (movement != null);

            if (canMove)
            {
                movement.newMoveOrdered.AddListener(cancelOrders);
            }

            if (!this.IsBarbarian())
            {
                Debug.Log(gameObject.name);
                InvokeRepeating("checkEnemiesInRange", 2f, 2f);
            }
        }

        public bool isCurrentlyAttacking()
        {
            return isAttacking;
        }

        void Update()
        {
            if (updateTargetLive && frameCount % 40 == 0)
            {
                if (attackee != null && canMove)
                {
                    movement.goToSilently(attackee.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
                }
            }
            frameCount++;
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
            Health candidateEnemy;
            if (canMove)
            {
                candidateEnemy = firstWithReasonablePath(units);
            }
            else
            {
                candidateEnemy = firstEnemy(units);
            }

            if (candidateEnemy != null && candidateEnemy.GetComponent<DoNotAutoAttack>() == null)
            {
                attack(candidateEnemy);
            }
        }

        private Health firstEnemy(Health[] units)
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].IsEnemyOf(this) && units[i].HP > 0)
                {
                    if (isInRange(units[i]))
                    {
                        return units[i];
                    }
                }
            }

            return null;
        }

        private Health firstWithReasonablePath(Health[] units) {
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].IsEnemyOf(this) && units[i].HP > 0)
                {
                    float dist = movement.pathLength(units[i].GetComponent<Transform>().position);
                    if (isInRange(units[i]) || dist < 3f)
                    {
                        return units[i];
                    }
                }
            }

            return null;
        }

        private void reportEnemyDead(Health h) {
            enemyKilled.Invoke(h);
            Invoke("checkEnemiesInRange", 0.1f);
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
            attackee.onZeroHP.AddListener(reportEnemyDead);
            if (canMove)
            {
                StartCoroutine(moveUntilInRangeAndAttack(attackee));
            }
            else
            {
                StartCoroutine(waitUntilInRangeAndAttack(attackee));
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Health collidingUnit = collision.gameObject.GetComponent<Health>();
            if (
                collidingUnit != null 
                && collidingUnit.IsEnemyOf(this) 
                && collidingUnit.HP > 0 && !inFight
                && collidingUnit.GetComponent<DoNotAutoAttack>() == null
                )
            {
                cancelOrders();
                attack(collidingUnit);
            }
        }

        private IEnumerator moveUntilInRangeAndAttack(Health attackee)
        {
             if (attackee.HP > 0) {
                if (attackee.GetComponent<Movement>() != null) {
                    updateTargetLive = true;
                }

                this.attackee = attackee;
                isAttacking = true;

                movement.goToSilently(attackee.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
                yield return new WaitUntil (() => isInRange(attackee));
                movement.stop();

                InvokeRepeating("doDamageIfShould", 0f, this.attackRate);
            }

        }

        private IEnumerator waitUntilInRangeAndAttack(Health attackee) {
            if (attackee.HP > 0) {
                if (attackee.GetComponent<Movement>() != null) {
                    updateTargetLive = true;
                }

                this.attackee = attackee;
                isAttacking = true;

                yield return new WaitUntil (() => isInRange(attackee));

                inFight = true;
                InvokeRepeating("doDamageIfShould", 0f, this.attackRate);
            }
        }

        private void doDamageIfShould()
        {
            if (isAttackable(attackee))
            {
                doDamage();
            }
            else
            {
                cancelOrders();
            }
        }

        protected virtual void doDamage()
        {
            onAttack.Invoke();
            faceTarget(attackee.transform.position);
            this.attackee.lowerHP(attackPower, this);
        }


        private bool isAttackable(Health attackee) {
            return (attackee != null && attackee.HP > 0);
        }

        private bool isInRange(Health thingToAttack) {
            Vector3 closestPoint = thingToAttack.gameObject.GetComponent<Collider>().ClosestPointOnBounds(gameObject.transform.position);

            float deltaX = this.gameObject.transform.position.x - closestPoint.x;
            float deltaZ = this.gameObject.transform.position.z - closestPoint.z;
            float distance = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);

            return (distance < this.attackRange);
        }

        protected void faceTarget(Vector3 position)
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
            if (attackee != null)
            {
                attackee.onZeroHP.RemoveListener(reportEnemyDead);
            }
            StopAllCoroutines();
            updateTargetLive = false;
            inFight = false;
            CancelInvoke("doDamageIfShould");
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
