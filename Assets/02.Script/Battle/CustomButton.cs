using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class CustomButton : MonoBehaviour,IPointerClickHandler//,IPointerDownHandler,IPointerUpHandler
{
    public delegate void ButtonClickFunc();
    private ButtonClickFunc onClick;
    private SpriteRenderer spriteRenderer;
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
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
    }
    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     spriteRenderer.color = Color.white;
    // }
    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     float grayScale = 150.0f / 255.0f;
    //     spriteRenderer.color = new Color(grayScale,grayScale,grayScale);
        
    // }
}
