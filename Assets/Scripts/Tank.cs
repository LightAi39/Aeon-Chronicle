using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public Character character;
    public Tank()
    {
        character.name = "Paladin";
        Skill skill = new Skill("Block", "Blocks enemy attacks.", Skill.Skilltype.Defense, 3, 8);
        character.skills.Add(skill);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
