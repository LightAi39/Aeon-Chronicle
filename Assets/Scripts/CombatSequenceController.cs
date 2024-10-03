using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class CombatSequenceController : MonoBehaviour
{
    private ICombatState _currentState;
    
    
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
        _currentState = newState;
        _currentState.Enter();
    }
    
    // condition checks
    public bool IsPlayerActionComplete()
    {
        return true;
        //TODO: real logic
    }

    public bool IsEnemyActionComplete()
    {
        return true;
        //TODO: real logic
    }

    public bool IsActionComplete()
    {
        return true;
        //TODO: real logic
    }
}
