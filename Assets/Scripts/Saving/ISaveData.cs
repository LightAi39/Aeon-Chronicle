using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ISaveData
{ 
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public string PrefabID;
    public string GUID;
    public string ParentGuid;
}

public abstract class ParentReference
{
    public string ParentGuid;
    public string PrefabChildId;
}