using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseEvents : MonoBehaviour
{
    [Tooltip("Invoked when left mouse button clicked")]
    public UnityEvent leftClick;

    [Tooltip("Invoked when left mouse button double clicked. ")]
    public UnityEvent leftDoubleClick;

    [Tooltip("Invoked when right mouse button clicked")]
    public UnityEvent rightClick;

    private float lastClickTime;
    private const float DOUBLE_CLICK_TIME = 0.2f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float lastClick = Time.time - lastClickTime;
            if (lastClick <= DOUBLE_CLICK_TIME)
            {
                leftDoubleClick.Invoke();
                Debug.Log("DoubleLeftClick");
            }
            else
            {
                leftClick.Invoke();
                Debug.Log("LeftClick");
            }

            lastClickTime = Time.time;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            rightClick.Invoke();
        }
    }
}
