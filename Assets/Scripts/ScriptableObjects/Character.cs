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
    public DamageType damageType;
    public Element element;
    public List<Skill> skills = new List<Skill>();
        public enum DamageType
    {
        Slash,
        Thrust,
        Blunt,
        Magical
    }
    public enum Element
    { 
        Fire,
        Water,
        Earth,
        Wind,
        Neutral
    } 
}
