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
    [SerializeField] private GameObject[] gageObjects;
    [SerializeField] private GameObject sketchUI;
    [SerializeField] private GameObject warningText;

    private int problemIndex;
    private string curProblem;
    private string curAnswer;
    private int gageAmount;
    private Vector3 startMousePosition;
    private bool onSwipe;
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(() => ui.gameObject.SetActive(false));
        moveButton[0].BindClickEvent(()=>SetProblem(problemIndex - 1));
        moveButton[1].BindClickEvent(()=>SetProblem(problemIndex + 1));
        mainSketchManger.Init();
    }
    private void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
            if (-7.72f <= mousePositionInWorld.x && mousePositionInWorld.x < 7.4f && -12.0f <= mousePositionInWorld.y && mousePositionInWorld.y <= 12.45f)
            {
                startMousePosition = Input.mousePosition;
                onSwipe = true;
            }
        }
        if (onSwipe && Input.GetMouseButton(0))
        {
            if (startMousePosition.x - Input.mousePosition.x > 600.0f)
            {
                if(problemIndex != SaveManager.Instance.GetMagicBookLength()-1)SetProblem(problemIndex + 1);
                onSwipe = false;
            }
            if (startMousePosition.x - Input.mousePosition.x < -600.0f)
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
    private void ShowGage()
    {
        for(int i=0;i<gageObjects.Length;i++)
        {
            if(i < gageAmount)
                gageObjects[i].SetActive(true);
            else
                gageObjects[i].SetActive(false);
        }
    }
    private void IncreaseGage()
    {
        gageAmount++;
        SaveManager.Instance.SetGageData(gageAmount);
        SaveManager.Instance.SaveMagicBookData();
        ShowGage();
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
        problemIndex = index;
        problemIndex = Mathf.Min(problemIndex,SaveManager.Instance.GetMagicBookLength() - 1);
        if(problemIndex < 0)
        {
            SetEmpty();
            return;
        }
        ShowButton();
        curProblem = SaveManager.Instance.GetProblemData(index);
        curAnswer = SaveManager.Instance.GetAnswerData(index);
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
    public void CorrectProblem()
    {
        IncreaseGage();
        RemoveProblem(problemIndex);
    }
    public bool CheckAnswer(string answer)
    {
        return (answer == curAnswer);
    }
}
