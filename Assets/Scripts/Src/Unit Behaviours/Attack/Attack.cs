using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using game.assets.utilities;
using game.assets.ai.units;
using Fusion;

namespace game.assets.ai
{
    public class Attack : NetworkBehaviour
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
        public UnityEvent onSelect = new UnityEvent();

        [Tooltip("Invoked when unit is deselected")]
        public UnityEvent onDeselect = new UnityEvent();

        [Tooltip("Invoked when enemy is killed")]
        public UnityEvent<Health> enemyKilled = new UnityEvent<Health>();

        [Tooltip("Invoked when unit has not done anything for several seconds")]
        public UnityEvent idled = new UnityEvent();

        public SetShaderColour unitRadius;

        public bool idle = false;

        protected Health attackee;

        [Networked]
        private bool isAttacking { get; set; } = false;
        
        private Movement movement;
        private bool canMove;
        private bool inFight = false;

        private bool updateTargetLive = true;

        private int lastNoEnemies = 0;
        private int frameCount = 0;

        private const bool DEBUG_ATTACKING = false;

        public override void Spawned()
        {
            if (!Object.HasStateAuthority)
            {
                return;
            } 
            movement = gameObject.GetComponent<Movement>();
            canMove = (movement != null);

            if (canMove)
            {
                movement.newMoveOrdered.AddListener(cancelOrders);
            }

            if (!this.IsBarbarian())
            {
                InvokeRepeating("checkEnemiesInRange", 2f, 2f);
            }

            movement?.reachedDestination.AddListener(checkEnemiesInRange);
        }

        public bool isCurrentlyAttacking()
        {
            return isAttacking;
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasInputAuthority)
            {
                setRadiusColorFromState();
            }
            if (!Object.HasStateAuthority)
            {
                return;
            }

            if (updateTargetLive && frameCount % 40 == 0)
            {
                if (attackee != null && canMove)
                {
                    movement.goToSilently(attackee.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
                }
            }

            frameCount++;
        }

        private void setRadiusColorFromState()
        {
            if (!Object.HasInputAuthority || unitRadius == null)
            {
                return;
            }

            if (isAttacking)
            {
                unitRadius.SetColour(Color.red);
            }
            else if (movement != null && movement.moveOrdered)
            {
                unitRadius.SetColour(Color.blue);
            }
            else
            {
                unitRadius.SetColour(Color.green);
            }
        }

        public void attackRandom(Health[] units)
        {
            if (!Object.HasStateAuthority)
            {
                return;
            }
            if (units.Length > 0)
            {
                attack(units.RandomElem());
            }
        }

        private void checkEnemiesInRange()
        {
            if (DEBUG_ATTACKING) Debug.Log("ATK - Checking enemies in range");
            if (isAttacking || (this.IsBarbarian() && canMove && movement.moveOrdered)) {
                if (DEBUG_ATTACKING) Debug.Log("ATK - Move ordered, not attacking");
                return;
            }

            Health[] units = GameUtils.findEnemyUnitsInRange(GetComponent<Collider>().bounds.center, responseRange);
            if (units.Length == lastNoEnemies)
            {
                if (DEBUG_ATTACKING) Debug.Log("ATK - no new enemies in range");
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
                if (DEBUG_ATTACKING) Debug.Log("ATK - Attacking!");
                attack(candidateEnemy);
            }
        }

