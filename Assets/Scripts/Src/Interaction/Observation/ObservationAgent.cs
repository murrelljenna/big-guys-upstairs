using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets.interaction
{
    /* 
     * Carries our Unity API calls to be mocked for testing.
     */
    interface ICameraRaycastObserver
    {
        ObservationEvents RaycastToObservable(Camera cam);
    }

    [RequireComponent(typeof(Camera))]
    public class ObservationAgent : MonoBehaviour, ICameraRaycastObserver
    {
        [Tooltip("Invoked once when agent observes another object.")]
        public UnityEvent onObserve;
        [Tooltip("Invoked once when agent breaks observation with another object.")]
        public UnityEvent onBreakObserve;

        public Ownership owner;

        private Camera cam;

        private ObservationEvents lastObserved;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private bool canView(Ownership targetOwnership)
        {
            if (targetOwnership == null || owner == null)
            {
                return true;
            }
            
            if (owner.owner == targetOwnership.owner)
            {
                return true;
            }

            return false;
        }

        #region ICameraRaycastObserver implementation
        public ObservationEvents RaycastToObservable(Camera cam)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                return hit.collider.gameObject.GetComponent<ObservationEvents>();
            }

            return null;
        }

        #endregion

        // Update is called once per frame
        void Update()
        {
            ObservationEvents nextObserved = RaycastToObservable(cam);
            if (nextObserved != lastObserved)
            {
                if (lastObserved != null)
                {
                    lastObserved.breakObserve();
                    onBreakObserve.Invoke();
                }

                if (nextObserved != null && canView(nextObserved.GetComponent<Ownership>()))
                {
                    onObserve.Invoke();
                    nextObserved.observe();
                }

                lastObserved = nextObserved;
            }
        }
    }
}
