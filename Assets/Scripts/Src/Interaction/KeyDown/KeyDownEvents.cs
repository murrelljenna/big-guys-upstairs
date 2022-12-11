using Fusion;
using game.assets.player;
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
    public class KeyDownEvents : NetworkBehaviour
    {
        [Tooltip("Invoked when E key is pressed.")]
        public UnityEvent eOnPressed = new UnityEvent();
        [Tooltip("Invoked when X key is pressed.")]
        public UnityEvent xOnPressed;
        [Tooltip("Invoked when U key is pressed.")]
        public UnityEvent uOnPressed;
        [Tooltip("Invoked when R key is pressed.")]
        public UnityEvent rOnPressed;
        [Tooltip("Invoked when Esc key is pressed.")]
        public UnityEvent escOnPressed = new UnityEvent();

        private float eLastPressed;
        private float escLastPressed;

        private const float BUFFER_BETWEEN_PRESSES = 0.05f;

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority)
            {
                return;
            }
            if (GetInput(out PlayerNetworkInput input))
            {
                if (input.IsDown(PlayerNetworkInput.BUTTON_ACTION1))
                {
                    fireWithinMeter(ref eLastPressed, eOnPressed);
                }

                if (input.IsDown(PlayerNetworkInput.BUTTON_ESC))
                {
                    fireWithinMeter(ref escLastPressed, escOnPressed);
                }
            }
        }

        private void fireWithinMeter(ref float lastPressed, UnityEvent eventToFire)
        {
            float lastPress = Time.time - lastPressed;
            Debug.Log("INS - Trying to invoke. ");
            if (lastPress > BUFFER_BETWEEN_PRESSES)
            {
                Debug.Log("INS - INVOKING");
                eventToFire.Invoke();
                lastPressed = Time.time;
            }
        }
    }


}
