using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEveryOtherCamera : MonoBehaviour
{
    public void Start()
    {
        Camera theOnlyCameraWeNeed = GetComponent<Camera>();
        int i = 0;
        foreach (Object obj in FindObjectsOfType<Camera>())
        {
            i++;
            Camera datTriCount = (Camera)obj;

            if (datTriCount != theOnlyCameraWeNeed)
            {
                datTriCount.enabled = false;
            }
        }
    }
}
