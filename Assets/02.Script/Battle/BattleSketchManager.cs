using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSketchManager : MonoBehaviour
{
    [SerializeField] private QuestionBox questionBox;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Material drawingBookMaterial;
    [SerializeField] private Material drawingMaterial;
    [SerializeField] private Transform drawingCanvas;
    [SerializeField] private LineRenderer fracHorizontalLine;
    [SerializeField] private LineRenderer fracVerticalLine;


    public bool isEnd = false;
    private MouseSketch mouseSketch;
    private void Awake() {
        mouseSketch = GetComponent<MouseSketch>();
        mouseSketch.Init();
        questionBox.BindShowProblemCallBack(() => mouseSketch.ShowDrawingBoxByType());
        questionBox.BindShowProblemCallBack(()=>mouseSketch.onAnimation = false);
        drawingMaterial.SetFloat("_HighLightedAmount", 1);
        drawingMaterial.SetFloat("_Alpha", 1);
        drawingBookMaterial.SetFloat("_OutLineAlpha", 0);
        InitFracLine();
    }
    private void InitFracLine()
    {
        mouseSketch.SetFracSpaceHelper(drawingCanvas.position);
        float ratio = (drawingCanvas.lossyScale.x - 11) / (14.07f - 11);
        fracVerticalLine.SetPosition(0,new Vector3(drawingCanvas.position.x,drawingCanvas.position.y - Mathf.Lerp(2.25f,2.85f,ratio),0));
        fracVerticalLine.SetPosition(1,new Vector3(drawingCanvas.position.x,drawingCanvas.position.y + Mathf.Lerp(2.25f,2.85f,ratio),0));
        float centerX = Mathf.Lerp(-1.0f,-6.1f,ratio);
        fracHorizontalLine.SetPosition(0, new Vector3(centerX - Mathf.Lerp(1.8f,2.5f,ratio), drawingCanvas.position.y, 0));
        fracHorizontalLine.SetPosition(1, new Vector3(centerX + Mathf.Lerp(1.8f, 2.5f, ratio),drawingCanvas.position.y, 0));
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
        if(mouseSketch.nonDrawingTime >= 0.0f)
        {
            BattleTutorial.Instance.Close();
            BattleTutorial.Instance.NextPage(1);
        }
        if (mouseSketch.nonDrawingTime > 1.0f)
        {
            mouseSketch.nonDrawingTime = -100000.0f;
            if (battleManager.CheckAnswer(predicteValue))
            {
                SoundManager.Instance.PlaySoundEffect(Sound.MagicSuccess);
                StartCoroutine(RuneEngrave());
            }
            else
            {
                SoundManager.Instance.PlaySoundEffect(Sound.MagicFailed);
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
        mouseSketch.onAnimation = false;
        mouseSketch.EraseSketch();
        mouseSketch.ShowDrawingBoxByType();
    }
    public void Dispose()
    {
        mouseSketch.Dispose();
    }
}
