using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// This script is used as a top level manager. It’s where events are declared and acts as a singleton which you can use to find the other managers.
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    
    public CombatSequenceController combatSequenceController;
    public TurnController turnController;
    public TargetingManager targetingManager;
    public CameraController cameraController;
    public AsyncLoader asyncLoader;
    
    void Awake()
    {
        Instance = this;
    }

    public event UnityAction EntityCompletedAction;
    public event UnityAction CombatStateChanged;
    public event UnityAction TargetChanged; 

    public void DoCombatStateChanged()
    {
        CombatStateChanged?.Invoke();
    }

    public void DoEntityCompletedAction()
    {
        EntityCompletedAction?.Invoke();
    }

    public void DoTargetChanged()
    {
        TargetChanged?.Invoke();
    }
}
