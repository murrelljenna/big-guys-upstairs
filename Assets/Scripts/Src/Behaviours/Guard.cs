using game.assets.ai;
using game.assets.utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Attack))]
[RequireComponent(typeof(Movement))]
public class Guard : MonoBehaviour
{
    private Attack attack;
    private Movement movement;

    private Vector3 pointToGuard;
    private float radiusToGuard;
    private int lastNoEnemies = 0;

    private Health intruder;

    void Start()
    {
        attack = GetComponent<Attack>();
        movement = GetComponent<Movement>();
    }

    public void guard(Vector3 point, float radius)
    {
        pointToGuard = point;
        radiusToGuard = radius;
        movement.goTo(point);
        movement.reachedDestination.AddListener(startGuard);
    }

    private void startGuard()
    {
        InvokeRepeating("guardInRange", 1f, 1f);
    }

    private void guardInRange()
    {
        Health[] units = GameUtils.findEnemyUnitsInRange(pointToGuard, radiusToGuard);
        if (units.Length == lastNoEnemies)
        {
            return;
        }

        lastNoEnemies = units.Length;

        Health candidateEnemy = firstWithReasonablePath(units);
        if (candidateEnemy != null)
        {
            attack.attack(candidateEnemy);
            intruder = candidateEnemy;
        }
        else
        {
            movement.goTo(pointToGuard);
        }
    }

    private Health firstWithReasonablePath(Health[] units)
    {
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

    private bool isInRange(Health thingToAttack)
    {
        Vector3 closestPoint = thingToAttack.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);

        float deltaX = this.gameObject.transform.position.x - closestPoint.x;
        float deltaZ = this.gameObject.transform.position.z - closestPoint.z;
        float distance = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);

        return (distance < attack.attackRange);
    }

    private void cancelOrdersAndReturnToGuard()
    {
        attack.cancelOrders();
        movement.goTo(pointToGuard);
    }

    public void cancel()
    {
        CancelInvoke();
        lastNoEnemies = 0;
    }
}
