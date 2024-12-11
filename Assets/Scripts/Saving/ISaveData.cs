using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveData
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }
    
    public string PrefabID { get; set; }
    public string GUID { get; set; }
    public string ParentGUID { get; set; }
}
