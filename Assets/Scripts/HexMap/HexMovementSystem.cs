using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GraphSearch;

/// <summary>
/// This is the movement system for the Unit on the Hex map. It manages the visuals for range, interacting with the range search algorithm, as well as showing the path and moving the character on it.
/// This uses a BFS algorithm you can find in GraphSearch.cs. This is how range is determined based on the movement budget and tile costs.
/// </summary>
public class HexMovementSystem : MonoBehaviour
{
    private BfsResult movementRange = new();
    private List<Vector3Int> currentPath = new();

    public void HideRange(HexGrid hexGrid)
    {
        foreach (Vector3Int hexPosition in movementRange.GetHexPositionsInRange())
        {
            hexGrid.GetTileAt(hexPosition).DisableHighlight();
        }
        movementRange = new BfsResult();
    }

    public void ShowRange(Unit selectedUnit, HexGrid hexGrid)
    {
        CalculateRange(selectedUnit, hexGrid);

        //Vector3Int unitPos = hexGrid.GetClosestHex(selectedUnit.transform.position);  --- enable this to not highlight the unit position

        foreach (Vector3Int hexPosition in movementRange.GetHexPositionsInRange())
        {
            //if (unitPos == hexPosition) continue;  --- enable this to not highlight the unit position
            hexGrid.GetTileAt(hexPosition).EnableHighlight();
        }
    }

    public void CalculateRange(Unit selectedUnit, HexGrid hexGrid)
    {
        movementRange = GraphSearch.BfsGetRange(hexGrid, hexGrid.GetClosestHex(selectedUnit.transform.position), selectedUnit.MovementPoints);
    }


    public void ShowPath(Vector3Int selectedHexPosition, HexGrid hexGrid)
    {
        if (movementRange.GetHexPositionsInRange().Contains(selectedHexPosition))
        {
            hexGrid.GetTileAt(movementRange.StartPoint).ResetHighlight(); // also reset current position
            foreach (Vector3Int hexPosition in currentPath)
            {
                hexGrid.GetTileAt(hexPosition).ResetHighlight();
            }
            currentPath = movementRange.GetPath(selectedHexPosition);
            hexGrid.GetTileAt(movementRange.StartPoint).HighlightPath(); // also highlight current position
            foreach (Vector3Int hexPosition in currentPath)
            {
                
                hexGrid.GetTileAt(hexPosition).HighlightPath();
            }
        }
    }

    public void MoveUnit(Unit selectedUnit, HexGrid hexGrid)
    {
        Debug.Log("Moving unit " + selectedUnit.name);
        selectedUnit.MoveThroughPath(currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList());

    }

    public bool IsHexInRange(Vector3Int hexPosition)
    {
        return movementRange.IsHexPositionInRange(hexPosition);
    }
}
