using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public Character character;
    public Healer()
    {
        character.name = "Cleric";
        Skill skill = new Skill("Heal", "Heals an ally.", Skill.Skilltype.Healing, 2, 6);
        character.skills.Add(skill);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
