using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class PressingButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public delegate void ButtonClickFunc();
    private ButtonClickFunc onClick;
    private SpriteRenderer spriteRenderer;
    public bool isPressed = false;
    private bool isClicked = false;
    float pressingTime = 0.0f;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (!isClicked) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f));
        RaycastHit2D raycastHit2D = Physics2D.Raycast(mousePosition, transform.forward, 999);
        if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("SkillUpgradeButton"))
        {
            pressingTime += Time.deltaTime;
            if (pressingTime >= Const.Skill.UPGRADE_BUTTON_DELAY)
            {
                SoundManager.Instance.PlaySoundEffect(Sound.ButtonClick);
                pressingTime = 0.0f;
                onClick.Invoke();
                isPressed = true;
            }
        }
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
        isPressed = false;
        pressingTime = 0.0f;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPressed)
        {
            SoundManager.Instance.PlaySoundEffect(Sound.ButtonClick);
            onClick.Invoke();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
    }
}