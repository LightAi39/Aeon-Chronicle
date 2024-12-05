using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GraphSearch;

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
            foreach (Vector3Int hexPosition in currentPath)
            {
                hexGrid.GetTileAt(hexPosition).ResetHighlight();
            }
            currentPath = movementRange.GetPath(selectedHexPosition);
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
