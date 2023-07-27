using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(SoundData))]
public class SoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundData soundSetting = target as SoundData;
        EditorGUILayout.LabelField($"Count : {soundSetting.count}");
        if (GUILayout.Button("plusSound"))
        {
            soundSetting.count++;
            soundSetting.nameList.Insert(0, null);
            soundSetting.clipList.Insert(0, null);
        }
        if (GUILayout.Button("minusSound"))
        {
            if (soundSetting.count == 0) return;
            soundSetting.count--;
            soundSetting.nameList.RemoveAt(0);
            soundSetting.clipList.RemoveAt(0);
        }

        GUILayoutOption[] labelOptions = new[]{
            GUILayout.Width(38f),
            GUILayout.Height(20f)
        };
        GUILayoutOption[] fieldOptions = new[]{
            GUILayout.Width(160f),
            GUILayout.Height(20f)
        };
        for(int i = 0; i < soundSetting.count; i++)
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField(i.ToString());
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("name", labelOptions);
            soundSetting.nameList[i] = EditorGUILayout.TextField(soundSetting.nameList[i], fieldOptions);
            EditorGUILayout.LabelField("clip", labelOptions);
            soundSetting.clipList[i] = EditorGUILayout.ObjectField(soundSetting.clipList[i], typeof(AudioClip), false, fieldOptions) as AudioClip;
            EditorGUILayout.EndHorizontal();
        }
        EditorUtility.SetDirty(target);
    }
}
