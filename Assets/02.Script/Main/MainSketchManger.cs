using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSketchManger : MonoBehaviour
{
    [SerializeField] private Material drawingBookMaterial;
    [SerializeField] private Material drawingMaterial;
    [SerializeField] private MagicBookUI magicBookUI;
    [SerializeField] private Transform drawingCanvas;

    private MouseSketch mouseSketch;
    public void Init()
    {
        mouseSketch = GetComponent<MouseSketch>();
        mouseSketch.Init();
        mouseSketch.SetFracSpaceHelper(drawingCanvas.position);
    }
    public void Open()
    {
        drawingMaterial.SetFloat("_HighLightedAmount", 1);
        drawingMaterial.SetFloat("_Alpha", 1);
        drawingBookMaterial.SetFloat("_OutLineAlpha", 0);
    }
    public void SetDrawType(bool isFrac)
    {
        mouseSketch.SetDrawType(isFrac);
    }
    // Update is called once per frame
    void Update()
    {
        if (mouseSketch.onAnimation) return;
        if(mouseSketch.nonDrawingTime >= 0.0f)magicBookUI.HideButton();
        else magicBookUI.ShowButton();
        mouseSketch.nonDrawingTime += Time.deltaTime;
        string predicteValue = "";
        if (!mouseSketch.isOnFracDrawing) predicteValue = mouseSketch.DrawDefault();
        else predicteValue = mouseSketch.DrawFrac();
        if (mouseSketch.nonDrawingTime > 1.0f)
        {
            mouseSketch.nonDrawingTime = -100000.0f;
            if (magicBookUI.CheckAnswer(predicteValue))
            {
                StartCoroutine(RuneEngrave());
            }
            else
            {
                mouseSketch.EraseSketch();
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            mouseSketch.EraseSketch();
        }
    }
    public void ShowDrawingBoxByType()
    {
        mouseSketch.ShowDrawingBoxByType();
    }
    public void Dispose()
    {
        if(mouseSketch != null)
            mouseSketch.Dispose();
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
        ShowDrawingBoxByType();
        magicBookUI.CorrectProblem();
    }
}
