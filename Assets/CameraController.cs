using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// This script simply switches the active camera based on the current acting character and the selected character. The cinemachine package handles the movement.
/// </summary>
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
        ChangeLookAt();
    }

    public void ChangeLookAt()
    {
        if (currentCamera != mainCamera)
        {
            var targeting = CombatManager.Instance.targetingManager;
            var currentlyActing = CombatManager.Instance.turnController.GetCurrentlyActing();
            Transform newTarget;
            if (currentlyActing.team == 0)
            {
                newTarget = targeting.TargetedEnemy.transform;
            }
            else
            {
                newTarget = currentlyActing.entity.transform;
            }
            currentCamera.LookAt = newTarget;
            
            
        }
    }
}
