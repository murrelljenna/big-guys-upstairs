using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.ui;
using game.assets.utilities;
using game.assets.ai;

namespace game.assets.interaction
{
    [RequireComponent(typeof(MouseEvents))]
    public class CommandTool : MonoBehaviour
    {
        [Tooltip("Camera Used By Command Tool")]
        public Camera camera;

        private const float QUICK_SELECT_RANGE = 5f;
        private Vector3 VIEWPORT_POINT_TO_RAY = new Vector3(0.5F, 0.5F, 0);

        private MouseEvents mouseEvents;

        CommandToolUIController uiController;

        public AttackAggregation attackAggregation = new AttackAggregation();

        private void OnDisable()
        {
            clearSelection();
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
                    attackAggregation.toggleInclude(attacker);
                }
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

                if (unit != null && unit.IsMine())
                {
                    attackAggregation.add(unit);
                }
            }
        }

        private void orderAttackOrMoveIfCan()
        {
            RaycastHit hit;
            Ray ray = camera.ViewportPointToRay(VIEWPORT_POINT_TO_RAY);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                Health health = hit.collider.GetComponent<Health>();
                if (health != null && !health.IsMine())
                {
                    attackAggregation.attack(health);
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameUtils.LayerMask.Terrain))
            {
                attackAggregation.unitsThatCanMove().goTo(hit.point);
            }
        }

        private void clearSelection() {
            attackAggregation.clear();
        }
    }
}
