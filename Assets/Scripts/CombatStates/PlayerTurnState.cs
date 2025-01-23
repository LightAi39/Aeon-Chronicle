using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTurnState : ICombatState
{
    private readonly CombatSequenceController _controller;
    private readonly TurnOrderEntity _entity;
    private CameraController _cameraController;

    public PlayerTurnState(CombatSequenceController combatSequenceController, TurnOrderEntity entity)
    {
        _controller = combatSequenceController;
        _entity = entity;
    }
    
    public void Enter()
    {
        if (!_cameraController)
        {
            _cameraController = CombatManager.Instance.cameraController;
        }
        
        Debug.Log("Player Turn Start");
        
        // move camera in position
        _cameraController.SwitchCamera(_entity.camera);
        
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
