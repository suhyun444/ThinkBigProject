using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    }
    public void OpenPetDataUI(int index)
    {
        petCollectionUI.gameObject.SetActive(false);
        petDataUI.gameObject.SetActive(true);
        petDataUI.OpenPetData(index);
    }
    public void ClosePetDataUI()
    {
        petDataUI.gameObject.SetActive(false);
        petCollectionUI.gameObject.SetActive(true);
    }
    

}
