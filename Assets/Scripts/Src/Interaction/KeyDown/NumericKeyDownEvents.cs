using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets.interaction {
    public class NumericKeyDownEvents : MonoBehaviour
    {
        [Tooltip("Invoked when numeric key is pressed.")]
        public UnityEvent<int> onKeyPressed;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                onKeyPressed.Invoke(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                onKeyPressed.Invoke(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                onKeyPressed.Invoke(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                onKeyPressed.Invoke(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                onKeyPressed.Invoke(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                onKeyPressed.Invoke(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                onKeyPressed.Invoke(7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                onKeyPressed.Invoke(8);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                onKeyPressed.Invoke(9);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                onKeyPressed.Invoke(0);
            }
        }
    }
}
