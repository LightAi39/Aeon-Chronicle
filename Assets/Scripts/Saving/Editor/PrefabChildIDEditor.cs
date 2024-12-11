#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabChildID))]
public class PrefabChildIDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PrefabChildID myScript = (PrefabChildID)target;
        
        if (GUILayout.Button("Generate Child ID"))
        {
            myScript.SetChildID(System.Guid.NewGuid().ToString());
            
            EditorUtility.SetDirty(myScript);
        }
        
        DrawDefaultInspector();
    }
}
#endif