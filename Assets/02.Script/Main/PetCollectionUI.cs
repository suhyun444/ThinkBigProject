using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PetCollectionUI : MonoBehaviour
{
    [HideInInspector] public PetUI petUI;
    [SerializeField] private GameObject petCollectionUI;
    [SerializeField] private GameObject buttonParents;
    [SerializeField] private GameObject petButton;
    private int page = 0;
    [SerializeField] private GameObject handle;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] private float dragSensitivy;
    List<FlexibleButton> petButtons = new List<FlexibleButton>();
    private Vector3 prevMousePosition;
    private bool onDrag = false;
    private bool firstDrag = true;
    private void Awake() {
        InstantiatePetButtons();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f));
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(mousePosition, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("PetSlider"))
            {
                prevMousePosition = mousePosition;
                onDrag = true;
                firstDrag = true;
            }
        }
        else if(onDrag && Input.GetMouseButton(0))
        {
            if(Mathf.Abs(prevMousePosition.y - mousePosition.y) > 0.1f)
            {
                if(firstDrag)
                {
                for(int i=0;i<petButtons.Count;i++)
                    petButtons[i].isClicked = false;
                    firstDrag = false;
                }
                float nextY = buttonParents.transform.localPosition.y + (mousePosition.y - prevMousePosition.y) * dragSensitivy;
                nextY = Mathf.Clamp(nextY,minY,maxY);
                handle.transform.localPosition = new Vector3(1.3f,Mathf.Lerp(-0.54f,-2.15f, (nextY - minY) / (maxY - minY)),-1);
                buttonParents.transform.localPosition = new Vector3(buttonParents.transform.localPosition.x,nextY,0.0f);
                prevMousePosition = mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            onDrag = false;
        }
    }
    public void SetButtonViewPosition(float ratio)
    {
        float nextY = Mathf.Lerp(maxY, minY, ratio);
        buttonParents.transform.localPosition = new Vector3(buttonParents.transform.localPosition.x, nextY, 0.0f);
    }
    private void InstantiatePetButtons()
    {
        for(int i=0;i<petUI.petDatas.Length;i++)
        {
            Vector3 position = new Vector3(0.345f + 0.62f * (i % 2),0.345f + -0.82f * (i/2 + 1),0);
            GameObject gameObject = Instantiate(petButton,Vector3.zero,Quaternion.identity);
            int index =  i;
            petButtons.Add(gameObject.GetComponent<FlexibleButton>());
            petButtons[i].BindClickEvent(()=>petUI.OpenPetDataUI(index));
            gameObject.transform.parent = buttonParents.transform;
            gameObject.transform.localScale = new Vector3(1.2f,1.2f,1.0f);
            gameObject.transform.localPosition = position;
            BindData(gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>(),gameObject.GetComponentInChildren<TextMeshPro>(),i);
        }
    }
    public void BindData(SpriteRenderer spriteRenderer,TextMeshPro name, int index)
    {
        spriteRenderer.sprite = petUI.petDatas[index].mainSprite;
        name.fontMaterial.SetFloat("_Stencil",1);
        name.fontMaterial.SetFloat("_StencilComp",3);
        LanguageText languageText = name.GetComponent<LanguageText>();
        if (petUI.petDatas[index].farmingType == FarmingType.Requirement && !SaveManager.Instance.GetHavingPetList().Contains(petUI.petDatas[index].id))
        {
            spriteRenderer.color = Color.black;
            languageText.koreanText = "???";
            languageText.englishText = "???";
        }
        else
        {
            spriteRenderer.color = Color.white;
            languageText.koreanText = petUI.petDatas[index].name;
            languageText.englishText = petUI.petDatas[index].engName;
        }
        LanguageManager.Instance.AddText(languageText);
    }
}
