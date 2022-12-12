using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyDownEvents : MonoBehaviour
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            eOnPressed.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escOnPressed.Invoke();
        }
    }
}
