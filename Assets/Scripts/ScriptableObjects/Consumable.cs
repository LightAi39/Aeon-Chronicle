using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Consumable : Item
{
    public ConsumableEffect effect;
    public bool aoe;
    public bool useOnFriendlies;
    public int effectStrength;
    public int amountOfUses;
}
