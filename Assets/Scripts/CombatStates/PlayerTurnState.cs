using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class PlayerTurnState : ICombatState
{
    private readonly CombatSequenceController _controller;
    private readonly int _characterIndex;

    public PlayerTurnState(CombatSequenceController combatSequenceController, int characterIndex)
    {
        _controller = combatSequenceController;
        _characterIndex = characterIndex;
    }
    
    public void Enter()
    {
        Debug.Log("Player Turn Start");
        // TODO: handle initialization of UI or player actions
    }

    public void Execute()
    {
        if (_controller.IsPlayerActionComplete())
        {
            _controller.ChangeState(new ActionState(_controller));
        }
    }

    public void Exit()
    {
        Debug.Log("Player Turn End");
    }
}
