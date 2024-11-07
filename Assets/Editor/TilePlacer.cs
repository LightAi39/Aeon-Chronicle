using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class TilePlacer : EditorWindow
{
    private float tileWidth = 3.00f;
    private float tileHeight = 3.00f;
    private float tolerance = 0.0001f;

    [MenuItem("Window/TilePlacer")]
    public static void ShowWindow()
    {
        GetWindow<TilePlacer>("Tile Placer Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tile Placer", EditorStyles.boldLabel);

        tileWidth = EditorGUILayout.FloatField("Tile Width", tileWidth);
        tileHeight = EditorGUILayout.FloatField("Tile Height", tileHeight);

        GUILayout.Space(10);

        GUILayout.Label("Position Controls", EditorStyles.boldLabel);

        // Row for the X-axis buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-X"))
        {
            MoveTiles(Vector3.left * tileWidth * Mathf.Cos(30f * Mathf.Deg2Rad));
        }
        if (GUILayout.Button("+X"))
        {
            MoveTiles(Vector3.right * tileWidth * Mathf.Cos(30f * Mathf.Deg2Rad));
        }
        GUILayout.EndHorizontal();

        // Row for the Z-axis buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-Z"))
        {
            MoveTiles(Vector3.back * tileHeight);
        }
        if (GUILayout.Button("+Z"))
        {
            MoveTiles(Vector3.forward * tileHeight);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Snap position"))
        {
            SnapPosition();
        }
        GUILayout.EndHorizontal();
    }

    private void MoveTiles(Vector3 direction)
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Move Hex Tile"); 
            float cosWidth = tileWidth * Mathf.Cos(30f * Mathf.Deg2Rad);

            Vector3 position = obj.transform.position;

            position += direction;
            position.x = Mathf.Round(position.x / cosWidth) * cosWidth;

            if (direction.x != 0)
            {
                if (direction.x > 0)
                {
                    position.z -= tileHeight * 0.5f;
                }
                else if (direction.x < 0)
                {
                    position.z += tileHeight * 0.5f;
                }
            }

            obj.transform.position = position;
            EditorUtility.SetDirty(obj); 
        }
    }

    private void SnapPosition()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Snap Position"); 

            Vector3 position = obj.transform.position;
            float cosWidth = tileWidth * Mathf.Cos(30f * Mathf.Deg2Rad);

            position.x = Mathf.Round(position.x / cosWidth) * cosWidth;
            
            if (MathF.Abs(Mathf.Abs(position.x / 2) % cosWidth) > tolerance)
            {
                bool negativeZ = true;
                if(position.z > 0)
                {
                    negativeZ = false;
                }
                position.z = Mathf.Round(position.z / (tileHeight/2f)) * (tileHeight/2f);
                if (position.z % tileHeight < tolerance)
                {
                    position.z += tileHeight/2f;
                }
                if(negativeZ == false)
                {
                    position.z += tileHeight/2f;
                }
                else
                {
                    position.z -= tileHeight/2f;
                }
            }
            else
            {
                position.z = Mathf.Round(position.z / tileHeight) * tileHeight;
            }

            position.y = 0;
            obj.transform.position = position;
            EditorUtility.SetDirty(obj);
        }
    }
}
