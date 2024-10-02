using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    private List<TurnOrderEntry> _turnOrder = new();
    void Start()
    {
        // create the turn order list
        _turnOrder.Add(new TurnOrderEntry()
        {
            team = 0,
            characterIndex = 0,
            currentDelay = 0
        });
        _turnOrder.Add(new TurnOrderEntry()
        {
            team = 0,
            characterIndex = 1,
            currentDelay = 10
        });
        _turnOrder.Add(new TurnOrderEntry()
        {
            team = 0,
            characterIndex = 2,
            currentDelay = 20
        });
        _turnOrder.Add(new TurnOrderEntry()
        {
            team = 0,
            characterIndex = 3,
            currentDelay = 50
        });
        _turnOrder.Add(new TurnOrderEntry()
        {
            team = 1,
            characterIndex = 0,
            currentDelay = 30
        });
        _turnOrder.Add(new TurnOrderEntry()
        {
            team = 1,
            characterIndex = 2,
            currentDelay = 40
        });
        _turnOrder.Add(new TurnOrderEntry()
        {
            team = 1,
            characterIndex = 3,
            currentDelay = 60
        });
    }

    // gets the next turn (and advances time to make the next turn 0 delay)
    public (int team, int characterIndex) GetNextTurn()
    {
        if (_turnOrder[0].currentDelay > 0)
        {
            int delayToRemove = _turnOrder[0].currentDelay;
            foreach (var turn in _turnOrder)
            {
                turn.currentDelay -= delayToRemove;
            }
        }
        
        return (_turnOrder[0].team, _turnOrder[0].characterIndex);
    }

    public void FinishCurrentTurn()
    {
        Debug.Log("Forwarding turn");
        //TODO: make delay amount not hardcoded
        int delayAmount = 60;

        _turnOrder[0].currentDelay = delayAmount;
        SortList();
    }

    private void SortList()
    {
        _turnOrder = _turnOrder.OrderBy(t => t.currentDelay).ToList();
    }
}

public class TurnOrderEntry
{
    public int team;
    public int characterIndex;
    public int currentDelay;
}
