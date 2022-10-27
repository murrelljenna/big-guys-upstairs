using Fusion;
using game.assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseEvents : NetworkBehaviour
{
    [Tooltip("Invoked when left mouse button clicked")]
    public UnityEvent leftClick;

    [Tooltip("Invoked when left mouse button double clicked. ")]
    public UnityEvent leftDoubleClick;

    [Tooltip("Invoked when right mouse button clicked")]
    public UnityEvent rightClick;

    private float lastClickTime;
    private const float DOUBLE_CLICK_TIME = 0.2f;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerNetworkInput input))
        {
            if (input.IsDown(PlayerNetworkInput.BUTTON_FIRE))
            {
                float lastClick = Time.time - lastClickTime;
                if (lastClick <= DOUBLE_CLICK_TIME)
                {
                    leftDoubleClick.Invoke();
                }
                else
                {
                    leftClick.Invoke();
                }

                lastClickTime = Time.time;
            }
            else if (input.IsDown(PlayerNetworkInput.BUTTON_FIRE_ALT))
            {
                rightClick.Invoke();
            }
        }

    }
}
