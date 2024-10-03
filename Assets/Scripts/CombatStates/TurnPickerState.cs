using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class TurnPickerState : ICombatState
{
    private readonly CombatSequenceController _controller;
    private TurnController _turnController;
    private bool _isReady;
    private bool _isFriendly;
    private int _characterIndex;

    public TurnPickerState(CombatSequenceController combatSequenceController)
    {
        _controller = combatSequenceController;
    }

    public void Enter()
    {
        Debug.Log("Picking next turn...");
        _turnController = CombatManager.Instance.turnController;
        var nextTurnDetails = _turnController.GetNextTurn();
        _isFriendly = nextTurnDetails.team == 0;
        _characterIndex = nextTurnDetails.characterIndex;
        _isReady = true;
    }

    public void Execute()
    {
        if (_isReady)
        {
            if (_isFriendly)
            {
                _controller.ChangeState(new PlayerTurnState(_controller, _characterIndex));
            }
            else
            {
                _controller.ChangeState(new EnemyTurnState(_controller, _characterIndex));
            }
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting turn picker.");
    }
}
