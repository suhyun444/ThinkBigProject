using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SortPosition : MonoBehaviour
{
    private TextMeshPro costText;
    public float crystalSize = 0.2f;
    public float oneFontSize = 0.075f;
    public float spaceBetweenSpriteAndText = 0.44f;
    [SerializeField] private Transform spriteTransform;
    private void Awake() {
        costText = GetComponent<TextMeshPro>();
    }
    private void Update() {
        int size = costText.text.Length;
        float width = crystalSize + size * oneFontSize;
        float halfWidth = width / 2;
        spriteTransform.localPosition = new Vector3(-halfWidth + 0.03f,spriteTransform.localPosition.y,0);
        costText.transform.localPosition = new Vector3(spaceBetweenSpriteAndText - halfWidth + 0.03f,costText.transform.localPosition.y,0);
    }
}
