using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MagicBookUI : MonoBehaviour
{
    [SerializeField] private CustomButton exitButton;
    [SerializeField] GameObject ui;
    [SerializeField] MainSketchManger mainSketchManger;
    [SerializeField] private TEXDraw3D problemText;
    [SerializeField] private CustomButton[] moveButton;
    [SerializeField] private GameObject sketchUI;
    [SerializeField] private GameObject warningText;
    [SerializeField] private Material gageMaterial;
    [SerializeField] private CustomButton crystalPopupExitButton;
    [SerializeField] private GameObject crystalPopup;
    [SerializeField] private CustomButton gageButton;

    private int problemIndex;
    private string curProblem;
    private string curAnswer;
    private int gageAmount;
    private Vector3 startMousePosition;
    private bool onSwipe;
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(() => ui.gameObject.SetActive(false));
        crystalPopupExitButton.BindClickEvent(()=>crystalPopup.SetActive(false));
        gageButton.BindClickEvent(EarnCrystal);
        moveButton[0].BindClickEvent(()=>SetProblem(problemIndex - 1));
        moveButton[1].BindClickEvent(()=>SetProblem(problemIndex + 1));
        mainSketchManger.Init();
    }
    private void Update() {
        Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
        if (Input.GetMouseButtonDown(0))
        {
            if (-7.72f <= mousePositionInWorld.x && mousePositionInWorld.x < 7.4f && -12.0f <= mousePositionInWorld.y && mousePositionInWorld.y <= 12.45f)
            {
                startMousePosition = mousePositionInWorld;
                onSwipe = true;
            }
        }
        if (onSwipe && Input.GetMouseButton(0))
        {
            if (startMousePosition.x - mousePositionInWorld.x > 8.0f)
            {
                if(problemIndex != SaveManager.Instance.GetMagicBookLength()-1)SetProblem(problemIndex + 1);
                onSwipe = false;
            }
            if (startMousePosition.x - mousePositionInWorld.x < -8.0f)
            {
                if(problemIndex != 0)SetProblem(problemIndex - 1);
                onSwipe = false;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            onSwipe = false;
        }
    }
    public void Dispose()
    {
        mainSketchManger.Dispose();
    }
    private void OpenUI()
    {
        gageAmount = SaveManager.Instance.GetGageData();
        ui.gameObject.SetActive(true);
        sketchUI.SetActive(true);
        warningText.SetActive(false);
        mainSketchManger.Open();
        ShowGage();
        SetProblem(0);
    }
    private void SetProblem(int index)
    {
        problemIndex = Mathf.Min(index,SaveManager.Instance.GetMagicBookLength() - 1);
        if(problemIndex < 0)
        {
            SetEmpty();
            return;
        }
        ShowButton();
        curProblem = SaveManager.Instance.GetProblemData(problemIndex);
        curAnswer = SaveManager.Instance.GetAnswerData(problemIndex);
        problemText.text = curProblem;
        mainSketchManger.SetDrawType(curAnswer.Contains("frac"));
        mainSketchManger.ShowDrawingBoxByType();
    }
    public void ShowButton()
    {
        if (problemIndex != 0)
            moveButton[0].gameObject.SetActive(true);
        else
            moveButton[0].gameObject.SetActive(false);
        if (problemIndex != SaveManager.Instance.GetMagicBookLength() - 1)
            moveButton[1].gameObject.SetActive(true);
        else
            moveButton[1].gameObject.SetActive(false);
    }
    public void HideButton()
    {
        moveButton[0].gameObject.SetActive(false);
        moveButton[1].gameObject.SetActive(false);
    }  
    private void SetEmpty()
    {
        sketchUI.SetActive(false);
        warningText.SetActive(true);
    }
    private void RemoveProblem(int index)
    {
        SaveManager.Instance.RemoveMagicBookData(index);
        SaveManager.Instance.SaveMagicBookData();
        SetProblem(index);
    }
    private void ShowGage()
    {
        gageMaterial.SetFloat("_FillAmount",(float)gageAmount / 10);
        if(gageAmount >= 10)
            gageMaterial.SetFloat("_isHighlighted",1);
        else
            gageMaterial.SetFloat("_isHighlighted",0);
    }
    public void CorrectProblem()
    {
        StartCoroutine(IncreaseGage());
        RemoveProblem(problemIndex);
    }
    private void EarnCrystal()
    {
        if(gageAmount < 10) return;
        gageAmount = 0;
        gageMaterial.SetFloat("_FillAmount", (float)gageAmount / 10);
        gageMaterial.SetFloat("_isHighlighted", 0);
        SaveManager.Instance.SetGageData(0);
        SaveManager.Instance.SetCrystalData(SaveManager.Instance.GetCrystalData() + 500);
        SaveManager.Instance.SaveMagicBookData();
        SaveManager.Instance.SaveData();
        crystalPopup.SetActive(true);
    }
    public IEnumerator IncreaseGage()
    {
        gageAmount++;
        SaveManager.Instance.SetGageData(gageAmount);
        SaveManager.Instance.SaveMagicBookData();
        float start = ((float)gageAmount - 1) / 10;
        float end = ((float)gageAmount)/ 10;
        float time = 0;
        float t = 0.5f;
        while(time < 1)
        {
            time += Time.deltaTime / t;
            gageMaterial.SetFloat("_FillAmount", Mathf.Lerp(start,end,time));
            yield return null;
        }
        ShowGage();
    }
    public bool CheckAnswer(string answer)
    {
        return (answer == curAnswer);
    }
}
