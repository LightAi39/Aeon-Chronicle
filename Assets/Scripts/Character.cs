using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Character : ScriptableObject
{
    public string name;
    public int maxHp;
    public int currentHp;
    public int maxSp;
    public int currentSp;
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
            currentHp -= damage - def/2;
        }
        else
        {
            currentHp -= (damage - def/2)/2;
        }
    }

    public void Defend()
    {
        state = State.Defending; //idk fix way to get out tomorrow
    }

    public void UseSkill(Skill skillUsed)
    {
        switch(skillUsed.skilltype)
        {
            case Skill.Skilltype.Damage:
            break;
            case Skill.Skilltype.Defense:
            break;
            case Skill.Skilltype.Healing:
            break;
        }
    }
}
