using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class CombatSequenceController : MonoBehaviour
{
    private ICombatState _currentState;

    private bool _entityCompletedAction = false;
    private bool _actionComplete = true;

    void Awake()
    {
        CombatManager.Instance.EntityCompletedAction += () => _entityCompletedAction = true;
        CombatManager.Instance.CombatStateChanged += SetActionsToIncomplete;
    }
    
    void Start()
    {
        _currentState = new TurnPickerState(this);
        _currentState.Enter();
    }
    
    void Update()
    {
        _currentState.Execute();
    }

    public void ChangeState(ICombatState newState)
    {
        _currentState.Exit();
        CombatManager.Instance.DoCombatStateChanged();
        _currentState = newState;
        _currentState.Enter();
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
