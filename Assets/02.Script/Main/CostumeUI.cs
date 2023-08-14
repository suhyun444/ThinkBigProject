using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CostumeUI : MonoBehaviour
{
    [SerializeField] private MainManager mainManager;
    [SerializeField] private Transform bigCostumeParent;
    [SerializeField] private SpriteRenderer mainCostume;
    [SerializeField] private SpriteRenderer subCostume;
    [SerializeField] private Transform smallCostumeParent;
    [SerializeField] private SpriteRenderer[] miniatureCostume;
    [SerializeField] private SpriteRenderer miniatureSubCostume;
    [SerializeField] private CostumeData[] costumeDatas;
    [SerializeField] CustomButton exitButton;
    [SerializeField] CustomButton[] moveButton;
    [SerializeField] CustomButton checkButton;
    [SerializeField] private SortPosition costText;
    [SerializeField] private GameObject selectText;
    [SerializeField] private GameObject warningText;
    [SerializeField] GameObject ui;
    
    private int index = 0;
    private float waringTime = 0.0f;
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(() => ui.SetActive(false));
        exitButton.AddClickEvent(() => Tutorial.Instance.NextPage(1));
        moveButton[0].BindClickEvent(()=>StartCoroutine(MoveLeft()));
        moveButton[1].BindClickEvent(()=>StartCoroutine(MoveRight()));
    }
    private void Update() {
        SetSubCostumeSize();
        waringTime -= Time.deltaTime;
        if(waringTime < 0.0f)
        {
            warningText.SetActive(false);
        }
    }
    private void SetSubCostumeSize()
    {
        float scaleAmount;
        float size;
        for (int i = 0; i < 3; i++)
        {
            scaleAmount = Mathf.Abs(miniatureCostume[i].transform.position.x) / 7.0f;
            size = Mathf.Lerp(0.3f, 0, scaleAmount);
            miniatureCostume[i].transform.localScale = new Vector3(size, size, 1);
        }
        scaleAmount = Mathf.Abs(miniatureSubCostume.transform.position.x) / 7.0f;
        size = Mathf.Lerp(0.3f, 0, scaleAmount);
        miniatureSubCostume.transform.localScale = new Vector3(size, size, 1);
    }
    private void OpenUI()
    {
        Tutorial.Instance.Close();
        warningText.SetActive(false);
        index = 0;
        ui.SetActive(true);
        Show();
    }
    private void Show()
    {
        subCostume.gameObject.SetActive(false);
        miniatureSubCostume.gameObject.SetActive(false);
        bigCostumeParent.localPosition = Vector3.zero;
        mainCostume.sprite = costumeDatas[index].sprite;
        smallCostumeParent.localPosition = Vector3.zero;
        SetSubCostumeSize();
        miniatureCostume[0].sprite = (index != 0) ? costumeDatas[index - 1].sprite : null;
        miniatureCostume[1].sprite = costumeDatas[index].sprite;
        miniatureCostume[2].sprite = (index != costumeDatas.Length - 1) ? costumeDatas[index + 1].sprite : null;
        ShowButton();
        SetCheckButton();
    }
    private void HideButton()
    {
        moveButton[0].gameObject.SetActive(false);
        moveButton[1].gameObject.SetActive(false);
        checkButton.gameObject.SetActive(false);
    }
    private void ShowButton()
    {
        if(index != 0)
            moveButton[0].gameObject.SetActive(true);
        else
            moveButton[0].gameObject.SetActive(false);
        if(index != costumeDatas.Length - 1)
            moveButton[1].gameObject.SetActive(true);
        else
            moveButton[1].gameObject.SetActive(false);
        checkButton.gameObject.SetActive(true);
    }
    private void SetCheckButton()
    {
        if((int)SaveManager.Instance.GetCostumeTypeData() == index)
        {
            checkButton.gameObject.SetActive(false);
        }
        if(SaveManager.Instance.GetHavingCostumeList().Contains(index))
        {
            selectText.SetActive(true);
            costText.transform.parent.gameObject.SetActive(false);
            checkButton.BindClickEvent(SelectMethod);
        }
        else
        {
            costText.SetText("x" + costumeDatas[index].cost.ToString());
            selectText.SetActive(false);
            costText.transform.parent.gameObject.SetActive(true);
            checkButton.BindClickEvent(BuyMethod);
        }
    }
    private void SelectMethod()
    {
        SoundManager.Instance.PlaySoundEffect(Sound.ButtonClick);
        SaveManager.Instance.SetCostumeTypeData(costumeDatas[index].type);
        SaveManager.Instance.SaveData();
        mainManager.ChangePlayerCostime(costumeDatas[index].type);
        SetCheckButton();
    }
    private void BuyMethod()
    {
        int curCrystal = SaveManager.Instance.GetCrystalData();
        if(curCrystal < costumeDatas[index].cost)
        {
            SoundManager.Instance.PlaySoundEffect(Sound.Warning);
            waringTime = 0.7f;
            warningText.SetActive(true);
            return;
        }
        SoundManager.Instance.PlaySoundEffect(Sound.Buy);
        curCrystal -= costumeDatas[index].cost;
        SaveManager.Instance.AddHavingCostumeList(index);
        SaveManager.Instance.SaveData();
        SetCheckButton();
    }
    private IEnumerator MoveRight()
    {
        HideButton();
        SetSubCostume(1);
        Vector3 end = new Vector3(-1.3f,0,0);
        Vector3 miniatureEnd = new Vector3(-0.3f, 0, 0);
        yield return StartCoroutine(MoveCostumeParent(end, miniatureEnd));
        index++;
        Show();
    }
    private IEnumerator MoveLeft()
    {
        HideButton();
        SetSubCostume(-1);
        Vector3 end = new Vector3(1.3f, 0, 0);
        Vector3 miniatureEnd = new Vector3(0.3f, 0, 0);
        yield return StartCoroutine(MoveCostumeParent(end,miniatureEnd));
        index--;
        Show();
    }
    private IEnumerator MoveCostumeParent(Vector3 end,Vector3 miniatureEnd)
    {
        float time = 0;
        float t = 0.5f;
        while (time < 1)
        {
            time += Time.deltaTime / t;
            bigCostumeParent.localPosition = Vector3.Lerp(Vector3.zero, end, time);
            smallCostumeParent.localPosition = Vector3.Lerp(Vector3.zero, miniatureEnd, time);
            yield return null;
        }
    }
    private void SetSubCostume(int direction)
    {
        subCostume.sprite = costumeDatas[index + (1*direction)].sprite;
        subCostume.transform.localPosition = new Vector3(1.3f * direction, -0.086f, 0);
        subCostume.gameObject.SetActive(true);
        miniatureSubCostume.sprite = (0 <= index + (2*direction) && index + (2 * direction) < costumeDatas.Length) ? costumeDatas[index + (2*direction)].sprite : null;
        miniatureSubCostume.transform.localPosition = new Vector3(0.6f * direction, 0.68f, 0);
        miniatureSubCostume.gameObject.SetActive(true);
    }

}
