using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class ActionState : ICombatState
{
    private readonly CombatSequenceController _controller;
    private TurnController _turnController;

    public ActionState(CombatSequenceController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        _turnController = CombatManager.Instance.turnController;
        Debug.Log("Performing action...");
        // TODO: add action initialization logic here
    }

    public void Execute()
    {
        if (_controller.IsActionComplete())
        {
            _turnController.FinishCurrentTurn(); // finishes the current turn, for now this is fine.
            _controller.ChangeState(new TurnPickerState(_controller));
        }
    }

    public void Exit()
    {
        Debug.Log("Action Complete");
    }
}
