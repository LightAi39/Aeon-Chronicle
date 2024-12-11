using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class HexTile : MonoBehaviour, ISaveable
{
    private string _guid;
    public string GUID => _guid;
    
    [SerializeField] private string prefabID;
    public string PrefabID => prefabID;
    
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
        if (string.IsNullOrEmpty(_guid))
        {
            _guid = Guid.NewGuid().ToString();
        }
        
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

    public ISaveData SaveData()
    {
        return new HexTileData()
        {
            Position = transform.localPosition,
            Rotation = transform.localRotation,
            Scale = transform.localScale,
            hexType = hexType,
            PrefabID = prefabID,
            GUID = _guid,
            ParentGUID = transform.parent != null ? transform.parent.GetComponent<ISaveableComponent>()?.GUID : null
        };
    }

    public void LoadData(ISaveData data)
    {
        var hexTileData = data as HexTileData;
        if (hexTileData == null) throw new Exception("hexTileData is null");
        
        transform.localPosition = hexTileData.Position;
        transform.localRotation = hexTileData.Rotation;
        transform.localScale = hexTileData.Scale;
        hexType = hexTileData.hexType;
        prefabID = hexTileData.PrefabID;
        _guid = hexTileData.GUID;
    }

    [Serializable]
    private class HexTileData : ISaveData
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string PrefabID { get; set; }
        public string GUID { get; set; }
        public string ParentGUID { get; set; }
        
        public HexType hexType;
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