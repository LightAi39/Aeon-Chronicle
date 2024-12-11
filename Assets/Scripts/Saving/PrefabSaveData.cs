using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSaveData : SaveableMonoBehaviour
{
    protected override ISaveData CreateSaveDataInstance()
    {
        return new PrefabData();
    }
}
