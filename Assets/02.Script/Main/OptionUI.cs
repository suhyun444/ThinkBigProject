using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private GameObject optionUI;
    [SerializeField] private CustomButton exitButton;
    [SerializeField] private CustomButton resetButton;
    [SerializeField] private SoundSlider sfxSoundSlider;
    [SerializeField] private SoundSlider bgmSoundSlider;
    [SerializeField] private CustomButton languageButton;
    [SerializeField] private TextMeshPro curLanguage;
    [SerializeField] private GameObject comboBox;
    [SerializeField] private CustomButton closeComboBoxButton;
    [SerializeField] private CustomButton[] selectLanguageButton;
    [SerializeField] private CustomButton level;
    [SerializeField] private CustomButton crys;
    
    private LanguageType languageType;
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(Save);
        resetButton.BindClickEvent(TutorialAgain);
        languageButton.BindClickEvent(()=>SetActiveComboBox(true));
        languageButton.AddClickEvent(()=>SettingComboBox());
        closeComboBoxButton.BindClickEvent(()=>SetActiveComboBox(false));
        sfxSoundSlider.onValueChanged = (() => SaveManager.Instance.SetSFXVolumeData(sfxSoundSlider.GetVolume()));
        sfxSoundSlider.onValueChanged += (() => SoundManager.Instance.ChangeSFXVolume(sfxSoundSlider.GetVolume()));
        bgmSoundSlider.onValueChanged = (() => SaveManager.Instance.SetBGMVolumeData(bgmSoundSlider.GetVolume()));
        bgmSoundSlider.onValueChanged += (() => SoundManager.Instance.ChangeBGMVolue(bgmSoundSlider.GetVolume()));
        selectLanguageButton[0].BindClickEvent(()=>SelectLanguage(LanguageType.Korean));
        selectLanguageButton[1].BindClickEvent(()=>SelectLanguage(LanguageType.English));
        
        crys.BindClickEvent(()=>SaveManager.Instance.SetCrystalData(10098));
        crys.AddClickEvent(()=>SaveManager.Instance.SaveData());
        level.BindClickEvent(()=>SaveManager.Instance.SetLevelData(34));
        level.AddClickEvent(()=>SaveManager.Instance.SetExpAmountData(400));
        level.AddClickEvent(()=>SaveManager.Instance.SaveData());
    }
    private void TutorialAgain()
    {
        Tutorial.Instance.StartTutorialAgain();
        SaveManager.Instance.SaveOptionData();
        optionUI.SetActive(false);
    }
    private void OpenUI()
    {
        sfxSoundSlider.Open(SaveManager.Instance.GetSFXVolumeData());
        bgmSoundSlider.Open(SaveManager.Instance.GetBGMVolumeData());
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
        SaveManager.Instance.SetLanguagueTypeData(languageType);
        LanguageManager.Instance.ChangeLanguage(languageType);
        curLanguage.text = (languageType == LanguageType.Korean) ? "한국어" : "English";
        SettingComboBox();
        SetActiveComboBox(false);
    }
    private void Save()
    {
        SaveManager.Instance.SaveOptionData();
        optionUI.SetActive(false);
    }
}
