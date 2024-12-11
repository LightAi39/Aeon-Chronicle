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
    
    protected virtual void Awake()
    {
        if (string.IsNullOrEmpty(_guid))
        {
            _guid = Guid.NewGuid().ToString();
        }
    }
    
    public virtual ISaveData SaveData()
    {
        if (string.IsNullOrEmpty(_prefabId)) throw new Exception("_prefabId may not be null or empty. Without this property, the object cannot be instantiated.");

        string currentGuid = _guid;
        if (string.IsNullOrEmpty(currentGuid))
        {
            currentGuid = Guid.NewGuid().ToString();
        }
        
        var data = CreateSaveDataInstance();
        data.Position = transform.position;
        data.Rotation = transform.rotation;
        data.Scale = transform.localScale;
        data.PrefabID = _prefabId;
        data.GUID = currentGuid;
        data.ParentReference = GetParent();
        return data;
    }

    public virtual void LoadData(ISaveData data)
    {
        var transformData = data;
        transform.position = transformData.Position;
        transform.rotation = transformData.Rotation;
        transform.localScale = transformData.Scale;
        _prefabId = transformData.PrefabID;
        _guid = transformData.GUID;
    }
    
    protected abstract ISaveData CreateSaveDataInstance();

    public virtual ParentReference GetParent()
    {
        var parentSaveable = transform.parent?.GetComponent<ISaveable>();
        if (parentSaveable != null)
            return new ParentReference
            {
                ParentGuid = parentSaveable.GUID,
                PrefabChildId = null
            };
        
        var childIdInParent = transform.parent?.GetComponent<PrefabChildID>();
        if (childIdInParent == null) return null;
            
        // Find the ISaveable that is the eventual parent of this object
        Transform parentTransform = transform.parent;
        ISaveable parent = null;

        // Traverse upwards through the hierarchy to find an ISaveable
        while (parentTransform != null)
        {
            parent = parentTransform.GetComponent<ISaveable>();
            if (parent != null)
            {
                break;  // Found the first ISaveable parent
            }
            parentTransform = parentTransform.parent;  // Move up the hierarchy
        }

        // If no ISaveable parent was found, return null
        if (parent == null)
        {
            Debug.LogWarning($"A ChildID {childIdInParent.ChildID} was found, but no ISaveable to act as top level parent was detected. Parent reference won't be saved!");
            return null;
        }

        return new ParentReference
        {
            ParentGuid = parent.GUID,
            PrefabChildId = childIdInParent.ChildID
        };

    }
}

[Serializable]
public class PrefabData : ISaveData
{

}
