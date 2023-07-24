using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum SkillType
{
    Stamina,
    Defense,
    Combo,
    Crystal,
    End
}
public class SkillInformation : MonoBehaviour
{
    [SerializeField] private SkillType skillType;
    [TextArea][SerializeField] private string description;
    [TextArea][SerializeField] private string engDescription;
    [SerializeField] private TextMeshPro descriptionText;
    [SerializeField] private TextMeshPro levelText;
    [SerializeField] private CustomButton upgradeButton;
    public void Init()
    {
        upgradeButton.BindClickEvent(UpgradeSkill);
        BindInformation();
    }
    public bool CheckHasSkillPoint()
    {
        return (SaveManager.Instance.GetLevelData() - SaveManager.Instance.GetUsedSkillPoint()) > 0;
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
        descriptionText.text = curDescription.Replace("@",(Const.Skill.effectIncreaseAmount[(int)skillType] * SaveManager.Instance.GetSkillLevel(skillType)).ToString() + "%");
        levelText.text = SaveManager.Instance.GetSkillLevel(skillType).ToString();
    }
    public void AddUpgradeButtonEvent(CustomButton.ButtonClickFunc clickEvent)
    {
        upgradeButton.AddClickEvent(clickEvent);
    }
}