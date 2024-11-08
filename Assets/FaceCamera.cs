using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // Cache the main camera's transform
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // Make the object look at the camera
        transform.LookAt(mainCameraTransform);
        transform.Rotate(0, 180, 0); // Adjust if the object appears flipped
    }
}
