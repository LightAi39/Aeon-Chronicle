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
    public float transition = 1f; //time bar takes to move
    public float transitionForeground = 0.005f;

    // Start is called before the first frame update
    void Start()
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
        //healthbarForeground.value = turnOrderEntity.currentHp;
        //shieldbarForeground.value = turnOrderEntity.shield;
        
        GameObject DamageTextInstance = Instantiate(damageTextPrefab, turnOrderEntity.transform);
        DamageTextInstance.transform.localPosition = new Vector3(14.18f, 6.38f, 4.2f);
        DamageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().SetText($"{damageTaken}");
        if (turnOrderEntity.team != 0)
        {
            DamageTextInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        StartCoroutine(DelayedSlider());
    }

    private IEnumerator DelayedSlider()
    {
        float hpFgStartValue = healthbarForeground.value;
        float shieldFgStartValue = shieldbarForeground.value;
        float hpBgStartValue = healthbarBackground.value;
        float shieldBgStartValue = shieldbarBackground.value;
        float elapsedTime = 0f;

        while (elapsedTime < transition)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime < transitionForeground)
            {
                float tshort = Mathf.Clamp(elapsedTime / transitionForeground, 0, 1);
                
                healthbarForeground.value = Mathf.Lerp(hpFgStartValue, turnOrderEntity.currentHp, tshort);
                shieldbarForeground.value = Mathf.Lerp(shieldFgStartValue, turnOrderEntity.shield, tshort);
            }
            
            float tlong = Mathf.Clamp(elapsedTime / transition, 0, 1);
            healthbarBackground.value = Mathf.Lerp(hpBgStartValue, turnOrderEntity.currentHp, tlong);
            shieldbarBackground.value = Mathf.Lerp(shieldBgStartValue, turnOrderEntity.shield, tlong);
            yield return null;
        }
        
        healthbarForeground.value = turnOrderEntity.currentHp;
        shieldbarForeground.value = turnOrderEntity.shield;
        healthbarBackground.value = turnOrderEntity.currentHp;
        shieldbarBackground.value = turnOrderEntity.shield;
    }
}
