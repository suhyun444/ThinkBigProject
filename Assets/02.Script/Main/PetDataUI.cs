using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PetDataUI : MonoBehaviour
{
    [SerializeField] private MainManager mainManager;
    [HideInInspector] public PetUI petUI;
    [SerializeField] private GameObject petDataUI;
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private TextMeshPro name;
    [SerializeField] private TextMeshPro description;
    [SerializeField] private CustomButton acceptButton;
    [SerializeField] private TextMeshPro acceptText;
    [SerializeField] private CustomButton denyButton;

    [Header("Slot")]
    //[SerializeField] private bool 
    [SerializeField] private GameObject slotUI; 
    [SerializeField] private SpriteRenderer[] spriteRenderer;
    [SerializeField] private CustomButton[] slots;

    private int petId;

    private void Awake() {
        denyButton.BindClickEvent(petUI.ClosePetDataUI);
        for(int i=0;i<4;i++)
        {
            int index = i;
            slots[i].BindClickEvent(()=>SelectSlot(index));
        }
    }
    public void OpenPetData(int page,int index)
    {
        petId = index;
        index = page*6 + index;
        mainSprite.sprite = petUI.petDatas[index].mainSprite;
        name.text = petUI.petDatas[index].name;
        description.text = petUI.petDatas[index].description;
        SetAcceptButton();
    }
    private void SetAcceptButton()
    {
        if(SaveManager.Instance.GetPetList().Contains(petId))
        {
            acceptButton.BindClickEvent(Despawn);
            acceptText.text = "소환 해제";
        }
        else
        {
            acceptButton.BindClickEvent(OpenSlotUI);
            acceptText.text = "소환";
        }

    }
    private void OpenSlotUI()
    {
        List<int> petList = SaveManager.Instance.GetPetList();
        for(int i=0;i<4;i++)
        {
            if(petList[i] == -1)
            {
                SelectSlot(i);
                return;
            }
        }
        SetSlot();
        slotUI.SetActive(true);
    }
    private void Despawn()
    {
        List<int> petList = SaveManager.Instance.GetPetList();
        for (int i = 0; i < 4; i++)
        {
            if (petList[i] == petId)
            {
                mainManager.ChangePet(i,-1);
                SetAcceptButton();
                return;
            }
        }
    }
    private void SetSlot()
    {
        List<int> petList = SaveManager.Instance.GetPetList();
        for(int i=0;i<petList.Count;i++)
        {
            spriteRenderer[i].sprite = (petList[i]!=-1)? petUI.petDatas[petList[i]].mainSprite:null;
        }
    }
    private void SelectSlot(int index)
    {
        mainManager.ChangePet(index,petId);
        SetAcceptButton();
        slotUI.SetActive(false);
    }
}
