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
            string prefabId = saveData.PrefabID;
            string guid = saveData.GUID;
            string parentGuid = saveData.ParentGUID;
            if (string.IsNullOrEmpty(prefabId) || string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"Prefab ID or GUID not found in ISaveData for saveable: {saveable}. These must be present to be saved.");
                continue;
            }
            serializedData.Add(new SerializableSaveData(saveData.GetType().AssemblyQualifiedName, saveDataJson, prefabId, guid, parentGuid));
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
        
        List<InstantiatedObject> instantiatedObjects = new(); // To track loaded objects so we can later give them their parents.
        
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
            string guid = serializedEntry.GUID;

            // Load the prefab
            GameObject prefab = Resources.Load<GameObject>($"{prefabID}");
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found: {prefabID}");
                continue;
            }

            // Instantiate the prefab
            GameObject instance = Instantiate(prefab);
            instantiatedObjects.Add(new InstantiatedObject{
                GameObject = instance,
                ParentGUID = serializedEntry.ParentGUID
            });

            // Assign the loaded data to the instantiated prefab
            var saveable = instance.GetComponent<ISaveable>();
            if (saveable != null)
            {
                var objFromJson = JsonUtility.FromJson(jsonData, type);
                if (objFromJson is ISaveData saveData)
                {
                    saveable.LoadData(saveData);
                }
                else
                {
                    Debug.LogError($"ISaveData not found: {objFromJson}. The data of instance {instance} will not be loaded.");
                }
            }
        }
        
        // prepare a list of objects with GUIDs
        Dictionary<string, GameObject> possibleParents = new Dictionary<string, GameObject>();
        var objects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().ToList();
        
        foreach (var guidObjects in objects)
        {
            string guid = guidObjects.GUID;
            
            if (!string.IsNullOrEmpty(guid) && guidObjects is MonoBehaviour saveableObject)
            {
                possibleParents[guid] = saveableObject.gameObject;
            }
        }
        
        
        // resolve parent-child relationships based on GUIDs
        foreach (var obj in instantiatedObjects)
        {
            string parentGUID = obj.ParentGUID;
            if (!string.IsNullOrEmpty(parentGUID) && possibleParents.TryGetValue(parentGUID, out var possibleParent))
            {
                GameObject parent = possibleParent.gameObject;
                obj.GameObject.transform.SetParent(parent.transform);
            }
            else if (!string.IsNullOrEmpty(parentGUID))
            {
                Debug.LogWarning("Parent not found: " + parentGUID);
            }
        }
        
        
    }
    
    [System.Serializable]
    public class SerializableSaveData
    {
        public string TypeName;
        public string JsonData;
        public string PrefabID;
        public string GUID;
        public string ParentGUID; // Parent GUID for parent-child relationships

        public SerializableSaveData(string typeName, string jsonData, string prefabID, string guid, string parentGUID = null)
        {
            TypeName = typeName;
            JsonData = jsonData;
            PrefabID = prefabID;
            GUID = guid;
            ParentGUID = parentGUID;
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

    private class InstantiatedObject
    {
        public GameObject GameObject;
        public string ParentGUID;
    }
}
