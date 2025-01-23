using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class EnemyTurnState : ICombatState
{
    private readonly CombatSequenceController _controller;
    private readonly TurnOrderEntity _entity;
    private CameraController _cameraController;

    public EnemyTurnState(CombatSequenceController combatSequenceController, TurnOrderEntity entity)
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
        
        Debug.Log("Enemy Turn Start");
        if (_entity.currentHp == 0)
        {
            // TODO: handle death
            _entity.EndTurn();
        }
        else
        {
            // pick a random target
            var friendlies = CombatManager.Instance.targetingManager.Friendlies;
            var target = friendlies[Random.Range(0, friendlies.Count)];
            
            // move camera in position
            _cameraController.SwitchCamera(target.camera);
            
            // initialize turn
            _entity.StartTurn();
        
            // attack
            _entity.UseSkill(_entity.characterScriptableObject.equipment[0].basicAttack, target);
        }
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
