using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

/// <summary>
/// This is where all UI logic is done, in order to consolidate it. There is nothing special about this particular manager, as it is not at all necessary to expand UI with this manager. You can look at the inline documentation for more information.
/// </summary>
public class UiManager : MonoBehaviour
{
    public WeaknessInfoManager weaknessInfoManager;
    public GameObject skillPanel;
    public GameObject itemPanel;
    public List<GameObject> orderPanelEntries = new();
    
    public GameObject objectForTurnOrder;
    public Transform uiParentTurnOrder;
    public GameObject itemButtonPrefab;

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
            
            foreach (Transform child in itemPanel.transform)
            {
                Destroy(child.gameObject);
            }

            var turnOrderEntity = CombatManager.Instance.turnController.GetCurrentlyActing().entity;

            foreach (var consumable in turnOrderEntity.character.consumables)
            {
                if (consumable.amountOfUses <= 0) continue;
                GameObject newButton = Instantiate(itemButtonPrefab, itemPanel.transform);
                TextMeshProUGUI textComponent = newButton.GetComponentInChildren<TextMeshProUGUI>();
                Button button = newButton.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => turnOrderEntity.UseItem(consumable, null));
                if (textComponent != null)
                {
                    textComponent.text = $"{consumable.itemName}: {consumable.amountOfUses}";
                }
                else
                {
                    Debug.LogWarning("The button prefab is missing a TextMeshProUGUI component.");
                }

            }
            
            itemPanel.SetActive(!itemPanel.activeSelf);
        }
    }

    public void ToggleTargetInfoPanel()
    {
        if (!weaknessInfoManager.isEnabled)
        { 
            weaknessInfoManager.ShowPanel();
        }
        else
        {
            weaknessInfoManager.HidePanel();
        }
        
    }

    public void Start()
    {
        var turnOrder = CombatManager.Instance.turnController.TurnOrder;

        List<TurnOrderEntity> entities = turnOrder.Select(x => x.entity).ToList();

        foreach (var entity in entities)
        {
            GameObject newObject = Instantiate(objectForTurnOrder, uiParentTurnOrder);
            newObject.name = entity.character.name + $" {entity.team}-{entity.characterIndex}";
            TextMeshProUGUI tmpText = newObject.GetComponentInChildren<TextMeshProUGUI>();
            tmpText.text = entity.character.name;
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
            tmpText.text = entity.name;
            Image img = newObject.GetComponentInChildren<Image>();
            img.color = entity.color;
            orderPanelEntries.Add(newObject);
        }
        
        // TODO: this is temp
        // this is to update the skill/item info
        skillPanel.SetActive(false);
        itemPanel.SetActive(false);

        var buttonText = skillPanel.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = CombatManager.Instance.turnController.GetNextTurn().entity.character.skills[0].name;
    }
}
