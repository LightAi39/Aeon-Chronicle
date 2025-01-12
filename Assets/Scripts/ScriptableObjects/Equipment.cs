using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Equipment : Item
{
    public EquipmentType equipmentType;
    public List<KeyValuePair<Stats, int>> stats = new List<KeyValuePair<Stats, int>>();
    public List<Passives> passives = new List<Passives>();
}
