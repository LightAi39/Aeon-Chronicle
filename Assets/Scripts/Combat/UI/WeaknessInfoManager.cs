using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class WeaknessInfoManager : MonoBehaviour
{
    
    public GameObject panel;
    public TextMeshProUGUI slashText;
    public TextMeshProUGUI bluntText;
    public TextMeshProUGUI thrustText;
    public TextMeshProUGUI fireText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI earthText;
    public TextMeshProUGUI windText;
    
    [HideInInspector]
    public bool isEnabled = false;

    public void Start()
    {
        CombatManager.Instance.TargetChanged += PreparePanel;
    }

    public void PreparePanel()
    {
        var targetingManager = CombatManager.Instance.targetingManager;
        TurnOrderEntity target = targetingManager.targetingEnemies ? targetingManager.TargetedEnemy : targetingManager.TargetedFriendly;
        if (target.character.damageWeaknesses.Count < 1) return;
        slashText.text = target.character.damageWeaknesses[DamageType.Slash].ToString(CultureInfo.InvariantCulture);
        bluntText.text = target.character.damageWeaknesses[DamageType.Blunt].ToString(CultureInfo.InvariantCulture);
        thrustText.text = target.character.damageWeaknesses[DamageType.Thrust].ToString(CultureInfo.InvariantCulture);
        fireText.text = target.character.damageWeaknesses[DamageType.Fire].ToString(CultureInfo.InvariantCulture);
        waterText.text = target.character.damageWeaknesses[DamageType.Water].ToString(CultureInfo.InvariantCulture);
        earthText.text = target.character.damageWeaknesses[DamageType.Earth].ToString(CultureInfo.InvariantCulture);
        windText.text = target.character.damageWeaknesses[DamageType.Wind].ToString(CultureInfo.InvariantCulture);
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
        isEnabled = true;
    }

    public void HidePanel()
    {
        panel.SetActive(false);
        isEnabled = false;
    }
}
