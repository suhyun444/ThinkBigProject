using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HelpUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private CustomButton exitButton;
    [SerializeField] private CustomButton[] moveButton;
    [SerializeField] private TEXDraw3D[] problemText;
    [SerializeField] private string[] problems;
    [SerializeField] private GameObject[] infos;
    private int index;
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(CloseUI);
        moveButton[0].BindClickEvent(()=>BindPage(index - 1));
        moveButton[1].BindClickEvent(()=>BindPage(index + 1));
    }
    private void OpenUI()
    {
        Debug.Log("Close");
        BattleTutorial.Instance.Close();
        Time.timeScale = 0;
        ui.SetActive(true);
        BindPage(0);
    }
    private void CloseUI()
    {
        Time.timeScale = 1.0f;
        ui.SetActive(false);
        BattleTutorial.Instance.NextPage(0);
    }
    private void BindPage(int page)
    {
        index = page;
        ShowButton();
        problemText[0].text = problems[page*2];
        problemText[1].text = problems[page*2 + 1];
        for(int i=0;i<infos.Length;++i)infos[i].SetActive(false);
        infos[index].SetActive(true);

    }
    private void ShowButton()
    {
        if(index != 0)
            moveButton[0].gameObject.SetActive(true);
        else
            moveButton[0].gameObject.SetActive(false);
        if(index != infos.Length - 1)
            moveButton[1].gameObject.SetActive(true);
        else
            moveButton[1].gameObject.SetActive(false);
    }
}
