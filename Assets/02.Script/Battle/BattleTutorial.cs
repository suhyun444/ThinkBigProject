using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct BattleTutorialPage
{
    public List<SpriteRenderer> spriteRenderers;
    public List<int> spriteOrders;
    public List<TextMeshPro> texts;
    public List<int> textOrders;
    public List<Material> materials;
    public TEXDraw3D texDraw;
    public float blurSortingOrder;
    public void SetHighlighted()
    {
        for (int i = 0; i < spriteRenderers.Count; ++i)
            spriteRenderers[i].sortingOrder = 51 + i;
        for (int i = 0; i < texts.Count; ++i)
            texts[i].sortingOrder = 61 + i;
        for(int i=0;i<materials.Count;++i)
        {
            materials[i].SetFloat("_SortingOrder",61);
            texDraw.UpdateSortingOrder();
        }
    }
    public void SetDefault()
    {
        for (int i = 0; i < spriteRenderers.Count; ++i)
            spriteRenderers[i].sortingOrder = spriteOrders[i];
        for (int i = 0; i < texts.Count; ++i)
            texts[i].sortingOrder = textOrders[i];
        for (int i = 0; i < materials.Count; ++i)
        {
            materials[i].SetFloat("_SortingOrder", 0);
            texDraw.UpdateSortingOrder();
        }
    }
}
public class BattleTutorial : Singleton<BattleTutorial>
{
    [SerializeField] private List<BattleTutorialPage> tutorials;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject[] infoTexts;
    private int curPage;
    public bool isTutorialEnd = true;
    private void Awake()
    {
        curPage = -1;
        StartTutorial();
    }
    public void StartTutorial()
    {
        if (!SaveManager.Instance.GetEndBattleTutorialData())
        {
            isTutorialEnd = false;
            OpenPage(0);
        }
    }
    private void OpenPage(int page)
    {
        if (page == tutorials.Count)
        {
            EndTutorial();
            return;
        }
        curPage = page;
        ui.SetActive(true);
        ui.transform.position = new Vector3(0,0,tutorials[page].blurSortingOrder);
        infoTexts[page].SetActive(true);
        tutorials[page].SetHighlighted();
    }
    public void Close()
    {
        if (curPage >= 0)
        {
            ui.SetActive(false);
            ui.transform.position = new Vector3(0, 0, -1);
            infoTexts[curPage].SetActive(false);
            tutorials[curPage].SetDefault();
        }
    }
    public void NextPage(int page)
    {
        if (curPage == page)
        {
            OpenPage(curPage + 1);
        }
    }
    private void EndTutorial()
    {
        isTutorialEnd = true;
        SaveManager.Instance.SetEndBattleTutorialData(true);
        SaveManager.Instance.SaveOptionData();
    }
}
