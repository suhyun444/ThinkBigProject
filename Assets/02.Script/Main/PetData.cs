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
    public int id;
    public string name;
    public string engName;
    [TextArea]
    public string description;
    [TextArea]
    public string engDescription;
    public FarmingType farmingType;
    public int cost;
    public string requirementKey;
    [TextArea]
    public string requirementDescription;
    [TextArea]
    public string engRequirementDescription;
}
