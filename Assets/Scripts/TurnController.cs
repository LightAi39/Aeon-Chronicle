using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    private List<TurnOrderEntry> _turnOrder = new();
    void Awake()
    {
        // create the turn order list
        var allEntities = FindObjectsOfType<TurnOrderEntity>();
        foreach (var entity in allEntities)
        {
            _turnOrder.Add(new TurnOrderEntry()
            {
                entity = entity,
                currentDelay = entity.startDelay,
            });
        }

        SortList();
    }

    // gets the next turn (and advances time to make the next turn 0 delay)
    public TurnOrderEntry GetNextTurn()
    {
        if (_turnOrder[0].currentDelay > 0)
        {
            int delayToRemove = _turnOrder[0].currentDelay;
            foreach (var turn in _turnOrder)
            {
                turn.currentDelay -= delayToRemove;
            }
        }

        return _turnOrder[0];
    }

    public void FinishCurrentTurn()
    {
        Debug.Log("Forwarding turn");
        int delayAmount = _turnOrder[0].entity.delayPerTurn;

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
    public TurnOrderEntity entity;
    public int team => entity.team;
    public int characterIndex => entity.characterIndex;
    public int currentDelay;
}
