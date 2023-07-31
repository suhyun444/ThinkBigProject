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
    [SerializeField] private GameObject resetPopup;
    [SerializeField] private CustomButton resetPopupAcceptButton;
    [SerializeField] private CustomButton resetPopupExitButton;
    [SerializeField] private GameObject warningText;

    [SerializeField] private GameObject buttonParents;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] private float dragSensitivy;
    private Vector3 prevMousePosition;
    private bool onDrag = false;
    private float warningTime = 0.0f;
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(()=>skillUI.SetActive(false));
        for(int i=0;i<skillInformations.Length;++i)
        {
            skillInformations[i].Init();
            skillInformations[i].AddUpgradeButtonEvent(UpdateSkillPointText);
        }
        resetButton.BindClickEvent(()=>resetPopup.SetActive(true));
        resetPopupExitButton.BindClickEvent(()=>resetPopup.SetActive(false));
        resetPopupAcceptButton.BindClickEvent(ResetSkillPoint);
    }
    void Update()
    {
        warningTime -= Time.deltaTime;
        if(warningTime < 0.0f)warningText.SetActive(false);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f));
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(mousePosition, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("SkillSlider"))
            {
                prevMousePosition = mousePosition;
                onDrag = true;
            }
        }
        else if (onDrag && Input.GetMouseButton(0))
        {
            if (Mathf.Abs(prevMousePosition.y - mousePosition.y) > 0.1f)
            {
                float nextY = buttonParents.transform.localPosition.y + (mousePosition.y - prevMousePosition.y) * dragSensitivy;
                nextY = Mathf.Clamp(nextY, minY, maxY);
                buttonParents.transform.localPosition = new Vector3(buttonParents.transform.localPosition.x, nextY, 0.0f);
                prevMousePosition = mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            onDrag = false;
        }
    }
    private void OpenUI()
    {
        warningText.SetActive(false);
        skillUI.SetActive(true);
        levelText.text = "Lv."+SaveManager.Instance.GetLevelData().ToString();
        UpdateSkillPointText();
        for (int i = 0; i < skillInformations.Length; ++i)
            skillInformations[i].BindInformation();
    }
    private void UpdateSkillPointText()
    {
        skillPointText.text =  "SP : "+ (((SaveManager.Instance.GetLevelData() + 1) * 5) - SaveManager.Instance.GetUsedSkillPoint()).ToString();
    }
    private void ResetSkillPoint()
    {
        if(SaveManager.Instance.GetCrystalData() < Const.Skill.RESET_COST)
        {
            warningTime = 0.7f;
            warningText.SetActive(true);
            return;
        }
        SaveManager.Instance.SetCrystalData(SaveManager.Instance.GetCrystalData() - 1000);
        SaveManager.Instance.ResetSkillPoint();
        SaveManager.Instance.SaveData();
        UpdateSkillPointText();
        for(int i=0;i<skillInformations.Length;++i)
            skillInformations[i].BindInformation();
        resetPopup.SetActive(false);
    }
}
