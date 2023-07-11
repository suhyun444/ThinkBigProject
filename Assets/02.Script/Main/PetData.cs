using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pet Data",menuName ="Scriptable Object/Pet Data")]
public class PetData : ScriptableObject
{
    public Sprite mainSprite;
    public Sprite lockSprite;
    public int id;
    public string name;
    [TextArea]
    public string description;
}
