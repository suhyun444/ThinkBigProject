using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        SaveManager manager = (SaveManager)target;
        if(GUILayout.Button("Init Data"))
        {
            manager.InitData();
        }
    }
}
