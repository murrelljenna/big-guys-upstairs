using Fusion;
using game.assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterViewHandler : NetworkBehaviour
{
    private const float ySensitivity = 4f;
    private const float xSensitivity = 4f;
    private Camera maybeLocalCamera;
    private float cameraRotationY = 0f;
    private float cameraRotationX = 0f;

    public bool isLocal = false;

    public bool isActive = true;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            maybeLocalCamera = GetComponentInChildren<Camera>();
            isLocal = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update()
    {
        if (isActive && Object.HasInputAuthority)
        {
            if (maybeLocalCamera != null)
            {
                cameraRotationY += Input.GetAxis("Mouse Y") * ySensitivity;
                cameraRotationY = Mathf.Clamp(cameraRotationY, -90, 90);

                Quaternion cameraYRotation = Quaternion.Euler(-cameraRotationY, 0f, 0f);

                //maybeLocalCamera.transform.localRotation = cameraYRotation;
            }

            cameraRotationX += Input.GetAxis("Mouse X") * xSensitivity;

        }
    }

    public override void FixedUpdateNetwork()
    {
        if (isActive && GetInput(out PlayerNetworkInput input))
        {
            transform.localRotation = Quaternion.Euler(-input.cameraRotationY, input.cameraRotationX, 0f);
        }
    }

    public float getXRotation()
    {
        return cameraRotationX;
    }

    public float getYRotation()
    {
        return cameraRotationY;
    }
}
