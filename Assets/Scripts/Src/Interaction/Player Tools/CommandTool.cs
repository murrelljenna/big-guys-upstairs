using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.ui;
using game.assets.utilities;
using game.assets.ai;
using static game.assets.utilities.GameUtils;
using game.assets.economy;
using UnityEngine.AI;

namespace game.assets.interaction
{
    [RequireComponent(typeof(MouseEvents))]
    public class CommandTool : MonoBehaviour
    {
        [Tooltip("Camera Used By Command Tool")]
        public Camera cam;
        [Tooltip("Turn on/off ui for testing")]
        public bool useUi = true;

        private const float QUICK_SELECT_RANGE = 5f;
        private Vector3 VIEWPORT_POINT_TO_RAY = new Vector3(0.5F, 0.5F, 0);

        private MouseEvents mouseEvents;

        public AttackAggregation attackAggregation = new AttackAggregation();

        private CommandUIController uiController;

        private void OnDisable()
        {
            if (useUi == true && uiController != null)
            {
                uiController.gameObject.SetActive(false);
            }
            clearSelection();
        }

        private void OnEnable()
        {
            if (useUi == true && uiController != null)
            {
                uiController.gameObject.SetActive(true);
            }
        }

        private void getUIController()
        {
            uiController = GameObject.Find(MagicWords.GameObjectNames.CommandMenu)?.GetComponent<CommandUIController>();
        }

        private void Awake()
        {
            
            if (cam == null)
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
            Ray ray = cam.ViewportPointToRay(VIEWPORT_POINT_TO_RAY);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameUtils.LayerMask.Unit))
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
                        addUnit(attacker);
                    }
                }
            }
        }

        private void addUnit(Attack unit)
        {
            if (attackAggregation.contains(unit))
            {
                return;
            }

            unit.select();
            attackAggregation.add(unit);
            if (useUi)
            {
                uiController.addCard(unit.GetComponent<Health>());
            }
        }

        private void removeUnit(Attack attacker)
        {
            attackAggregation.remove(attacker);
            if (useUi)
            {
                uiController.removeCard(attacker.GetComponent<Health>());
            }

            attacker.deselect();
        }

        private void selectUnitsInRadiusIfCan()
        {
            RaycastHit hit;
            Ray ray = cam.ViewportPointToRay(VIEWPORT_POINT_TO_RAY);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameUtils.LayerMask.Attackable))
            {
                Attack attacker = hit.collider.gameObject.GetComponent<Attack>();
                if (attacker != null && attacker.IsMine())
                {
                    if (isActiveWorker(attacker))
                        selectWorkersInRadius(hit.transform.position);
                    else
                        selectUnitsInRadius(hit.transform.position);
                }
            } else if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameUtils.LayerMask.Resource))
            {
                Resource resource = hit.collider.gameObject.GetComponent<Resource>();
                Debug.Log("Hit a resource");
                if (resource != null)
                {
                    resource.workers.ForEach((Worker worker) =>
                    {
                        var atk = worker.GetComponent<Attack>();
                        if (worker.IsMine() && atk != null) {
                            addUnit(atk);
                        }
                    });
                }
            }
        }

        private bool isActiveWorker(Attack unit) {
            if (unit == null) return false;
            Worker maybeWorker = unit.GetComponent<Worker>();
            if (maybeWorker == null) return false;
            return maybeWorker.isCollectingResources() || maybeWorker.isCurrentlyBuilding();
        }

        private void selectUnitsInRadius(Vector3 point)
        {
            Collider[] hitColliders = Physics.OverlapSphere(point, QUICK_SELECT_RANGE);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Attack unit = hitColliders[i].GetComponent<Attack>();

                if (unit != null && unit.IsMine() && !attackAggregation.contains(unit) && !isActiveWorker(unit))
                {
                    addUnit(unit);
                }
            }
        }

        private void selectWorkersInRadius(Vector3 point)
        {
            Collider[] hitColliders = Physics.OverlapSphere(point, QUICK_SELECT_RANGE);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Attack unit = hitColliders[i].GetComponent<Attack>();

                bool isWorking = isActiveWorker(unit);

                if (unit != null && unit.IsMine() && !attackAggregation.contains(unit) && isWorking)
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
            Ray ray = cam.ViewportPointToRay(VIEWPORT_POINT_TO_RAY);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameUtils.LayerMask.All)) {
                Health health = hit.collider.GetComponent<Health>();
                if (health != null && health.IsEnemy())
                {
                    attackAggregation.attack(health);
                    return;
                }

                Resource resource = hit.collider.GetComponent<Resource>();

                if (resource != null && !noWorkers(attackAggregation.units) && resource.workers.Count < resource.maxWorkers)
                {
                    List<Worker> workers = attackAggregation.unitsThatCanWork();

                    StartCoroutine(assignWorkersToResource(resource, workers));
                    return;
                }

                Construction construction = hit.collider.GetComponent<Construction>();

                if (construction != null && construction.IsMine())
                {
                    List<Worker> workers = attackAggregation.unitsThatCanWork();

                    workers.ForEach(worker => worker.setBuildingTarget(construction));
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameUtils.LayerMask.Terrain))
            {
                NavMeshHit navMeshHit;
                bool isOnMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, 0.2f, NavMesh.AllAreas);
                if (isOnMesh)
                {
                    attackAggregation.unitsThatCanMove().goTo(hit.point);
                }
                return;
            }
        }

        private bool noWorkers(List<Attack> units)
        {
            return units.TrueForAll((Attack unit) => unit.GetComponent<Worker>() == null);
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

        public void clearSelection() {
            attackAggregation.clear();
            if (useUi && uiController != null)
            {
                uiController.clearCards();
            }
        }
    }
}
