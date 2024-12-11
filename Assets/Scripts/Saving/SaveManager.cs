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
            if (string.IsNullOrEmpty(prefabId) || string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"Prefab ID or GUID not found in ISaveData for saveable: {saveable}. These must be present to be saved.");
                continue;
            }
            serializedData.Add(new SerializableSaveData(saveData.GetType().AssemblyQualifiedName, saveDataJson));
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
        foreach (var dataEntry in serializedList.Data)
        {
            // Extract type and data
            var jsonData = dataEntry.JsonData;
            var type = Type.GetType(dataEntry.TypeName);
            if (type == null)
            {
                Debug.LogError($"Type not found: {dataEntry.TypeName}");
                continue;
            }
            
            // load the object from json
            var objFromJson = JsonUtility.FromJson(jsonData, type);
            if (objFromJson is not ISaveData saveData)
            {
                Debug.LogError($"Data interface was wrong");
                continue;
            }

            dataEntry.saveData = saveData;
        }
        
        // Establish an instantiation order
        var objectsToQueue = serializedList.Data.Where(x => !string.IsNullOrEmpty(x.saveData.ParentReference.ParentGuid)).ToList(); // All but root objects
        var objectsToInstantiate = serializedList.Data.Where(x => string.IsNullOrEmpty(x.saveData.ParentReference.ParentGuid)).ToList(); // Root objects and resulting sorted list

        var enqueuedLastBatch = new List<SerializableSaveData>(objectsToInstantiate); // Track the last batch of instantiated objects
        
        // Sanitize out objects with missing parents
        var allGuids = new HashSet<string>(serializedList.Data.Select(x => x.saveData.GUID));
        
        objectsToQueue.RemoveAll(obj =>
        {
            if (!string.IsNullOrEmpty(obj.saveData.ParentReference.ParentGuid) && !allGuids.Contains(obj.saveData.ParentReference.ParentGuid))
            {
                Debug.LogWarning($"Object {obj.saveData.GUID} is missing its parent. This object will not be loaded.");
                return true;
            }
            return false;
        });

        // Loop until all items are queued in dependency order
        while (objectsToQueue.Count > 0)
        {
            var toQueue = new List<SerializableSaveData>();

            // Find objects that depend on the objects in enqueuedLastBatch and add them to toQueue
            var lastBatchGuids = new HashSet<string>(enqueuedLastBatch.Select(x => x.saveData.GUID));
            foreach (var obj in objectsToQueue)
            {
                if (obj.saveData.ParentReference != null && lastBatchGuids.Contains(obj.saveData.ParentReference.ParentGuid))
                {
                    toQueue.Add(obj);
                }
            }
            
            objectsToQueue.RemoveAll(toQueue.Contains);
            
            enqueuedLastBatch = toQueue;
            objectsToInstantiate.AddRange(toQueue);
        }

        
        foreach (var entry in objectsToInstantiate)
        {
            // Load the prefab
            GameObject prefab = Resources.Load<GameObject>($"{entry.saveData.PrefabID}");
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found: {entry.saveData.PrefabID}");
                continue;
            }
            
            // Find parent if needed
            Transform instantiationParent = null;
            if (!string.IsNullOrEmpty(entry.saveData.ParentReference.ParentGuid))
            {
                // Find the parent object in the instantiated objects list
                var parent = instantiatedObjects.FirstOrDefault(x => x.GUID == entry.saveData.ParentReference.ParentGuid);

                if (parent != null)
                {
                    if (string.IsNullOrEmpty(entry.saveData.ParentReference.PrefabChildId))
                    {
                        instantiationParent = parent.GameObject.transform;
                    }
                    else
                    {
                        PrefabChildID child = FindChildById(parent.GameObject.transform, entry.saveData.ParentReference.PrefabChildId);

                        if (child != null)
                        {
                            instantiationParent = child.gameObject.transform;
                        }
                        else
                        {
                            Debug.LogWarning($"Child with ID {entry.saveData.ParentReference.PrefabChildId} not found in parent {parent.GUID}");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"Parent with GUID {entry.saveData.ParentReference.ParentGuid} not found.");
                }
            }

            // Instantiate the prefab
            GameObject instance = Instantiate(prefab, instantiationParent);
            instantiatedObjects.Add(new InstantiatedObject{
                GameObject = instance,
                GUID = entry.saveData.GUID
            });

            // Assign the loaded data to the instantiated prefab
            var saveable = instance.GetComponent<ISaveable>();
            if (saveable != null)
            {
                saveable.LoadData(entry.saveData);
            }
        }
    }

    PrefabChildID FindChildById(Transform parentTransform, string childId)
    {
        foreach (Transform child in parentTransform)
        {
            var prefabChildID = child.GetComponent<PrefabChildID>();
            if (prefabChildID != null && prefabChildID.ChildID == childId)
            {
                return prefabChildID;
            }
            
            var foundChild = FindChildById(child, childId);
            if (foundChild != null)
            {
                return foundChild;
            }
        }
    
        return null;
    }

    [System.Serializable]
    public class SerializableSaveData
    {
        public string TypeName;
        public string JsonData;
        public ISaveData saveData;

        public SerializableSaveData(string typeName, string jsonData)
        {
            TypeName = typeName;
            JsonData = jsonData;
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
        public string GUID;
    }
}
