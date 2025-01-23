using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script, as the name implies, controls which Turn Order Entity may act according to the delay-based queue. At the Awake, this script will find all TurnOrderEntity’s and put them in a list sorted by delay.
/// To get the next turn, you call the GetNextTurn method. If necessary, this involves advancing the delay.
/// When you wish to end a turn, you can call FinishCurrentTurn, which will update the entity’s delay based on its stat, and reorder the list.
/// When a character dies, they are removed with the KillCharacter method.
/// </summary>
public class TurnController : MonoBehaviour
{
    public List<TurnOrderEntry> TurnOrder => _turnOrder;
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
        
        CombatManager.Instance.CombatStateChanged += OnCombatStateChanged;
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

    public TurnOrderEntry GetCurrentlyActing()
    {
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

    public void KillCharacter(TurnOrderEntity entity)
    {
        _turnOrder.Remove(_turnOrder.First(x => x.entity == entity));
    }
    
    // temporary check if the battle is over - if it is, return to the main menu
    private void OnCombatStateChanged()
    {
        // one of the teams is completely defeated
        if (_turnOrder.All(x => x.team != 0) || _turnOrder.All(x => x.team != 1))
        {
            CombatManager.Instance.asyncLoader.LoadScene("Main Menu");
        }
    }
}

public class TurnOrderEntry
{
    public TurnOrderEntity entity;
    public int team => entity.team;
    public int characterIndex => entity.characterIndex;
    public int currentDelay;
}
