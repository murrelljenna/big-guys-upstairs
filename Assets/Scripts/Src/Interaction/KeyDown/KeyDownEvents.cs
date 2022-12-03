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
        public UnityEvent escOnPressed;

        private float eLastPressed;

        private const float BUFFER_BETWEEN_PRESSES = 0.2f;

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out PlayerNetworkInput input))
            {
                if (input.IsDown(PlayerNetworkInput.BUTTON_ACTION1))
                {
                    fireWithinMeter(ref eLastPressed, eOnPressed);
                }
            }
        }

        private void fireWithinMeter(ref float lastPressed, UnityEvent eventToFire)
        {
            float lastPress = Time.time - lastPressed;

            if (lastPress > BUFFER_BETWEEN_PRESSES)
            {
                eventToFire.Invoke();
                lastPressed = Time.time;
            }
        }
    }


}
