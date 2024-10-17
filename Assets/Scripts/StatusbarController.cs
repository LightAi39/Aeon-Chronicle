using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusbarController : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public Slider healthbarForeground;
    public Slider healthbarBackground;
    public Slider shieldbarForeground;
    public Slider shieldbarBackground;
    public TurnOrderEntity turnOrderEntity;
    public float transition = 2f; //time bar takes to move

    // Start is called before the first frame update
    void Awake()
    {
        if (turnOrderEntity != null)
        {
            healthbarForeground.maxValue = turnOrderEntity.maxHp;
            healthbarBackground.maxValue = turnOrderEntity.maxHp;
            healthbarForeground.value = turnOrderEntity.currentHp;
            healthbarBackground.value = turnOrderEntity.currentHp;
            //you can have a max amount of shield maybe? at least this makes the bar fill up the same amount of relatively to hp
            shieldbarForeground.maxValue = turnOrderEntity.maxHp;
            shieldbarBackground.maxValue = turnOrderEntity.maxHp;
            shieldbarForeground.value = turnOrderEntity.shield;
            shieldbarBackground.value = turnOrderEntity.shield;
        }
    }
    public void UpdateStatusbar(int damageTaken)
    {
        healthbarForeground.value = turnOrderEntity.currentHp;
        shieldbarForeground.value = turnOrderEntity.shield;

        GameObject DamageTextInstance = Instantiate(damageTextPrefab, turnOrderEntity.transform);
        DamageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().SetText($"{damageTaken}");

        StartCoroutine(DelayedSlider());
    }

    private IEnumerator DelayedSlider()
    {
        float startValue1 = healthbarBackground.value;
        float startValue2 = shieldbarBackground.value;
        float elapsedTime = 0f;

        while (elapsedTime < transition)
        {
            elapsedTime += Time.deltaTime;

            healthbarBackground.value = Mathf.Lerp(startValue1, turnOrderEntity.currentHp, elapsedTime/transition);
            shieldbarBackground.value = Mathf.Lerp(startValue2, turnOrderEntity.shield, elapsedTime/transition);
            yield return null;
        }

        healthbarBackground.value = turnOrderEntity.currentHp;
        shieldbarBackground.value = turnOrderEntity.shield;
    }
}
