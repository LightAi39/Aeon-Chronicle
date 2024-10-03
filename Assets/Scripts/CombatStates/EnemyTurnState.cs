using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class EnemyTurnState : ICombatState
{
    private readonly CombatSequenceController _controller;
    private readonly TurnOrderEntity _entity;

    public EnemyTurnState(CombatSequenceController combatSequenceController, TurnOrderEntity entity)
    {
        _controller = combatSequenceController;
        _entity = entity;
    }
    
    public void Enter()
    {
        Debug.Log("Enemy Turn Start");
        // TODO: handle AI
        _entity.StartTurn();
        
        // placeholder logic
        _entity.Attack(_entity.EntityToAttackTemp);
    }

    public void Execute()
    {
        if (_controller.IsEntityActionComplete())
        {
            _controller.ChangeState(new ActionState(_controller));
        }
    }

    public void Exit()
    {
        Debug.Log("Enemy Turn End");
    }
}
