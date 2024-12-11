using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface ISaveable : ISaveableComponent
{
    string PrefabID { get; }
    ISaveData SaveData();
    void LoadData(ISaveData data);
}
