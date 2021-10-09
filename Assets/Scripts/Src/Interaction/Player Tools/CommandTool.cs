using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.ui;
using game.assets.utilities;
using game.assets.ai;
using static game.assets.utilities.GameUtils;
using game.assets.economy;

namespace game.assets.interaction
{
    [RequireComponent(typeof(MouseEvents))]
    public class CommandTool : MonoBehaviour
    {
        [Tooltip("Camera Used By Command Tool")]
        public Camera camera;
        [Tooltip("Turn on/off ui for testing")]
        public bool useUi = true;

        private const float QUICK_SELECT_RANGE = 5f;
        private Vector3 VIEWPORT_POINT_TO_RAY = new Vector3(0.5F, 0.5F, 0);

        private MouseEvents mouseEvents;

        public AttackAggregation attackAggregation = new AttackAggregation();

        private CommandUIController uiController;

        private void OnDisable()
        {
            clearSelection();
        }

        private void OnEnable()
        {
            if (useUi == true)
            {
                uiController = GameObject.Find(MagicWords.GameObjectNames.CommandMenu).GetComponent<CommandUIController>();
            }
        }

        private void Start()
        {
            if (camera == null)
            {
                Debug.LogError("CommandTool Camera has not been assigned.");
            }

            mouseEvents = GetComponent<MouseEvents>();

            mouseEvents.leftClick.AddListener(selectUnitIfCan);
            mouseEvents.leftDoubleClick.AddListener(selectUnitsInRadiusIfCan);
            mouseEvents.rightClick.AddListener(orderAttackOrMoveIfCan);
        }

        private void selectUnitIfCan()
        {
            RaycastHit hit;
            Ray ray = camera.ViewportPointToRay(VIEWPORT_POINT_TO_RAY);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Attack attacker = hit.collider.gameObject.GetComponent<Attack>();
                if (attacker != null && attacker.IsMine())
                {
                    if (attackAggregation.contains(attacker))
                    {
                        removeUnit(attacker);
                    }
                    else
                    {
                        attacker.select();
                        attackAggregation.add(attacker);
                        if (useUi)
                        {
                            uiController.addCard(attacker.GetComponent<Health>());
                        }
                    }
                }
            }
        }

        private void removeUnit(Attack attacker)
        {
            attackAggregation.remove(attacker);
            if (useUi)
            {
                uiController.removeCard(attacker.GetComponent<Health>());
            }
        }

        private void selectUnitsInRadiusIfCan()
        {
            RaycastHit hit;
            Ray ray = camera.ViewportPointToRay(VIEWPORT_POINT_TO_RAY);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Attack attacker = hit.collider.gameObject.GetComponent<Attack>();
                if (attacker != null && attacker.IsMine())
                {
                    selectUnitsInRadius(hit.transform.position);
                }
            }           
        }

        private void selectUnitsInRadius(Vector3 point)
        {
            Collider[] hitColliders = Physics.OverlapSphere(point, QUICK_SELECT_RANGE);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Attack unit = hitColliders[i].GetComponent<Attack>();

                if (unit != null && unit.IsMine() && !attackAggregation.contains(unit))
                {
                    attackAggregation.add(unit);
                    if (useUi)
                    {
                        uiController.addCard(unit.GetComponent<Health>()); //TODO: Rethink this a bit, I'm doing unsafe things for convenience
                    }
                }
            }
        }

        private void orderAttackOrMoveIfCan()
        {
            RaycastHit hit;
            Ray ray = camera.ViewportPointToRay(VIEWPORT_POINT_TO_RAY);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                Health health = hit.collider.GetComponent<Health>();
                if (health != null && health.IsEnemy())
                {
                    attackAggregation.attack(health);
                    return;
                }

                Resource resource = hit.collider.GetComponent<Resource>();

                if (resource != null)
                {
                    List<Worker> workers = attackAggregation.unitsThatCanWork();

                    StartCoroutine(assignWorkersToResource(resource, workers));
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameUtils.LayerMask.Terrain))
            {
                attackAggregation.unitsThatCanMove().goTo(hit.point);
                return;
            }
        }

        private IEnumerator assignWorkersToResource(Resource resourceTile, List<Worker> workers)
        {
            foreach (Worker worker in workers)
            {
                int runCount = 0;

                bool added = resourceTile.addWorker(worker); // True if worker was successfully added (there was an available slot) false otherwise.
                if (added)
                {
                    removeUnit(worker.GetComponent<Attack>());
                }

                runCount++;
                if (runCount % 2 == 0)
                {
                    yield return null;
                }
            }
        }

        private void clearSelection() {
            attackAggregation.clear();
            if (useUi)
            {
                uiController.clearCards();
            }
        }
    }
}
