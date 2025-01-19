using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// This class handles the logic for selecting tiles and units with the cursor from the camera view.
/// This is where you can expand mouse input actions to make other clickable things.
/// </summary>
public class SelectionSystem : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public LayerMask selectionMask;

    public UnityEvent<GameObject> OnUnitSelected;
    public UnityEvent<GameObject> OnTerrainSelected;
    
    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    public void HandleTileClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        GameObject result;
        
        if (!FindTarget(out result)) return;

        if (IsUnitSelectedTarget(result))
        {
            OnUnitSelected?.Invoke(result);
        }
        else
        {
            OnTerrainSelected.Invoke(result);
        }
    }
    
    private bool IsUnitSelectedTarget(GameObject result)
    {
        return result.GetComponent<Unit>() != null;
    }

    private bool FindTarget(out GameObject result)
    {
        Vector2 screenPosition = Mouse.current.position.ReadValue();

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out hit, 100, selectionMask))
        {
            result = hit.collider.gameObject;
            return true;
        }
        result = null;
        return false;
    }

}
