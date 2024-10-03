using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dps : MonoBehaviour
{
    public Character character;

    public Dps()
    {
        character.name = "Swordfighter";
        Skill skill = new Skill("Slash", "Slashes the enemy", Skill.Skilltype.Damage, 2, 5);
        character.skills.Add(skill);
    }

    void Start()
    {
        
    }

    void Update()
    {

    }
}
