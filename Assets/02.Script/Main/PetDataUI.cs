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
    [SerializeField] private TextMeshPro requirementText;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private TextMeshPro warningText;
    float warningTime = 0.0f;

    [Header("Slot")]
    //[SerializeField] private bool 
    [SerializeField] private GameObject slotUI; 
    [SerializeField] private SpriteRenderer[] spriteRenderer;
    [SerializeField] private CustomButton[] slots;

    private int petId;

    private void Awake() {
        denyButton.BindClickEvent(()=>warningText.gameObject.SetActive(false));
        denyButton.AddClickEvent(petUI.ClosePetDataUI);
        for(int i=0;i<4;i++)
        {
            int index = i;
            slots[i].BindClickEvent(()=>SelectSlot(index));
        }
    }
    private void Update() {
        warningTime -= Time.deltaTime;
        if(warningTime < 0.0f)
        {
            warningText.gameObject.SetActive(false);
        }
    }
    public void OpenPetData(int page,int index)
    {
        index = page*6 + index;
        petId = index;
        mainSprite.sprite = petUI.petDatas[index].mainSprite;
        name.text = petUI.petDatas[index].name;
        description.text = petUI.petDatas[index].description;
        SetAcceptButton();
    }
    private void SetAcceptButton()
    {
        acceptButton.gameObject.SetActive(true);
        requirementText.gameObject.SetActive(false);
        costText.transform.parent.gameObject.SetActive(false);
        acceptText.gameObject.SetActive(true);
        if(SaveManager.Instance.GetHavingPetList().Contains(petId))
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
        else
        {
            if(petUI.petDatas[petId].farmingType == FarmingType.Buy)
            {
                SetBuyLayout();
            }
            else
            {
                SetRequirementLayout();
            }
        }
    }
    private void SetBuyLayout()
    {
        costText.transform.parent.gameObject.SetActive(true);
        acceptText.gameObject.SetActive(false);
        acceptButton.BindClickEvent(BuyPet);
        acceptText.text = "구매";
        costText.text = petUI.petDatas[petId].cost.ToString();
    }
    private void BuyPet()
    {
        int curCrystal = SaveManager.Instance.GetCrystalData();
        if(curCrystal >= petUI.petDatas[petId].cost)
        {
            SaveManager.Instance.SetCrystalData(curCrystal - petUI.petDatas[petId].cost);
            SaveManager.Instance.AddHavingPetList(petId);
            SaveManager.Instance.SaveData();
            SetAcceptButton();
        }
        else
        {
            warningTime = 0.7f;
            warningText.gameObject.SetActive(true);
        }
    }
    private void SetRequirementLayout()
    {
        acceptButton.gameObject.SetActive(false);
        requirementText.gameObject.SetActive(true);
        requirementText.text = petUI.petDatas[petId].requirementDescription;
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
