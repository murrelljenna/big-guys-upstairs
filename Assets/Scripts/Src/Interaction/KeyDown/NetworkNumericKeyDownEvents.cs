using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets;
using UnityEngine.Events;

public class NetworkNumericKeyDownEvents : NetworkBehaviour
{
    [Tooltip("Invoked when numeric key is pressed.")]
    public UnityEvent<int> onKeyPressed;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerNetworkInput input)) {
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA1))
            {
                onKeyPressed.Invoke(1);
            }
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA2))
            {
                onKeyPressed.Invoke(2);
            }
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA3))
            {
                onKeyPressed.Invoke(3);
            }
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA4))
            {
                onKeyPressed.Invoke(4);
            }
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA5))
            {
                onKeyPressed.Invoke(5);
            }
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA6))
            {
                onKeyPressed.Invoke(6);
            }
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA7))
            {
                onKeyPressed.Invoke(7);
            }
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA8))
            {
                onKeyPressed.Invoke(8);
            }
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA9))
            {
                onKeyPressed.Invoke(9);
            }
            if (input.IsDown(PlayerNetworkInput.BUTTON_ALPHA0))
            {
                onKeyPressed.Invoke(0);
            }
        }
    }
}
