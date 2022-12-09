using System;
using UnityEngine;


namespace UnityStandardAssets.Effects
{
    public class Hose : MonoBehaviour
    {
        public float maxPower = 20;
        public float minPower = 5;
        public float changeSpeed = 5;
        public ParticleSystem[] hoseWaterSystems;
        public Renderer systemRenderer;


        // Update is called once per frame
        private void Update()
        {

            foreach (var system in hoseWaterSystems)
            {
                //system.main.simulationSpeed = 0.15f;
            }
        }
    }
}
