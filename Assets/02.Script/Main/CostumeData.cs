using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Costume Data", menuName = "Scriptable Object/Costume Data")]
public class CostumeData : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public int cost;
}

