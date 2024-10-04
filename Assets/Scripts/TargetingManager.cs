using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetingManager : MonoBehaviour
{
    [HideInInspector]
    public List<TurnOrderEntity> Friendlies = new();
    [HideInInspector]
    public List<TurnOrderEntity> Enemies = new();
    
    public TurnOrderEntity TargetedEnemy => Enemies[targetedEnemyIndex];
    [HideInInspector]
    public bool IsActivelyTargeting = false;
    
    private int targetedEnemyIndex = 0;
    
    void Awake()
    {
        var allEntities = FindObjectsOfType<TurnOrderEntity>();
        Friendlies.AddRange(allEntities.Where(x => x.team == 0));
        Enemies.AddRange(allEntities.Where(x => x.team == 1));
    }
    
    void Update()
    {
        
    }
    
    // TODO: fuck this temp shit
    public void ChangeTargetLeft()
    {
        targetedEnemyIndex += 1;
        if (targetedEnemyIndex > 2)
        {
            targetedEnemyIndex = 0;
        }

        CombatManager.Instance.DoTargetChanged();
    }

    public void ChangeTargetRight()
    {
        targetedEnemyIndex -= 1;
        if (targetedEnemyIndex < 0)
        {
            targetedEnemyIndex = 2;
        }
        
        CombatManager.Instance.DoTargetChanged();
    }

    public void AttackWithCurrentTeammate()
    {
        if (IsActivelyTargeting)
        {
            var teammate = CombatManager.Instance.turnController.GetNextTurn().entity;

            teammate.Attack(TargetedEnemy);
        }
        
    }

    public void ActivateTargeting()
    {
        IsActivelyTargeting = true;
        CombatManager.Instance.DoTargetChanged();
    }

    public void DeactivateTargeting()
    {
        IsActivelyTargeting = false;
        CombatManager.Instance.DoTargetChanged();
    }

    public void DefendWithCurrentTeammate()
    {
        if (IsActivelyTargeting)
        {
            var teammate = CombatManager.Instance.turnController.GetNextTurn().entity;

            teammate.Defend();
        }
    }
}
