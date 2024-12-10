using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private List<ISaveable> _saveables = new();

    private void RegisterSaveables()
    {
        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        
        _saveables = saveables.ToList();
    }

    public void SaveTest()
    {
        Save("save.json");
    }

    public void LoadTest()
    {
        Load("save.json");
    }
    
    public void Save(string filePath)
    {
        RegisterSaveables();
        
        List<SerializableSaveData> serializedData = new List<SerializableSaveData>();
        foreach (var saveable in _saveables)
        {
            var saveData = saveable.SaveData();
            string saveDataJson = JsonUtility.ToJson(saveData);
            string prefabId = saveable.GetPrefabID();
            if (string.IsNullOrEmpty(prefabId))
            {
                Debug.LogError("Prefab ID not found for saveable: " + saveable);
                continue;
            }
            serializedData.Add(new SerializableSaveData(saveData.GetType().AssemblyQualifiedName, saveDataJson, prefabId));
        }
        
        string json = JsonUtility.ToJson(new SerializableSaveDataList(serializedData), true);
        File.WriteAllText(filePath, json);
    }

    public void Load(string filePath)
    {
        if (!File.Exists(filePath)) return;
        
        string json = File.ReadAllText(filePath);
        var serializedList = JsonUtility.FromJson<SerializableSaveDataList>(json);
        
        RegisterSaveables();
        // Clear existing saveables
        foreach (var saveable in _saveables)
        {
            if (saveable is MonoBehaviour mono) Destroy(mono.gameObject);
        }
        
        _saveables.Clear();
        
        
        // Process loaded data
        foreach (var serializedEntry in serializedList.Data)
        {
            // Extract type and data
            var jsonData = serializedEntry.JsonData;
            var type = Type.GetType(serializedEntry.TypeName);
            if (type == null)
            {
                Debug.LogError($"Type not found: {serializedEntry.TypeName}");
                continue;
            }
            
            string prefabID = serializedEntry.PrefabID;

            // Load the prefab
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabID}");
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found: Prefabs/{prefabID}");
                continue;
            }

            // Instantiate the prefab
            GameObject instance = Instantiate(prefab);

            // Assign the loaded data to the instantiated prefab
            var saveable = instance.GetComponent<ISaveable>();
            if (saveable != null)
            {
                saveable.LoadData(JsonUtility.FromJson(jsonData, type));
                _saveables.Add(saveable);
            }
        }
        
        
    }
    
    [System.Serializable]
    public class SerializableSaveData
    {
        public string TypeName; // The fully qualified name of the type
        public string JsonData;
        public string PrefabID;

        public SerializableSaveData(string typeName, string jsonData, string prefabID)
        {
            TypeName = typeName;
            JsonData = jsonData;
            PrefabID = prefabID;
        }
    }
    
    [System.Serializable]
    public class SerializableSaveDataList
    {
        public List<SerializableSaveData> Data;

        public SerializableSaveDataList(List<SerializableSaveData> data)
        {
            Data = data;
        }
    }
}
