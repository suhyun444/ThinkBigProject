using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCollectionUI : MonoBehaviour
{
    [HideInInspector] public PetUI petUI;
    [SerializeField] private GameObject petCollectionUI;
    [SerializeField] private CustomButton[] petButtons;
    [SerializeField] private PetButtonData[] petButtonDatas;
    [SerializeField] private CustomButton[] moveButton;
    private int page = 0;
    private Vector3 startMousePosition;
    private bool onSwipe = false;
    private void Awake() {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            petButtons[i].BindClickEvent(() => petUI.OpenPetDataUI(page,index));
        }
        moveButton[0].BindClickEvent(() => BindData(page - 1));
        moveButton[1].BindClickEvent(() => BindData(page + 1));
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
            if (-7.72f <= mousePositionInWorld.x && mousePositionInWorld.x < 7.4f && -12.0f <= mousePositionInWorld.y && mousePositionInWorld.y <= 12.45f)
            {
                startMousePosition = Input.mousePosition;
                onSwipe = true;
            }
        }
        if (onSwipe && Input.GetMouseButton(0))
        {
            if (startMousePosition.x - Input.mousePosition.x > 600.0f)
            {
                if (page != petUI.petDatas.Length / 6) BindData(page + 1);
            }
            if (startMousePosition.x - Input.mousePosition.x < -600.0f)
            {
                if (page != 0) BindData(page - 1);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            onSwipe = false;
        }
    }
    private void ShowButton()
    {
        if (page != 0) moveButton[0].gameObject.SetActive(true);
        else moveButton[0].gameObject.SetActive(false);
        if (page != petUI.petDatas.Length / 6) moveButton[1].gameObject.SetActive(true);
        else moveButton[1].gameObject.SetActive(false);
    }
    public void BindData(int page)
    {
        Debug.Log(page);
        this.page = page;
        ShowButton();
        for (int i = 0; i < 6; i++)
        {
            int curIndex = page * 6 + i;
            if (curIndex >= petUI.petDatas.Length)
                petButtons[i].gameObject.SetActive(false);
            else
            {
                petButtons[i].gameObject.SetActive(true);
                petButtonDatas[i].BindData(petUI.petDatas[curIndex]);
            }

        }
    }
}
