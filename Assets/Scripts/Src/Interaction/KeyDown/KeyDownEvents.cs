using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Listen for input events and translate into equivalent UnityEvent invocations. 
 * 
 * Attach behaviours to these events.
 * 
 */

namespace game.assets.interaction
{
    public class KeyDownEvents : MonoBehaviour
    {
        [Tooltip("Invoked when E key is pressed.")]
        public UnityEvent eOnPressed;
        [Tooltip("Invoked when X key is pressed.")]
        public UnityEvent xOnPressed;
        [Tooltip("Invoked when U key is pressed.")]
        public UnityEvent uOnPressed;
        [Tooltip("Invoked when R key is pressed.")]
        public UnityEvent rOnPressed;
        [Tooltip("Invoked when Esc key is pressed.")]
        public UnityEvent escOnPressed;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                eOnPressed.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                uOnPressed.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                xOnPressed.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                rOnPressed.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                escOnPressed.Invoke();
            }
        }
    }
}
