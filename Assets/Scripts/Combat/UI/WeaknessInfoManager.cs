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

    public void PreparePanel(TurnOrderEntity target)
    {
        slashText.text = target.characterScriptableObject.damageWeaknesses[DamageType.Slash].ToString(CultureInfo.InvariantCulture);
        bluntText.text = target.characterScriptableObject.damageWeaknesses[DamageType.Blunt].ToString(CultureInfo.InvariantCulture);
        thrustText.text = target.characterScriptableObject.damageWeaknesses[DamageType.Thrust].ToString(CultureInfo.InvariantCulture);
        fireText.text = target.characterScriptableObject.damageWeaknesses[DamageType.Fire].ToString(CultureInfo.InvariantCulture);
        waterText.text = target.characterScriptableObject.damageWeaknesses[DamageType.Water].ToString(CultureInfo.InvariantCulture);
        earthText.text = target.characterScriptableObject.damageWeaknesses[DamageType.Earth].ToString(CultureInfo.InvariantCulture);
        windText.text = target.characterScriptableObject.damageWeaknesses[DamageType.Wind].ToString(CultureInfo.InvariantCulture);
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
