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
    private Vector3 prevMousePosition;
    private bool onDrag = false;
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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f));
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(mousePosition, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("DrawingBox"))
            {
                prevMousePosition = Input.mousePosition;
                onDrag = true;
            }
        }
        else if(onDrag && Input.GetMouseButton(0))
        {
            if(Mathf.Abs(prevMousePosition.y - mousePosition.y) < 0.5f)
            {
                
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            onDrag = false;
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
