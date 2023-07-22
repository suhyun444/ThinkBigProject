using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumeUI : MonoBehaviour
{
    [SerializeField] private Transform bigCostumeParent;
    [SerializeField] private SpriteRenderer mainCostume;
    [SerializeField] private SpriteRenderer subCostume;
    [SerializeField] private Transform smallCostumeParent;
    [SerializeField] private SpriteRenderer[] miniatureCostume;
    [SerializeField] private SpriteRenderer miniatureSubCostume;
    [SerializeField] private CostumeData[] costumeDatas;
    [SerializeField] CustomButton exitButton;
    [SerializeField] CustomButton[] moveButton;
    [SerializeField] CustomButton buyButton;
    [SerializeField] GameObject ui;
    private int index = 0;
    
    private void Awake() {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(() => ui.SetActive(false));
        moveButton[0].BindClickEvent(()=>StartCoroutine(MoveLeft()));
        moveButton[1].BindClickEvent(()=>StartCoroutine(MoveRight()));
    }
    private void Update() {
        SetSubCostumeSize();
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
    }
    private void HideButton()
    {
        moveButton[0].gameObject.SetActive(false);
        moveButton[1].gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
    }
    private void ShowButton()
    {
        if(index != 0)
            moveButton[0].gameObject.SetActive(true);
        if(index != costumeDatas.Length - 1)
            moveButton[1].gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true);

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
