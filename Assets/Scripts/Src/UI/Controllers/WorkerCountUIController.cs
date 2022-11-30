using game.assets.economy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace game.assets.interaction
{
    [RequireComponent(typeof(Resource))]
    public class WorkerCountUIController : MonoBehaviour
    {
        public Text workerCount;

        private Resource resource;

        private void Start()
        {
            resource = GetComponent<Resource>();
            resource.workerCountChanged.AddListener(updateUI);
            updateUI(resource.workers.Count);
        }

        private void updateUI(int count)
        {
            workerCount.text = $"{count}/{resource.maxWorkers}";
        }
    }
}
