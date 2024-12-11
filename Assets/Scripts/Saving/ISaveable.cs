using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface ISaveable
{
    string GUID { get; }
    string PrefabID { get; }
    ISaveData SaveData();
    void LoadData(ISaveData data);
}
