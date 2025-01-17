using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Character))]
public class DamageTypeWeaknessEditor : Editor
{
    private ReorderableList reorderableList;
    private static float defaultWeakness = 100f;

    private void OnEnable()
    {
        var character = (Character)target;
        // Ensure weaknesses array is populated with defaults
        if (character.weaknesses == null || character.weaknesses.Length == 0)
        {
            character.weaknesses = new Character.Weakness[System.Enum.GetValues(typeof(DamageType)).Length];
            for (int i = 0; i < character.weaknesses.Length; i++)
            {
                character.weaknesses[i] = new Character.Weakness
                {
                    type = (DamageType)i,
                    weaknessValue = defaultWeakness
                };
            }
        }
        
        // Initialize the reorderable list with the weaknesses property
        SerializedProperty weaknessesProperty = serializedObject.FindProperty("weaknesses");

        // Ensure weaknesses array is populated with defaults
        if (weaknessesProperty == null || weaknessesProperty.arraySize == 0)
        {
            // Get the total number of damage types and initialize the array with default values
            int damageTypeCount = System.Enum.GetValues(typeof(DamageType)).Length;
            weaknessesProperty.arraySize = damageTypeCount;

            for (int i = 0; i < damageTypeCount; i++)
            {
                var element = weaknessesProperty.GetArrayElementAtIndex(i);
                var typeProperty = element.FindPropertyRelative("type");
                var valueProperty = element.FindPropertyRelative("weaknessValue");

                // Set default values
                typeProperty.enumValueIndex = i;
                valueProperty.floatValue = defaultWeakness;
            }
        }

        // Create the reorderable list to display weaknesses
        reorderableList = new ReorderableList(serializedObject, weaknessesProperty, false, true, false, false);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Damage Type Weaknesses");
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var typeProperty = element.FindPropertyRelative("type");
            var valueProperty = element.FindPropertyRelative("weaknessValue");

            rect.height = EditorGUIUtility.singleLineHeight;

            // Display the type as a label instead of an index
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 3, rect.height), typeProperty.enumDisplayNames[typeProperty.enumValueIndex]);

            // Display the weakness value
            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3 + 5, rect.y, rect.width * 2 / 3 - 5, rect.height), valueProperty, GUIContent.none);
        };

        reorderableList.elementHeightCallback = (int index) =>
        {
            return EditorGUIUtility.singleLineHeight + 5;
        };

        // Add a callback to handle changes in the reorderable list
        reorderableList.onChangedCallback = (ReorderableList list) =>
        {
            EditorUtility.SetDirty(target); // Mark the object as dirty for saving
        };
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object before rendering
        serializedObject.Update();

        // Render the custom reorderable list
        reorderableList.DoLayoutList();

        // Apply changes to the custom list before calling DrawDefaultInspector()
        serializedObject.ApplyModifiedProperties();

        // Draw the default inspector UI for other properties in the Character class
        DrawDefaultInspector();
    }
}
