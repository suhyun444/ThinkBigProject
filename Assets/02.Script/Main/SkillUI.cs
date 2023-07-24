using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SkillUI : MonoBehaviour
{
    [SerializeField] private GameObject skillUI;
    [SerializeField] private CustomButton exitButton;
    [SerializeField] private SkillInformation[] skillInformations;
    [SerializeField] private TextMeshPro levelText;
    [SerializeField] private TextMeshPro skillPointText;
    [SerializeField] private CustomButton resetButton;
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(()=>skillUI.SetActive(false));
        for(int i=0;i<skillInformations.Length;++i)
        {
            skillInformations[i].Init();
            skillInformations[i].AddUpgradeButtonEvent(UpdateSkillPointText);
        }
        resetButton.BindClickEvent(ResetSkillPoint);
    }
    private void OpenUI()
    {
        skillUI.SetActive(true);
        levelText.text = "Lv."+SaveManager.Instance.GetLevelData().ToString();
        UpdateSkillPointText();
    }
    private void UpdateSkillPointText()
    {
        skillPointText.text = ((LanguageManager.Instance.languageType == LanguageType.Korean) ?  "보유 SP : " : "Remain SP : ")+ (SaveManager.Instance.GetLevelData() - SaveManager.Instance.GetUsedSkillPoint()).ToString();
    }
    private void ResetSkillPoint()
    {
        SaveManager.Instance.ResetSkillPoint();
        SaveManager.Instance.SaveData();
        UpdateSkillPointText();
        for(int i=0;i<skillInformations.Length;++i)
            skillInformations[i].BindInformation();
    }
}
