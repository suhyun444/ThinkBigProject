using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct TutorialPage
{
    public List<SpriteRenderer> spriteRenderers;
    public List<TextMeshPro> texts;
    public void SetHighlighted()
    {
        for(int i=0;i<spriteRenderers.Count;++i)
            spriteRenderers[i].sortingOrder = 51;
        for(int i=0;i<texts.Count;++i)
            texts[i].sortingOrder = 52;
    }
    public void SetDefault()
    {
        for (int i = 0; i < spriteRenderers.Count; ++i)
            spriteRenderers[i].sortingOrder = i;
        for (int i = 0; i < texts.Count; ++i)
            texts[i].sortingOrder = 1;
    }
}
public class Tutorial : Singleton<Tutorial>
{
    [SerializeField] private List<TutorialPage> tutorials;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject[] infoTexts;
    private int curPage;
    private void Awake() {
        curPage = -1;
        if(!SaveManager.Instance.GetEndTutorialData())
        {
            OpenPage(0);
        }
    }
    private void OpenPage(int page)
    {
        if(page == tutorials.Count)
        {
            EndTutorial();
            return;
        }
        curPage = page;
        ui.SetActive(true);
        infoTexts[page].SetActive(true);
        tutorials[page].SetHighlighted();
    }
    public void Close()
    {
        if(curPage >= 0)
        {
            ui.SetActive(false);
            infoTexts[curPage].SetActive(false);
            tutorials[curPage].SetDefault();
        }
    }
    public void NextPage(int page)
    {
        if(curPage == page)
        {
            OpenPage(curPage + 1);
        }
    }
    private void EndTutorial()
    {
        SaveManager.Instance.SetEndTutorialData(true);
        SaveManager.Instance.SaveOptionData();
    }
}
