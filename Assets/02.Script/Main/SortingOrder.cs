using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrder : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    private SpriteRenderer spriteRenderer;
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update() {
        spriteRenderer.sortingOrder = 20 - (int)pivot.position.y;
    }
}
