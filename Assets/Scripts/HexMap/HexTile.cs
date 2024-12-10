using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class HexTile : MonoBehaviour, ISaveable
{
    [SerializeField] private string prefabID;
    [SerializeField] private GlowHighlight highlight;
    private HexCoordinates _hexCoordinates;

    [SerializeField] private HexType hexType;

    public Vector3Int HexCoords => _hexCoordinates.GetHexCoords();
    
    public int GetCost() => hexType switch
    {
        HexType.Difficult => 20,
        HexType.Default => 10,
        HexType.Road => 5,
        HexType.Town => 10,
        HexType.PoI => 10,
        _ => throw new Exception($"Hex of type {hexType} not supported")
    };

    public bool IsObstacle()
    {
        return this.hexType is HexType.Obstacle or HexType.Water or HexType.Requirement;
        // water later on could be a different movement cost and depend on if you have a boat or not, but for now its counted as an obstacle
        // requirement later on could be a different movement cost and depend on if you have a requirement or not (either if you are allowed to move on it or if the cost is different), but for now its counted as an obstacle
        // TODO: implement conditional movement costs
    }
    
    private void Awake()
    {
        _hexCoordinates = GetComponent<HexCoordinates>();
        highlight = GetComponent<GlowHighlight>();
    }

    public void EnableHighlight()
    {
        highlight.ToggleGlow(true);
    }

    public void DisableHighlight()
    {
        highlight.ToggleGlow(false);
    }

    public void ResetHighlight()
    {
        highlight.ResetGlowHighlight();
    }

    public void HighlightPath()
    {
        highlight.HighlightValidPath();
    }

    
    public object SaveData()
    {
        return new HexTileData()
        {
            Position = transform.position,
            hexType = hexType,
            PrefabId = prefabID
        };
        // TODO: add custom model saving.
    }

    public void LoadData(object data)
    {
        var hexTileData = data as HexTileData;
        if (hexTileData == null) throw new Exception("hexTileData is null");
        
        transform.position = hexTileData.Position;
        hexType = hexTileData.hexType;
        prefabID = hexTileData.PrefabId;

        // TODO: load custom model
    }

    public string GetPrefabID() => prefabID;

    [Serializable]
    private class HexTileData
    {
        public Vector3 Position;
        public HexType hexType;

        public string PrefabId;
        // TODO: add custom models
    }
}

public enum HexType
{
    None,
    Default,
    Difficult,
    Road,
    Town,
    PoI,
    Water,
    Requirement,
    Obstacle
}