using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class PlayerTurnState : ICombatState
{
    private readonly CombatSequenceController _controller;
    private readonly TurnOrderEntity _entity;

    public PlayerTurnState(CombatSequenceController combatSequenceController, TurnOrderEntity entity)
    {
        _controller = combatSequenceController;
        _entity = entity;
    }
    
    public void Enter()
    {
        Debug.Log("Player Turn Start");
        // TODO: handle initialization of UI or player actions
        // TODO; remove the stupid
        CombatManager.Instance.targetingManager.ActivateTargeting();
        _entity.StartTurn();
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
        Debug.Log("Player Turn End");
        // TODO; remove the stupid
        CombatManager.Instance.targetingManager.DeactivateTargeting();
    }
}
