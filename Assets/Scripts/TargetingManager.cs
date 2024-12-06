using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TargetingManager : MonoBehaviour
{
    [HideInInspector]
    public List<TurnOrderEntity> Friendlies = new();
    [HideInInspector]
    public List<TurnOrderEntity> Enemies = new();
    
    public TurnOrderEntity TargetedEnemy => Enemies[targetedEnemyIndex];
    public TurnOrderEntity TargetedFriendly => Friendlies[targetedFriendlyIndex];
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
    
    // TODO: fuck this temp shit
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

            teammate.UseSkill(teammate.character.weapon.basicAttack, TargetedEnemy);
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
                teammate.UseSkill(teammate.skills[0], TargetedEnemy);
            }
            else
            {
                teammate.UseSkill(teammate.skills[0], TargetedFriendly);
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
