using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public LayerMask selectionMask;
    public HexGrid hexGrid;

    private List<Vector3Int> _neighbours = new();
    
    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    public void HandleTileClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        GameObject result;
        
        if (!FindTarget(out result)) return;
        HexTile selectedHex = result.GetComponent<HexTile>();
        
        selectedHex.DisableHighlight();
        foreach (var neighbour in _neighbours)
        {
            hexGrid.GetTileAt(neighbour).DisableHighlight();
        }
        
        //_neighbours = hexGrid.GetNeighboursFor(selectedHex.HexCoords); to get all neighbour tiles
        GraphSearch.BfsResult bfsResult = GraphSearch.BfsGetRange(hexGrid, selectedHex.HexCoords, 20);
        _neighbours = new List<Vector3Int>(bfsResult.GetHexPositionsInRange());
        
        foreach (var neighbour in _neighbours)
        {
            hexGrid.GetTileAt(neighbour).EnableHighlight();
        }
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
