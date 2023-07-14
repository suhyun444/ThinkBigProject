using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class PetButtonData
{
    public SpriteRenderer spriteRenderer;
    public void BindData(PetData petData)
    {
        spriteRenderer.sprite = petData.mainSprite;
        if(petData.farmingType == FarmingType.Requirement && !SaveManager.Instance.GetHavingPetList().Contains(petData.id))
            spriteRenderer.color = Color.black;
        else
            spriteRenderer.color = Color.white;
    }
}
public class PetUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private CustomButton exitButton;
    public PetData[] petDatas;
    [SerializeField] private PetCollectionUI petCollectionUI;
    [SerializeField] private PetDataUI petDataUI;

    
    private void Awake() {
        petCollectionUI.petUI = this;
        petDataUI.petUI = this;
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(()=>ui.SetActive(false));
    }
    private void OpenUI()
    {
        OpenPetCollectionUI(0);
        ui.SetActive(true);
    }
    private void OpenPetCollectionUI(int page)
    {
        petDataUI.gameObject.SetActive(false);
        petCollectionUI.gameObject.SetActive(true);
        petCollectionUI.BindData(page);
    }
    public void OpenPetDataUI(int page,int index)
    {
        petCollectionUI.gameObject.SetActive(false);
        petDataUI.gameObject.SetActive(true);
        petDataUI.OpenPetData(page,index);
    }
    public void ClosePetDataUI()
    {
        petDataUI.gameObject.SetActive(false);
        petCollectionUI.gameObject.SetActive(true);
    }
    

}
