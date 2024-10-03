using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    
    public CombatSequenceController combatSequenceController;
    public TurnController turnController;
    
    void Awake()
    {
        Instance = this;
    }

    public event UnityAction EntityCompletedAction;
    public event UnityAction CombatStateChanged;

    public void DoCombatStateChanged()
    {
        CombatStateChanged?.Invoke();
    }

    public void DoEntityCompletedAction()
    {
        EntityCompletedAction?.Invoke();
    }
}
