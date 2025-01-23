using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Cache the main camera
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // Set the health bar's rotation to match the camera's rotation
        transform.rotation = mainCamera.transform.rotation;
    }
}
