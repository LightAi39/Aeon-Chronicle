using UnityEngine;
using UnityEngine.Serialization;

public class PrefabChildID : MonoBehaviour
{
    [SerializeField]
    private string childID;

    public string ChildID => childID;
    
    public void SetChildID(string id)
    {
        childID = id;
    }
}