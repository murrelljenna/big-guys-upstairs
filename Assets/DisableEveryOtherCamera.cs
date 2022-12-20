using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEveryOtherCamera : MonoBehaviour
{
    private List<Camera> disabledCameras = new List<Camera>();
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
                disabledCameras.Add(datTriCount);
            }
        }
    }

    public void OnDestroy()
    {
        disabledCameras.ForEach(cam => { if (cam != null) cam.enabled = true; } );
    }
}
