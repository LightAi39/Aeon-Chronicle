using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TurnOrderEntity : MonoBehaviour
{
    [Header("Turn Stats")]
    public int team = 0;
    public int characterIndex = 0;
    public int startDelay = 0;
    public int delayPerTurn = 60;
    
    [Header("Stats (instantiated by Character SO)")]
    public Character character;
    [Space(20)]
    public string name;
    public int maxHp;
    public int maxSp;
    public int atk;
    public int def;
    public List<Skill> skills = new List<Skill>();
    
    [Header("Stats (live)")]
    [Tooltip("Instantiated based on maxHp")]
    public int currentHp;
    [Tooltip("Instantiated based on maxSp")]
    public int currentSp;
    public int shield;
    public State state = State.Idle;
    
    [Space(20)]
    public TextMeshPro hpTextBox;
    public TextMeshPro turnIndicator;
    public TurnOrderEntity EntityToAttackTemp;
    
    private bool isActiveTurn = false;
    private bool isTargeted = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (character != null)
        {
            name = character.name;
            maxHp = character.maxHp;
            currentHp = character.maxHp;
            maxSp = character.maxSp;
            currentSp = character.maxSp;
            atk = character.atk;
            def = character.def;
            skills = character.skills;
        }
    }
    
    void Update()
    {
        hpTextBox.text = currentHp.ToString();
        
        turnIndicator.color = isActiveTurn ? Color.white : Color.red;
        turnIndicator.enabled = isActiveTurn || isTargeted;
    }

    // all these should be through pub/sub
    public void StartTurn()
    {
        isActiveTurn = true;
    }

    public void GetTargeted()
    {
        isTargeted = true;
    }
    
    public void EndTurn()
    {
        isActiveTurn = false;
        CombatManager.Instance.DoEntityCompletedAction();
    }
    
    public void GetUntargeted()
    {
        isTargeted = false;
    }
    
    public async void Attack(TurnOrderEntity entity)
    {
        entity.GetTargeted();
        await Task.Delay(500);
        entity.TakeDamage(this.atk);
        await Task.Delay(2000);
        entity.GetUntargeted();
        EndTurn(); // temp
    }

    public void TakeDamage(int damage)
    {
        if (state != State.Defending)
        {
            int damageTaken = Convert.ToInt32(Mathf.Clamp(damage - def/2, 0, Mathf.Infinity));
            int trueDamage = damageTaken - shield;
            if (trueDamage > 0)
            {
                currentHp -=  trueDamage;
            }
            shield -= damageTaken;
            if (shield < 0)
            {
                shield = 0;
            }
        }
        else
        {
            int damageTaken = Convert.ToInt32(Mathf.Clamp((damage - def/2)/2, 0, Mathf.Infinity));
            int trueDamage = damageTaken - shield;
            if (trueDamage > 0)
            {
                currentHp -=  trueDamage;
            }
            shield -= damageTaken;
            if (shield < 0)
            {
                shield = 0;
            }
        }
    }

    public void Defend()
    {
        state = State.Defending; //idk fix way to get out tomorrow (like when getting a turn set the state to idle)
    }

    public void GetHealed(int healing)
    {
        currentHp += healing;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

    public void GetShield(int shielding)
    {
        shield = shielding;
    }

    public void UseSkill(Skill skillUsed, TurnOrderEntity? enemy, TurnOrderEntity? actingEntity)
    {
        switch(skillUsed.skilltype)
        {
            case Skill.Skilltype.Damage:
            enemy.TakeDamage(skillUsed.value * atk);
            break;
            case Skill.Skilltype.Defense:
            GetShield(skillUsed.value * def);
            break;
            case Skill.Skilltype.Healing:
            GetHealed(skillUsed.value * atk); //use attack stat for healing for now
            break;
        }
    }

    public void UseItem(TurnOrderEntity targetEntity/*, Item item*/) //hard coded as a healthpotion rn
    {
        GetHealed(10); //item.value or so
    }
}

public enum State {
    Idle,
    Busy,
    Defending
}
