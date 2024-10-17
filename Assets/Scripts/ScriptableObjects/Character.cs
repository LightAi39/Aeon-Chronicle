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
    public int atk;
    public int def;
    public List<Skill> skills = new List<Skill>();
}
