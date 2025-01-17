using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

/*public class CharacterTests
{
    [Test]
    public void TurnOrderEntityInstantiates()
    {
        // Arrange
        List<Skill> skills = new List<Skill>();
        Skill skill = new Skill();
            skill.name = "Fire Slash";
            skill.skilltype = Skill.Skilltype.PDamage;
            skill.value = 2;
            skill.spCost = 3;
            skill.powerModifier = 1;
            skill.damageType = Character.DamageType.Slash;
            skill.element = Character.Element.Fire;
            skills.Add(skill);

        Character character = new Character();
            character.name = "Swordfighter";
            character.maxHp = 20;
            character.maxSp = 12;
            character.strength = 7;
            character.resilience = 4;
            character.intelligence = 1;
            character.mind = 2;
            character.agility = 7;
            character.critChance = 25;
            character.critDamage = 50;
            character.skills = skills;
            character.damageType = Character.DamageType.Slash;
            character.element = Character.Element.Neutral;

        GameObject toeGameObject = new GameObject();
        TurnOrderEntity toEntity = toeGameObject.AddComponent<TurnOrderEntity>();
        toEntity.character = character;

        // Act
        toEntity.Initialize();

        // Assert
        Assert.AreEqual(toEntity.maxHp, 20);
    }
    
    [Test]
    public void TurnOrderEntityTakesDamage()
    {
        // Arrange
        List<Skill> skills = new List<Skill>();
        Skill skill = new Skill();
            skill.name = "Fire Slash";
            skill.skilltype = Skill.Skilltype.PDamage;
            skill.value = 2;
            skill.spCost = 3;
            skill.powerModifier = 1;
            skill.damageType = Character.DamageType.Slash;
            skill.element = Character.Element.Fire;
            skills.Add(skill);

        Character character = new Character();
            character.name = "Swordfighter";
            character.maxHp = 20;
            character.maxSp = 12;
            character.strength = 7;
            character.resilience = 4;
            character.intelligence = 1;
            character.mind = 2;
            character.agility = 7;
            character.critChance = 25;
            character.critDamage = 50;
            character.skills = skills;
            character.damageType = Character.DamageType.Slash;
            character.element = Character.Element.Neutral;

        GameObject toeGameObject = new GameObject();
        TurnOrderEntity toEntity = toeGameObject.AddComponent<TurnOrderEntity>();
        toEntity.character = character;

        // Act
        toEntity.Initialize();
        toEntity.TakeDamage(6, Character.DamageType.Slash, Character.Element.Earth, 1);

        // Assert
        Assert.IsTrue(toEntity.currentHp < toEntity.maxHp);
    }

    [Test]
    public void TurnOrderEntityHpNotUnder0()
    {
        // Arrange
        List<Skill> skills = new List<Skill>();
        Skill skill = new Skill();
            skill.name = "Fire Slash";
            skill.skilltype = Skill.Skilltype.PDamage;
            skill.value = 2;
            skill.spCost = 3;
            skill.powerModifier = 1;
            skill.damageType = Character.DamageType.Slash;
            skill.element = Character.Element.Fire;
            skills.Add(skill);

        Character character = new Character();
            character.name = "Swordfighter";
            character.maxHp = 20;
            character.maxSp = 12;
            character.strength = 7;
            character.resilience = 4;
            character.intelligence = 1;
            character.mind = 2;
            character.agility = 7;
            character.critChance = 25;
            character.critDamage = 50;
            character.skills = skills;
            character.damageType = Character.DamageType.Slash;
            character.element = Character.Element.Neutral;

        GameObject toeGameObject = new GameObject();
        TurnOrderEntity toEntity = toeGameObject.AddComponent<TurnOrderEntity>();
        toEntity.character = character;

        // Act
        toEntity.Initialize();
        toEntity.TakeDamage(500, Character.DamageType.Slash, Character.Element.Earth, 50);

        // Assert
        Assert.IsTrue(toEntity.currentHp >= 0);
    }
}*/
