using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the top-level hex grid manager. It gathers a list of all tiles at Start and stores it.
/// It also manages getting tiles from both world space and tile coordinates, as well as all neighbouring tiles of a tile coordinate.
/// When loading tiles after loading them dynamically in the future, you can simply trigger the Start logic to initialize everything correctly.
/// </summary>
public class HexGrid : MonoBehaviour
{
    private Dictionary<Vector3Int, HexTile> _hexTileDict = new();
    private Dictionary<Vector3Int, List<Vector3Int>> _hexTileNeighboursDict = new();

    private void Start()
    {
        foreach (var hex in FindObjectsOfType<HexTile>())
        {
            _hexTileDict[hex.HexCoords] = hex;
        }
        
    }

    public HexTile GetTileAt(Vector3Int hexCoordinates)
    {
        HexTile result = null;
        _hexTileDict.TryGetValue(hexCoordinates, out result);
        return result;
    }

    public List<Vector3Int> GetNeighboursFor(Vector3Int hexCoordinates)
    {
        if (_hexTileDict.ContainsKey(hexCoordinates) == false)
        {
            return new List<Vector3Int>();
        }

        if (_hexTileNeighboursDict.TryGetValue(hexCoordinates, out var directions))
        {
            return directions;
        }
        
        _hexTileNeighboursDict.Add(hexCoordinates, new List<Vector3Int>());

        foreach (var direction in Direction.GetDirectionList(hexCoordinates.z))
        {
            var neighbour = hexCoordinates + direction;
            if (_hexTileDict.ContainsKey(neighbour))
            {
                _hexTileNeighboursDict[hexCoordinates].Add(neighbour);
            }
        }

        return _hexTileNeighboursDict[hexCoordinates];
    }

    public Vector3Int GetClosestHex(Vector3 worldPosition)
    {
        worldPosition.y = 0;
        return HexCoordinates.ConvertPositionToOffset(worldPosition);
    }
}

// This stores coordinate offsets for use in the GetNeighboursFor function.
public static class Direction
{
    public static readonly List<Vector3Int> DirectionsOffsetOdd = new()
    {
        new Vector3Int(-1, 0, 1), //NW
        new Vector3Int(0, 0, 1), //NE
        new Vector3Int(1, 0, 0), //E
        new Vector3Int(0, 0, -1), //SE
        new Vector3Int(-1, 0, -1), //SW
        new Vector3Int(-1, 0, 0), //W
    };

    public static readonly List<Vector3Int> DirectionsOffsetEven = new()
    {
        new Vector3Int(0, 0, 1), //NW
        new Vector3Int(1, 0, 1), //NE
        new Vector3Int(1, 0, 0), //E
        new Vector3Int(1, 0, -1), //SE
        new Vector3Int(0, 0, -1), //SW
        new Vector3Int(-1, 0, 0), //W
    };

    public static List<Vector3Int> GetDirectionList(int z)
        => z % 2 == 0 ? DirectionsOffsetEven : DirectionsOffsetOdd;
}