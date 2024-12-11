using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveableMonoBehaviour : MonoBehaviour, ISaveable
{
    private string _guid;
    public string GUID => _guid;
    [SerializeField] private string _prefabId;
    public string PrefabID => _prefabId;
    
    public virtual ISaveData SaveData()
    {
        if (string.IsNullOrEmpty(_prefabId)) throw new Exception("_prefabId may not be null or empty. Without this property, the object cannot be instantiated.");

        string currentGuid = _guid;
        if (string.IsNullOrEmpty(currentGuid))
        {
            currentGuid = Guid.NewGuid().ToString();
        }
        
        var data = CreateSaveDataInstance();
        data.Position = transform.localPosition;
        data.Rotation = transform.localRotation;
        data.Scale = transform.localScale;
        data.PrefabID = _prefabId;
        data.GUID = currentGuid;
        data.ParentGuid = transform.parent != null ? transform.parent.GetComponent<ISaveable>()?.GUID : null;
        return data;
    }

    public virtual void LoadData(ISaveData data)
    {
        var transformData = data;
        transform.localPosition = transformData.Position;
        transform.localRotation = transformData.Rotation;
        transform.localScale = transformData.Scale;
        _prefabId = transformData.PrefabID;
        _guid = transformData.GUID;
    }
    
    protected abstract ISaveData CreateSaveDataInstance();
}

[Serializable]
public class PrefabData : ISaveData
{

}
