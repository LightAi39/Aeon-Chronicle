using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class EnemyTurnState : ICombatState
{
    private readonly CombatSequenceController _controller;
    private readonly int _characterIndex;

    public EnemyTurnState(CombatSequenceController combatSequenceController, int characterIndex)
    {
        _controller = combatSequenceController;
        _characterIndex = characterIndex;
    }
    
    public void Enter()
    {
        Debug.Log("Enemy Turn Start");
        // TODO: handle AI
    }

    public void Execute()
    {
        if (_controller.IsEnemyActionComplete())
        {
            _controller.ChangeState(new ActionState(_controller));
        }
    }

    public void Exit()
    {
        Debug.Log("Enemy Turn End");
    }
}
