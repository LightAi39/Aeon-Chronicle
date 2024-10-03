using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu]
public class Character : ScriptableObject
{
    public string name;
    public int maxHp;
    public int currentHp;
    public int maxSp;
    public int currentSp;
    public int shield;
    public int atk;
    public int def;
    public State state = State.Idle; 
    public enum State {
        Idle,
        Busy,
        Defending
    }    
    public List<Skill> skills = new List<Skill>();
    public IEnumerator Attack(Enemy enemy)
    {
        enemy.TakeDamage(this.atk);
        yield return new WaitForSeconds(2f);
    }

    public void TakeDamage(int damage)
    {
        if (state != State.Defending)
        {
            var damageTaken = damage - def/2;
            var trueDamage = damageTaken - shield;
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
            var damageTaken = (damage - def/2)/2;
            var trueDamage = damageTaken - shield;
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

    public void UseSkill(Skill skillUsed, Enemy? enemy, Character? character)
    {
        switch(skillUsed.skilltype)
        {
            case Skill.Skilltype.Damage:
            enemy.TakeDamage(skillUsed.value * atk);
            break;
            case Skill.Skilltype.Defense:
            character.GetShield(skillUsed.value * def);
            break;
            case Skill.Skilltype.Healing:
            character.GetHealed(skillUsed.value * atk); //use attack stat for healing for now
            break;
        }
    }

    public void UseItem(Character character/*, Item item*/) //hard coded as a healthpotion rn
    {
        character.GetHealed(10); //item.value or so
    }
}
