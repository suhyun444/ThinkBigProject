using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextOutline : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    [SerializeField] private float dilate = 0.2f;
    [SerializeField] private float thickness;
    [SerializeField] private Color32 color;
    private void Awake() {
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.fontMaterial.SetFloat("_FaceDilate",dilate);
        textMeshPro.outlineColor = color;
        textMeshPro.outlineWidth = thickness;
    }
}
