using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "SoundData", menuName = "Sound")]
public class SoundData : ScriptableObject
{
    public int count;
    public List<string> nameList = new List<string>();
    public List<AudioClip> clipList = new List<AudioClip>();
}
