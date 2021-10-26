using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [Tooltip("Invoked at end of time")]
    public UnityEvent timeUp;
    [Tooltip("Invokes in this many seconds")]
    public float timeInSeconds;
    void Start()
    {
        Invoke("fireEvent", timeInSeconds);
    }

    void fireEvent() {
        timeUp.Invoke();
    }
}
