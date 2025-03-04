using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// This manager facilitates the targeting mechanic, including methods for triggering actions.
/// This lets the player cycle between targets with a left and right input, and perform actions on the target, which will call logic inside the target TurnOrderEntity.
/// </summary>
public class TargetingManager : MonoBehaviour
{
    [HideInInspector]
    public List<TurnOrderEntity> Friendlies = new();
    [HideInInspector]
    public List<TurnOrderEntity> Enemies = new();

    public TurnOrderEntity TargetedEnemy => GetEnemy();
    public TurnOrderEntity TargetedFriendly => GetFriendly();
    [HideInInspector]
    public bool IsActivelyTargeting = false;
    
    private int targetedEnemyIndex = 0;
    private int targetedFriendlyIndex = 0;
    public bool targetingEnemies = true;
    
    void Awake()
    {
        var allEntities = FindObjectsOfType<TurnOrderEntity>();
        Friendlies.AddRange(allEntities.Where(x => x.team == 0));
        Enemies.AddRange(allEntities.Where(x => x.team == 1));
    }
    
    void Update()
    {
        
    }

    private TurnOrderEntity GetEnemy()
    {
        try
        {
            return Enemies[targetedEnemyIndex];
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    
    private TurnOrderEntity GetFriendly()
    {
        try
        {
            return Friendlies[targetedFriendlyIndex];
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    
    // this could be done with a better approach, but this works
    public void ChangeTargetLeft()
    {
        if (targetingEnemies)
        {
            targetedEnemyIndex += 1;
            if (targetedEnemyIndex > Enemies.Count - 1)
            {
                targetedEnemyIndex = 0;
            }
        }
        else
        {
            targetedFriendlyIndex += 1;
            if (targetedFriendlyIndex > Friendlies.Count - 1)
            {
                targetedFriendlyIndex = 0;
            }
        }
        

        CombatManager.Instance.DoTargetChanged();
    }
    
    public void ChangeTargetRight()
    {
        if (targetingEnemies)
        {
            targetedEnemyIndex -= 1;
            if (targetedEnemyIndex < 0)
            {
                targetedEnemyIndex = Enemies.Count - 1;
            }
        }
        else
        {
            targetedFriendlyIndex -= 1;
            if (targetedFriendlyIndex < 0)
            {
                targetedFriendlyIndex = Friendlies.Count - 1;
            }
        }
        

        CombatManager.Instance.DoTargetChanged();
    }

    public void AttackWithCurrentTeammate()
    {
        if (IsActivelyTargeting)
        {
            var teammate = CombatManager.Instance.turnController.GetNextTurn().entity;

            teammate.UseSkill(teammate.characterScriptableObject.equipment[0].basicAttack, TargetedEnemy);
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

    public void UseSkillWithCurrentTeammate()
    {
        if (IsActivelyTargeting)
        {
            var teammate = CombatManager.Instance.turnController.GetNextTurn().entity;

            if (targetingEnemies)
            {
                teammate.UseSkill(teammate.character.skills[0], TargetedEnemy);
            }
            else
            {
                teammate.UseSkill(teammate.character.skills[0], TargetedFriendly);
            }

            targetingEnemies = true;
            CombatManager.Instance.DoTargetChanged();
        }
    }

    public void ToggleTargetingFriendlies()
    {
        targetingEnemies = !targetingEnemies;
        CombatManager.Instance.DoTargetChanged();
    }
    

    public void RemoveEnemy(TurnOrderEntity enemy)
    {
        Enemies.Remove(enemy);
        if (targetedEnemyIndex > Enemies.Count - 1)
        {
            targetedEnemyIndex = Enemies.Count - 1;
        }

        CombatManager.Instance.turnController.KillCharacter(enemy);
        CombatManager.Instance.DoTargetChanged();
    }
    
    public void RemoveFriendly(TurnOrderEntity friendly)
    {
        Friendlies.Remove(friendly);
        if (targetedFriendlyIndex > Friendlies.Count - 1)
        {
            targetedFriendlyIndex = Friendlies.Count - 1;
        }
        CombatManager.Instance.turnController.KillCharacter(friendly);
        CombatManager.Instance.DoTargetChanged();
    }
}
