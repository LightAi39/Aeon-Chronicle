using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Skill : ScriptableObject
{
    public string name;
    public Skilltype skilltype;
    public string description;
    public int value; //multiplier for stat for now
    public int spCost;
    public float powerModifier;
    public Character.DamageType damageType;
    public Character.Element element;
    public enum Skilltype {
        PDamage,
        MDamage,
        Defense,
        Healing
    }
}
