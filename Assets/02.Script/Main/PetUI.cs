using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class PetButtonData
{
    public int index;
    public SpriteRenderer spriteRenderer;
    public void BindData(int index,PetData petData)
    {
        this.index = index;
        spriteRenderer.sprite = petData.mainSprite;
    }
}
public class PetUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private CustomButton exitButton;
    [Header("PetCollectionUI")]
    [SerializeField] private GameObject petCollectionUI;
    [SerializeField] private CustomButton[] petButtons;
    [SerializeField] private PetButtonData[] petButtonDatas;
    [SerializeField] private PetData[] petDatas;
    [SerializeField] private CustomButton[] moveButton;
    private int page = 0;

    [Header("PetDataUI")]
    [SerializeField] private GameObject petDataUI;
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private TextMeshPro name;
    [SerializeField] private TextMeshPro description;
    [SerializeField] private CustomButton acceptButton;
    [SerializeField] private TextMeshPro acceptText;
    [SerializeField] private CustomButton denyButton;

    
    private Vector3 startMousePosition;
    private bool onSwipe = false;
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(()=>ui.SetActive(false));
        denyButton.BindClickEvent(ClosePetData);
        for(int i=0;i<6;i++)
        {
            int index = i;
            petButtons[i].BindClickEvent(()=>OpenPetData(index));
        }
        moveButton[0].BindClickEvent(()=>BindData(page-1));
        moveButton[1].BindClickEvent(()=>BindData(page+1));
    }
    private void Update() {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
            if(-7.72f <= mousePositionInWorld.x && mousePositionInWorld.x < 7.4f && -12.0f <= mousePositionInWorld.y && mousePositionInWorld.y <= 12.45f)
            {
                startMousePosition = Input.mousePosition;
                onSwipe = true;
            }
        }
        if(onSwipe && Input.GetMouseButton(0))
        {
            if(startMousePosition.x - Input.mousePosition.x > 600.0f)
            {
                if(page != petDatas.Length / 6)BindData(page+1);
            }
            if(startMousePosition.x - Input.mousePosition.x < -600.0f)
            {
                if(page != 0)BindData(page - 1);
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            onSwipe = false;
        }
    }
    private void ShowButton()
    {
        if(page != 0)moveButton[0].gameObject.SetActive(true);
        else moveButton[0].gameObject.SetActive(false);
        if(page != petDatas.Length / 6)moveButton[1].gameObject.SetActive(true);
        else moveButton[1].gameObject.SetActive(false);
    }
    private void OpenUI()
    {
        petDataUI.SetActive(false);
        petCollectionUI.SetActive(true);
        BindData(0);
        ui.SetActive(true);
    }
    private void BindData(int page)
    {
        Debug.Log(page);
        this.page = page;
        ShowButton();
        for(int i=0;i<6;i++)
        {
            int curIndex = page * 6 + i;
            if(curIndex >= petDatas.Length)
                petButtons[i].gameObject.SetActive(false);
            else
            {
                petButtons[i].gameObject.SetActive(true);
                petButtonDatas[i].BindData(curIndex,petDatas[curIndex]);
            }

        }
    }
    private void OpenPetData(int index)
    {
        petDataUI.SetActive(true);
        petCollectionUI.SetActive(false);
        Debug.Log(index);
        index = petButtonDatas[index].index;
        mainSprite.sprite = petDatas[index].mainSprite;
        name.text = petDatas[index].name;
        description.text = petDatas[index].description;
        //acceptButton;
        //acceptText;
    }
    private void ClosePetData()
    {
        BindData(page);
        petDataUI.SetActive(false);
        petCollectionUI.SetActive(true);
    }

}
