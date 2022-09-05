using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObservationEvents : MonoBehaviour
{
    [Tooltip("Invoked when object is observed.")]
    public UnityEvent onObserve;
    [Tooltip("Invoked when observation is broken.")]
    public UnityEvent onBreakObserve;

    public void observe()
    {
        onObserve.Invoke();
    }

    public void breakObserve()
    {
        onBreakObserve.Invoke();
    }
}
