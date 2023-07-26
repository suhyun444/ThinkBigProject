using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextOutline : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    [SerializeField] private float thickness;
    [SerializeField] private Color32 color;
    private void Awake() {
        textMeshPro = GetComponent<TextMeshPro>();
    }
    private void Update() {
        textMeshPro.fontMaterial.SetFloat("_FaceDilate",0.2f);
        textMeshPro.outlineColor = color;
        textMeshPro.outlineWidth = thickness;
        
    }
}
