using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class CombatSequenceController : MonoBehaviour
{
    public ICombatState CurrentState { get; private set; }

    private bool _entityCompletedAction = false;
    private bool _actionComplete = true;

    void Awake()
    {
        CombatManager.Instance.EntityCompletedAction += () => _entityCompletedAction = true;
        CombatManager.Instance.CombatStateChanged += SetActionsToIncomplete;
    }
    
    void Start()
    {
        CurrentState = new TurnPickerState(this);
        CurrentState.Enter();
    }
    
    void Update()
    {
        CurrentState.Execute();
    }

    public void ChangeState(ICombatState newState)
    {
        CurrentState.Exit();
        CombatManager.Instance.DoCombatStateChanged();
        CurrentState = newState;
        CurrentState.Enter();
    }
    
    // condition checks
    public bool IsEntityActionComplete()
    {
        return _entityCompletedAction;
    }
    
    public bool IsActionComplete()
    {
        return _actionComplete;
    }

    public void SetActionsToIncomplete()
    {
        _entityCompletedAction = false;
    }
    
    
}
