using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu]
public class Character : ScriptableObject
{
    public string name;
    public int maxHp;
    public int maxSp;
    public int strength;
    public int resilience;
    public int intelligence;
    public int mind;
    public int agility;
    public int critChance;
    public int critDamage;
    public Weapon weapon;
    public Headpiece headpiece;
    public Chestpiece chestpiece;
    public Gloves gloves;
    public Legs legs;
    public Boots boots;
    public Accessory accessory;
    public List<DamageType> damageTypes = new List<DamageType>(); //temporary basic attack type/element tied to character instead of weapon
    public Dictionary<DamageType, float> damageWeaknesses = new Dictionary<DamageType, float>(); // Dictionary for attack weaknesses 100 being neutral damage, 0 being immune and 200 being 2x damage taken
    public List<Skill> skills = new List<Skill>();
    public List<DamageType> damageTypeWeaknesses = new List<DamageType> 
    {
        DamageType.Slash,
        DamageType.Thrust,
        DamageType.Blunt,
        DamageType.Magical,
        DamageType.Fire,
        DamageType.Water,
        DamageType.Earth,
        DamageType.Wind,
        DamageType.Neutral
    };
    public List<float> damageTypeValues = new List<float>
    {
        100f,
        100f,
        100f,
        100f,
        100f,
        100f,
        100f,
        100f,
        100f
    }; 
}
