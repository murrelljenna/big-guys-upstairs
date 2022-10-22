using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimationController : MonoBehaviour
{
    public KeyCode[] keycodes = new KeyCode[] {
        KeyCode.E,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
    };

    public GameObject upButtonE;
    public GameObject downButtonE;

    public GameObject upButton1;
    public GameObject downButton1;

    public GameObject upButton2;
    public GameObject downButton2;

    public GameObject upButton3;
    public GameObject downButton3;

    public GameObject upButton4;
    public GameObject downButton4;

    public GameObject upButton5;
    public GameObject downButton5;

    public GameObject upButton6;
    public GameObject downButton6;

    private GameObject[] upButtons;

    private GameObject[] downButtons;

    void Start()
    {
        upButtons = new GameObject[] {
            upButtonE,
            upButton1,
            upButton2,
            upButton3,
            upButton4,
            upButton5,
            upButton6
        };

        downButtons = new GameObject[] {
            downButtonE,
            downButton1,
            downButton2,
            downButton3,
            downButton4,
            downButton5,
            downButton6
        };
    }

    void Update()
    {
        for (int i = 0; i < keycodes.Length && i < upButtons.Length && i < downButtons.Length; i++)
        {
            var keyCode = keycodes[i];

            var downButton = downButtons[i];
            var upButton = upButtons[i];

            if (downButton == null || upButton == null)
            {
                continue;
            }

            if (Input.GetKey(keyCode))
            {
                deactivateIfNotNull(upButton);
                activateIfNotNull(downButton);
            }
            else
            {
                deactivateIfNotNull(downButton);
                activateIfNotNull(upButton);
            }
        }
    }

    private void deactivateIfNotNull(GameObject button)
    {
        if (button != null)
        {
            button.SetActive(false);
        }
    }

    private void activateIfNotNull(GameObject button)
    {
        if (button != null)
        {
            button.SetActive(true);
        }
    }
}
