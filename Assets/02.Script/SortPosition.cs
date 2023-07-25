using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SortPosition : MonoBehaviour
{
    [SerializeField] private TextMeshPro costText;
    public float crystalSize = 0.2f;
    public float oneFontSize = 0.075f;
    public float spaceBetweenSpriteAndText = 0.44f;
    [SerializeField] private Transform spriteTransform;
    public void SetText(string text)
    {
        costText.text = text;
        Sort();
    }
    private void Sort()
    {
        int size = costText.text.Length;
        float width = crystalSize + size * oneFontSize;
        float halfWidth = width / 2;
        spriteTransform.localPosition = new Vector3(-halfWidth + 0.03f,spriteTransform.localPosition.y,0);
        costText.transform.localPosition = new Vector3(spaceBetweenSpriteAndText - halfWidth + 0.03f,costText.transform.localPosition.y,0);

    }
}
