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
    [SerializeField] private SortPosition costText;
    [SerializeField] private TextMeshPro warningText;
    [SerializeField] private CustomButton slotExitButton;

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
        slotExitButton.BindClickEvent(()=>slotUI.SetActive(false));
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
    public void OpenPetData(int index)
    {
        warningText.gameObject.SetActive(false);
        petId = index;
        mainSprite.sprite = petUI.petDatas[index].mainSprite;
        name.text = (LanguageManager.Instance.languageType == LanguageType.Korean) ? petUI.petDatas[index].name : petUI.petDatas[index].engName;
        description.text = (LanguageManager.Instance.languageType == LanguageType.Korean) ? petUI.petDatas[index].description : petUI.petDatas[index].engDescription;
        SetAcceptButton();
    }
    private void SetAcceptButton()
    {
        acceptButton.gameObject.SetActive(true);
        requirementText.gameObject.SetActive(false);
        costText.transform.parent.gameObject.SetActive(false);
        acceptText.gameObject.SetActive(true);
        mainSprite.color = Color.white;
        if(SaveManager.Instance.GetHavingPetList().Contains(petId))
        {
            if(SaveManager.Instance.GetPetList().Contains(petId))
            {
                acceptButton.BindClickEvent(Despawn);
                acceptText.text = (LanguageManager.Instance.languageType == LanguageType.Korean) ?  "소환 해제" : "Release";
            }
            else
            {
                acceptButton.BindClickEvent(OpenSlotUI);
                acceptText.text = (LanguageManager.Instance.languageType == LanguageType.Korean) ? "소환" : "Summon";
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
        acceptText.text = (LanguageManager.Instance.languageType == LanguageType.Korean) ? "구매" : "Buy";
        costText.SetText("x" + petUI.petDatas[petId].cost.ToString());
    }
    private void BuyPet()
    {
        int curCrystal = SaveManager.Instance.GetCrystalData();
        if(curCrystal >= petUI.petDatas[petId].cost)
        {
            SoundManager.Instance.PlaySoundEffect(Sound.Buy);
            SaveManager.Instance.SetCrystalData(curCrystal - petUI.petDatas[petId].cost);
            SaveManager.Instance.AddHavingPetList(petId);
            SaveManager.Instance.SaveData();
            SetAcceptButton();
        }
        else
        {
            SoundManager.Instance.PlaySoundEffect(Sound.Warning);
            warningTime = 0.7f;
            warningText.gameObject.SetActive(true);
        }
    }
    private void SetRequirementLayout()
    {
        acceptButton.gameObject.SetActive(false);
        requirementText.gameObject.SetActive(true);
        requirementText.text = (LanguageManager.Instance.languageType == LanguageType.Korean) ? petUI.petDatas[petId].requirementDescription : petUI.petDatas[petId].engRequirementDescription;
        mainSprite.color = Color.black;
    }
    private void OpenSlotUI()
    {
        SoundManager.Instance.PlaySoundEffect(Sound.ButtonClick);
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
        SoundManager.Instance.PlaySoundEffect(Sound.ButtonClick);
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
