using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides a coordinate on the hex map.
/// This could be refactored into being replaced by a unity 2d Grid component, as they essentially serve the same purpose of providing coordinates, if desired for other functionality.
/// </summary>
[SelectionBase]
public class HexCoordinates : MonoBehaviour
{
    public static float xOffset = 3, yOffset = 1, zOffset = 2.598f; // z is square root // these values should correspond to the hex dimensions

    [Header("Offset coordinates")] [SerializeField]
    private Vector3Int offsetCoordinates;

    private void Awake()
    {
        offsetCoordinates = ConvertPositionToOffset(transform.position);
    }
    
    // This converts the world position into the hex grid coordinate
    public static Vector3Int ConvertPositionToOffset(Vector3 position)
    {
        int x = Mathf.CeilToInt(position.x / xOffset);
        int y = Mathf.RoundToInt(position.y / yOffset);
        int z = Mathf.RoundToInt(position.z / zOffset);
        return new Vector3Int(x, y, z);
    }

    public Vector3Int GetHexCoords()
        => offsetCoordinates;
}