        private bool checkEnemiesInRange(Vector3 point)
        {
            if (DEBUG_ATTACKING) Debug.Log("ATK - Checking enemies in range");
            if (isAttacking || (this.IsBarbarian() && canMove && movement.moveOrdered))
            {
                if (DEBUG_ATTACKING) Debug.Log("ATK - Move ordered, not attacking");
                return false;
            }

            Health[] units = GameUtils.findEnemyUnitsInRange(point, responseRange);
            if (units.Length == lastNoEnemies)
            {
                if (DEBUG_ATTACKING) Debug.Log("ATK - no new enemies in range");
                return false;
            }

            lastNoEnemies = units.Length;
            Health candidateEnemy;
            candidateEnemy = firstEnemy(units);

            if (candidateEnemy != null && candidateEnemy.GetComponent<DoNotAutoAttack>() == null)
            {
                if (DEBUG_ATTACKING) Debug.Log("ATK - Attacking!");
                attack(candidateEnemy);
                return true;
            }

            return false;
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
                    if (DEBUG_ATTACKING) Debug.Log("ATK - unit " + units[i].gameObject.name + " is enemy");
                    float dist = movement.pathLength(units[i].GetComponent<Transform>().position);
                    if (isInRange(units[i]) || dist < 3f)
                    {
                        if (DEBUG_ATTACKING) Debug.Log("ATK - unit " + units[i].gameObject.name + " is in range");
                        return units[i];
                    }
                }
            }

            return null;
        }

        private void reportEnemyDead(Health h) {
            enemyKilled.Invoke(h);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_FireAttackEvents()
        {
            onAttackOrdered.Invoke();
        }

        public void attack(Health attackee)
        {
            if (!Object.HasStateAuthority)
            {
                return;
            }

            if (attackee == null || !attackee.IsEnemyOf(this))
            {
                return;
            }
            cancelOrders();
            idle = false;
            attackee.onZeroHP.AddListener(cancelOrders);
            //attackee.onZeroHP.AddListener(goToUnit);
            RPC_FireAttackEvents();
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

        private void goToUnit(Health unit) {

            if (canMove)
            {
                Debug.Log("ATK - Just going to unit now");
                movement.goTo(unit.transform.position);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Object == null || !Object.HasStateAuthority)
            {
                return;
            }

            Health collidingUnit = collision.gameObject.GetComponent<Health>();
            if (
                collidingUnit != null 
                && collidingUnit.IsEnemyOf(this) 
                && collidingUnit.HP > 0 && !inFight
                && collidingUnit.GetComponent<DoNotAutoAttack>() == null
                )
            {
                if (DEBUG_ATTACKING) Debug.Log("ATK - Colliding with enemy, attacking");
                cancelOrders();
                attack(collidingUnit);
            }
        }

        private IEnumerator moveUntilInRangeAndAttack(Health attackee)
        {
            if (attackee.HP > 0)
            {
                if (attackee.GetComponent<Movement>() != null)
                {
                    updateTargetLive = true;
                }

                setAttackee(attackee);
                isAttacking = true;
                if (DEBUG_ATTACKING) Debug.Log("ATK - Moving to get in range");

                movement.goToSilently(attackee.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
                yield return new WaitUntil(() => isInRange(attackee));
                if (DEBUG_ATTACKING) Debug.Log("ATK - In range, attacking now");
                movement.stop();

                InvokeRepeating("doDamageIfShould", 0f, this.attackRate);
            }
        }

        private void setAttackee(Health newAttackee)
        {
            attackee?.onZeroHP?.RemoveListener(cancelOrders);
            attackee?.onZeroHP?.RemoveListener(goToUnit);

            attackee = newAttackee;
        }

        private IEnumerator waitUntilInRangeAndAttack(Health attackee) {
            if (attackee.HP > 0)
            {
                if (attackee.GetComponent<Movement>() != null)
                {
                    updateTargetLive = true;
                }

                setAttackee(attackee);
                isAttacking = true;

                yield return new WaitUntil(() => isInRange(attackee));

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
                if (DEBUG_ATTACKING) Debug.Log("ATK - Ok this guy I'm attacking isn't attackable so I'm switching up");
                cancelOrders();
                checkEnemiesInRange();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_FireDamageEvents()
        {
            onAttack.Invoke();
        }

        protected virtual void doDamage()
        {
            RPC_FireDamageEvents();
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
            cancelOrdersIgnoringMovement();
        }

        public void cancelOrders()
        {
            if (Object == null || !Object.HasStateAuthority)
            {
                return;
            }

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
            setAttackee(null);
            CancelInvoke("reportIdle");
            Invoke("reportIdle", 3f);
        }

        private void cancelOrdersIgnoringMovement()
        {
            if (Object == null || !Object.HasStateAuthority)
            {
                return;
            }

            if (attackee != null)
            {
                attackee.onZeroHP.RemoveListener(reportEnemyDead);
            }
            StopAllCoroutines();
            updateTargetLive = false;
            inFight = false;
            CancelInvoke("doDamageIfShould");
            isAttacking = false;
            setAttackee(null);
            CancelInvoke("reportIdle");
            Invoke("reportIdle", 3f);
        }

        private void reportIdle()
        {
            if (canMove && !GetComponent<Movement>().moveOrdered && isAttacking == false)
            {
                idled.Invoke();
                idle = true;
            }
        }

        public void select()
        {
            RPC_fireSelectEvents();
        }

        public void deselect()
        {
            RPC_fireDeselectEvents();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_fireSelectEvents()
        {
            onSelect.Invoke();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_fireDeselectEvents()
        {
            onDeselect.Invoke();
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
