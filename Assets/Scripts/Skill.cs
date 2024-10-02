using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string name;
    public Skilltype skilltype;
    public string description;
    public int value;
    public int spCost;
    public enum Skilltype {
        Damage,
        Defense,
        Healing
    }
    public Skill(string name, string description, Skilltype skilltype, int value, int spCost)
    {
        this.name = name;
        this.description = description;
        this.skilltype = skilltype;
        this.value = value;
        this.spCost = spCost;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
