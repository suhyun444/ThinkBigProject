using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FarmingType
{
    Buy,
    Requirement
}
[CreateAssetMenu(fileName = "Pet Data",menuName ="Scriptable Object/Pet Data")]
public class PetData : ScriptableObject
{
    public Sprite mainSprite;
    public Sprite lockSprite;
    public int id;
    public string name;
    [TextArea]
    public string description;
    public FarmingType farmingType;
    public int cost;
    public string requirementKey;
    [TextArea]
    public string requirementDescription;
}
