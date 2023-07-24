using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private GameObject optionUI;
    [SerializeField] private CustomButton exitButton;
    [SerializeField] private CustomButton acceptButton;
    [SerializeField] private SoundSlider soundSlider;
    [SerializeField] private CustomButton languageButton;
    [SerializeField] private TextMeshPro curLanguage;
    [SerializeField] private GameObject comboBox;
    [SerializeField] private CustomButton closeComboBoxButton;
    [SerializeField] private CustomButton[] selectLanguageButton;
    
    private LanguageType languageType;
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(()=>optionUI.SetActive(false));
        acceptButton.BindClickEvent(Save);
        languageButton.BindClickEvent(()=>SetActiveComboBox(true));
        languageButton.AddClickEvent(()=>SettingComboBox());
        closeComboBoxButton.BindClickEvent(()=>SetActiveComboBox(false));
        selectLanguageButton[0].BindClickEvent(()=>SelectLanguage(LanguageType.Korean));
        selectLanguageButton[1].BindClickEvent(()=>SelectLanguage(LanguageType.English));
    }
    private void OpenUI()
    {
        soundSlider.Open(SaveManager.Instance.GetVolumeData());
        SelectLanguage(SaveManager.Instance.GetLanguageTypeData());
        optionUI.SetActive(true);
    }
    private void SetActiveComboBox(bool isActive)
    {
        languageButton.gameObject.SetActive(!isActive);
        comboBox.gameObject.SetActive(isActive);
    }
    private void SettingComboBox()
    {
        int mainLanguage = (int)languageType;
        int subLanguage = ((mainLanguage == 0) ? 1 : 0);
        selectLanguageButton[mainLanguage].transform.localPosition = new Vector3(selectLanguageButton[mainLanguage].transform.localPosition.x,0.3706f,-1.0f); 
        selectLanguageButton[subLanguage].transform.localPosition = new Vector3(selectLanguageButton[subLanguage].transform.localPosition.x,0.239f,-1.0f); 
    }
    private void SelectLanguage(LanguageType type)
    {
        languageType = type;
        curLanguage.text = (languageType == LanguageType.Korean) ? "한국어" : "English";
        SettingComboBox();
        SetActiveComboBox(false);
    }
    private void Save()
    {
        SaveManager.Instance.SetVolumeData(soundSlider.GetVolume());
        //AudioManager에 볼륨 저장
        SaveManager.Instance.SetLanguagueTypeData(languageType);
        //LanguageManager에서 언어 바꿔주기
        SaveManager.Instance.SaveOptionData();
        optionUI.SetActive(false);
    }
}
