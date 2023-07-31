using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CustomUtils;

public enum SkillType
{
    Stamina,
    Defense,
    Combo,
    Crystal,
    Speed,
    Kill,
    Exp,
    End
}
public class SkillInformation : MonoBehaviour
{
    [SerializeField] private SkillType skillType;
    [TextArea][SerializeField] private string description;
    [TextArea][SerializeField] private string engDescription;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro descriptionText;
    [SerializeField] private TextMeshPro levelText;
    [SerializeField] private PressingButton upgradeButton;
    public void Init()
    {
        upgradeButton.BindClickEvent(UpgradeSkill);
        BindInformation();
        nameText.fontMaterial.SetFloat("_Stencil", 1);
        nameText.fontMaterial.SetFloat("_StencilComp", 3);
        descriptionText.fontMaterial.SetFloat("_Stencil", 1);
        descriptionText.fontMaterial.SetFloat("_StencilComp", 3);
        levelText.fontMaterial.SetFloat("_Stencil", 1);
        levelText.fontMaterial.SetFloat("_StencilComp", 3);
    }
    public bool CheckHasSkillPoint()
    {
        return (((SaveManager.Instance.GetLevelData() + 1)* 5) - SaveManager.Instance.GetUsedSkillPoint()) > 0;
    }
    public void UpgradeSkill()
    {
        if(!CheckHasSkillPoint())return;
        SaveManager.Instance.UseSkillPoint(skillType);
        SaveManager.Instance.SaveData();
        BindInformation();
    }
    public void BindInformation()
    {
        string curDescription = (LanguageManager.Instance.languageType == LanguageType.Korean) ? description : engDescription;
        descriptionText.text = curDescription.Replace("@",Util.SplitRemainZero((Const.Skill.EFFECT_INCREASE_AMOUNT[(int)skillType] * SaveManager.Instance.GetSkillLevel(skillType)).ToString("F2")) + "%");
        int skillLevel = SaveManager.Instance.GetSkillLevel(skillType);
        if(skillLevel == 100)
        {
            levelText.text = "MAX";
            upgradeButton.gameObject.SetActive(false);
        }
        else
        {
            levelText.text = skillLevel.ToString();
            upgradeButton.gameObject.SetActive(true);
        }
    }
    public void AddUpgradeButtonEvent(PressingButton.ButtonClickFunc clickEvent)
    {
        upgradeButton.AddClickEvent(clickEvent);
    }
}