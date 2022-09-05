using game.assets.ai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.animation
{
    [RequireComponent(typeof(Movement))]
    public class MovementAnimator : MonoBehaviour
    {
        [Tooltip("Animator to communicate movement to")]
        public Animator animator;

        private Movement movement;

        void Start()
        {
            movement = GetComponent<Movement>();
        }

        void Update()
        {
            if (animator != null)
            {
                animator.SetFloat("speed", movement.speed());
            }
        }
    }
}
