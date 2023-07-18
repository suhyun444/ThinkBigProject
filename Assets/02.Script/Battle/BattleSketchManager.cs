using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSketchManager : MonoBehaviour
{
    [SerializeField] private QuestionBox questionBox;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Material drawingBookMaterial;
    [SerializeField] private Material drawingMaterial;


    public bool isEnd = false;
    private MouseSketch mouseSketch;
    private void Awake() {
        mouseSketch = GetComponent<MouseSketch>();
        mouseSketch.Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        drawingMaterial.SetFloat("_HighLightedAmount", 1);
        drawingMaterial.SetFloat("_Alpha", 1);
        drawingBookMaterial.SetFloat("_OutLineAlpha", 0);
        questionBox.BindShowProblemCallBack(() => mouseSketch.ShowDrawingBoxByType());
        questionBox.BindShowProblemCallBack(()=>mouseSketch.onAnimation = false);
    }
    public void SetDrawType(bool isFrac)
    {
        mouseSketch.SetDrawType(isFrac);
    }
    // Update is called once per frame
    void Update()
    {
        if (isEnd) return;
        if (mouseSketch.onAnimation) return;
        mouseSketch.nonDrawingTime += Time.deltaTime;
        string predicteValue = "";
        if (!mouseSketch.isOnFracDrawing) predicteValue = mouseSketch.DrawDefault();
        else predicteValue = mouseSketch.DrawFrac();
        if (mouseSketch.nonDrawingTime > 1.0f)
        {
            mouseSketch.nonDrawingTime = -100000.0f;
            if (battleManager.CheckAnswer(predicteValue))
            {
                StartCoroutine(RuneEngrave());
            }
            else
            {
                mouseSketch.ShowDrawingBoxByType();
                mouseSketch.EraseSketch();
                mouseSketch.onAnimation = true;
            }
        }
    }
    public IEnumerator RuneEngrave()
    {
        mouseSketch.onAnimation = true;
        float time = 0;
        float t = 0.35f;
        while (time < 1)
        {
            time += Time.deltaTime / t;
            drawingMaterial.SetFloat("_HighLightedAmount", Mathf.Lerp(1, 0.4f, time));
            drawingBookMaterial.SetFloat("_OutLineAlpha", Mathf.Lerp(0, 1, time));
            yield return null;
        }
        yield return new WaitForSeconds(0.4f);
        time = 0;
        t = 0.7f;
        while (time < 1)
        {
            time += Time.deltaTime / t;
            drawingMaterial.SetFloat("_Alpha", Mathf.Lerp(1, 0, time));
            drawingBookMaterial.SetFloat("_OutLineAlpha", Mathf.Lerp(1, 0, time));
            yield return null;
        }
        drawingMaterial.SetFloat("_Alpha", 1);
        drawingMaterial.SetFloat("_HighLightedAmount", 1);
        mouseSketch.EraseSketch();
        mouseSketch.ShowDrawingBoxByType();
    }
    public void Dispose()
    {
        mouseSketch.Dispose();
    }
}
