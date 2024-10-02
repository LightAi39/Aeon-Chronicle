using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Enemy : ScriptableObject
{
    public int maxHp;
    public int currentHp;
    public int atk;
    public int def;

    public IEnumerator Attack(Character character)
    {
        character.TakeDamage(this.atk);
        yield return new WaitForSeconds(2f);
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage - def/2;
    }
}
