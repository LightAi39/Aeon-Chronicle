using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject skillPanel;
    public GameObject itemPanel;
    public List<GameObject> orderPanelEntries = new();

    public GameObject objectForTurnOrder;
    public Transform uiParentTurnOrder;

    public void ShowSkillPanel()
    {
        if (CombatManager.Instance.targetingManager.IsActivelyTargeting)
        {
            skillPanel.SetActive(!skillPanel.activeSelf);
            itemPanel.SetActive(false);
        }
        
    }

    public void ShowItemPanel()
    {
        if (CombatManager.Instance.targetingManager.IsActivelyTargeting)
        {
            skillPanel.SetActive(false);
            itemPanel.SetActive(!itemPanel.activeSelf);
        }
    }

    public void Start()
    {
        var turnOrder = CombatManager.Instance.turnController.TurnOrder;

        List<TurnOrderEntity> entities = turnOrder.Select(x => x.entity).ToList();

        foreach (var entity in entities)
        {
            GameObject newObject = Instantiate(objectForTurnOrder, uiParentTurnOrder);
            newObject.name = entity.name + $" {entity.team}-{entity.characterIndex}";
            TextMeshProUGUI tmpText = newObject.GetComponentInChildren<TextMeshProUGUI>();
            tmpText.text = entity.name + $" {entity.characterIndex}";
            Image img = newObject.GetComponentInChildren<Image>();
            img.color = entity.team == 0 ? Color.cyan : Color.red;
            orderPanelEntries.Add(newObject);
        }

        CombatManager.Instance.CombatStateChanged += UpdateTurnOrder;
    }

    private void Update()
    {
        
    }

    private void UpdateTurnOrder()
    {
        // TODO: fix quick and bad thing
        foreach (var entry in orderPanelEntries)
        {
            Destroy(entry);
        }

        orderPanelEntries = new();
        
        var turnOrder = CombatManager.Instance.turnController.TurnOrder;

        List<TurnOrderEntity> entities = turnOrder.Select(x => x.entity).ToList();

        foreach (var entity in entities)
        {
            GameObject newObject = Instantiate(objectForTurnOrder, uiParentTurnOrder);
            newObject.name = entity.name + $" {entity.team}-{entity.characterIndex}";
            TextMeshProUGUI tmpText = newObject.GetComponentInChildren<TextMeshProUGUI>();
            tmpText.text = entity.name + $" {entity.characterIndex}";
            Image img = newObject.GetComponentInChildren<Image>();
            img.color = entity.team == 0 ? Color.cyan : Color.red;
            orderPanelEntries.Add(newObject);
        }
        
        // TODO: this is temp
        // this is to update the skill/item info
        skillPanel.SetActive(false);
        itemPanel.SetActive(false);

        var buttonText = skillPanel.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = CombatManager.Instance.turnController.GetNextTurn().entity.skills[0].name;
    }
}
