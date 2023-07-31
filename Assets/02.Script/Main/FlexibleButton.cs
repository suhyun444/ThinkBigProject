using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class FlexibleButton : MonoBehaviour,IPointerClickHandler,IPointerDownHandler//,IPointerUpHandler
{
    public delegate void ButtonClickFunc();
    private ButtonClickFunc onClick;
    private SpriteRenderer spriteRenderer;
    public bool isClicked = false;
    private void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
    }
    public void BindClickEvent(ButtonClickFunc func)
    {
        onClick = func;
    }
    public void AddClickEvent(ButtonClickFunc func)
    {
        onClick += func;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true; 
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(isClicked)
            onClick.Invoke();
    }
    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     spriteRenderer.color = Color.white;
    // }
}
