using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class Equipment : Item
{
    public EquipmentType equipmentType;
    public List<StatValuePair>? stats = new List<StatValuePair>();
    public Skill? basicAttack;
    public List<Passives>? passives = new List<Passives>();
}
