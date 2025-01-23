using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

/// <summary>
/// This script is essentially a modified state machine. It uses the scripts in the CombatStates folder to execute different behaviour as states change.
/// States have an Enter, Execute, and Exit method through the ICombatState interface. You should prepare the state in the Enter method, Execute it in the method of the same name, and perform cleanup tasks as well as trigger a new state in the Exit method.
/// You may also choose to enter new states through other means, in which case you should make replacement logic for the current handovers in the Exit methods of the current implementation.
/// </summary>
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
