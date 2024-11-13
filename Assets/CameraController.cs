using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera currentCamera;
    public CinemachineVirtualCamera mainCamera;
    
    void Start()
    {
        currentCamera = mainCamera;
        CombatManager.Instance.TargetChanged += ChangeLookAt;
    }
    
    void Update()
    {
        
    }
    
    
    public void SwitchCamera(CinemachineVirtualCamera newCam)
    {
        // Deactivate other cameras and enable the target camera
        currentCamera.enabled = false;
        newCam.enabled = true;
        currentCamera = newCam;
    }

    public void ChangeLookAt()
    {
        if (currentCamera != mainCamera)
        {
            Transform newTarget = CombatManager.Instance.targetingManager.TargetedEnemy.transform;
            currentCamera.LookAt = newTarget;
        }
    }
}
