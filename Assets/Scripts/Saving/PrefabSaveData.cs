using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSaveData : MonoBehaviour, ISaveable
{
    private string _guid;
    public string GUID => _guid;
    
    [SerializeField] private string _prefabID;
    public string PrefabID => _prefabID;

    private void Awake()
    {
        if (string.IsNullOrEmpty(_guid))
        {
            _guid = Guid.NewGuid().ToString();
        }
    }

    public ISaveData SaveData()
    {
        return new PrefabData
        {
            PrefabID = _prefabID,
            GUID = _guid,
            ParentGUID = transform.parent != null ? transform.parent.GetComponent<ISaveableComponent>()?.GUID : null,
            Position = transform.localPosition,
            Rotation = transform.localRotation,
            Scale = transform.localScale
        };
    }

    public void LoadData(ISaveData data)
    {
        var prefabData = (PrefabData)data;
        _prefabID = prefabData.PrefabID;
        _guid = prefabData.GUID;
        transform.localPosition = prefabData.Position;
        transform.localRotation = prefabData.Rotation;
        transform.localScale = prefabData.Scale;
    }
    
    [System.Serializable]
    public class PrefabData : ISaveData
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string PrefabID { get; set; }
        public string GUID { get; set; }
        public string ParentGUID { get; set; }
    }
}




