using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabGUIDComponent : MonoBehaviour, ISaveableComponent
{
    private string _guid;
    public string GUID => _guid;
    
    private void Awake()
    {
        if (string.IsNullOrEmpty(_guid))
        {
            _guid = Guid.NewGuid().ToString();
        }
    }
    
    public void SetGUID(string guid)
    {
        _guid = guid;
    }
}
