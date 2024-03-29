using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    SerializedProperty tlTileProp;
    SerializedProperty tmTileProp;
    SerializedProperty trTileProp;
    SerializedProperty mlTileProp;
    SerializedProperty mmTileProp;
    SerializedProperty mrTileProp;
    SerializedProperty blTileProp;
    SerializedProperty bmTileProp;
    SerializedProperty brTileProp;
    bool showTiles = true;

    void OnEnable () {
        //Get refrences to properties
        tlTileProp = serializedObject.FindProperty ("tlTile");
        tmTileProp = serializedObject.FindProperty ("tmTile");
        trTileProp = serializedObject.FindProperty ("trTile");
        mlTileProp = serializedObject.FindProperty ("mlTile");
        mmTileProp = serializedObject.FindProperty ("mmTile");
        mrTileProp = serializedObject.FindProperty ("mrTile");
        blTileProp = serializedObject.FindProperty ("blTile");
        bmTileProp = serializedObject.FindProperty ("bmTile");
        brTileProp = serializedObject.FindProperty ("brTile");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        showTiles = EditorGUILayout.Foldout(showTiles, "Tiles");
        if (showTiles)
        {
            DrawTileInspector();
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTileInspector()
    {
        //Draw tile inspector
        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField("T", GUILayout.Width(20));
        EditorGUILayout.PropertyField (tlTileProp, new GUIContent (""));
        EditorGUILayout.PropertyField (tmTileProp, new GUIContent (""));
        EditorGUILayout.PropertyField (trTileProp, new GUIContent (""));
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField("M", GUILayout.Width(20));
        EditorGUILayout.PropertyField (mlTileProp, new GUIContent (""));
        EditorGUILayout.PropertyField (mmTileProp, new GUIContent (""));
        EditorGUILayout.PropertyField (mrTileProp, new GUIContent (""));
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField("B", GUILayout.Width(20));
        EditorGUILayout.PropertyField (blTileProp, new GUIContent (""));
        EditorGUILayout.PropertyField (bmTileProp, new GUIContent (""));
        EditorGUILayout.PropertyField (brTileProp, new GUIContent (""));
        EditorGUILayout.EndHorizontal ();


    }
}
