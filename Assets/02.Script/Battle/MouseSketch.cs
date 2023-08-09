using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

class SketchedDigit
{
    public float x;
    public int index;
    public Camera camera;
    public SketchedDigit()
    {

    }
    public SketchedDigit(float x, int index, Camera camera)
    {
        this.x = x;
        this.index = index;
        this.camera = camera;
    }
};
public class MouseSketch : MonoBehaviour
{
    [SerializeField] private TextMeshPro guessText;
    [SerializeField] private CustomButton eraseButton;
    public GameObject drawObj;
    public GameObject drawObjToCalc;
    public Transform drawParent;
    public GameObject cameraObject;
    [SerializeField] RecognizeDigitsAI ai;

    private LineRenderer curLine;  //Line which draws now
    private LineRenderer curLineToCalc;
    private int positionCount = 2;  //Initial start and end position
    private Vector3 PrevPos = Vector3.zero; // 0,0,0 position variable

    private SketchedDigit curSketchedDigit;
    private DrawingCalculator drawingCalculator;
    public float nonDrawingTime = -100000.0f;
    private int index = 0;
    private List<SketchedDigit> sketchedDigits = new List<SketchedDigit>();
    private float minX = 100000, maxX = -100000;
    private float minY = 100000, maxY = -100000;
    private bool isDrawing = false;
    public bool onAnimation = false;
    public bool isOnFracDrawing = false;
    [SerializeField] private GameObject fracHelper;
    private SketchedDigit[] sketchedDigitsOnFrac = new SketchedDigit[3];
    [SerializeField] private int drawSpaceInFrac = -1;

    [SerializeField] private Vector3 fracSpaceHelper = new Vector3(-3.35f, 12.0f);
    [SerializeField] private int lineOrderInLayer = 0;
    public void Init()
    {
        ai.Init();
        for (int i = 0; i < 3; i++) sketchedDigitsOnFrac[i] = new SketchedDigit();
        drawingCalculator = new DrawingCalculator();
        eraseButton.BindClickEvent(EraseSketch);
        eraseButton.AddClickEvent(()=>SoundManager.Instance.PlaySoundEffect(Sound.Erase));
    }
    public void SetFracSpaceHelper(Vector3 spaceHelper)
    {
        fracSpaceHelper = spaceHelper;
    }
    // Update is called once per frame
    public int CalcSpaceTypeInFrac(Vector3 position)
    {
        if (position.x < fracSpaceHelper.x)
            return 0;
        else
        {
            if (position.y > fracSpaceHelper.y)
                return 1;
            else
                return 2;
        }
    }
    public void Dispose()
    {
        ai.Dispose();
    }
    public void SetDrawType(bool isFrac)
    {
        isOnFracDrawing = isFrac;
    }
    public void ShowDrawingBoxByType()
    {
        fracHelper.SetActive(isOnFracDrawing);
    }
    public string DrawFrac()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("DrawingBox"))
            {
                isDrawing = true;
                if (drawSpaceInFrac != CalcSpaceTypeInFrac(transform.position))
                {
                    drawSpaceInFrac = CalcSpaceTypeInFrac(transform.position);
                    index++;
                    minX = 100000;
                    maxX = -100000;
                    minY = 100000;
                    maxY = -100000;
                    Camera camera = Instantiate(cameraObject, this.transform.position + new Vector3(50 * index, -58.5f, -10), Quaternion.identity).GetComponent<Camera>();
                    sketchedDigitsOnFrac[drawSpaceInFrac] = new SketchedDigit(transform.position.x, index, camera);
                }
                curLine = Instantiate(drawObj, this.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
                curLineToCalc = Instantiate(drawObjToCalc, this.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
                curLine.sortingOrder = lineOrderInLayer;
                curLine.gameObject.transform.SetParent(drawParent);
                curLineToCalc.gameObject.transform.SetParent(drawParent);

                nonDrawingTime = 0.0f;
                createLine(raycastHit2D.point, (Vector3)raycastHit2D.point + new Vector3(50 * index, -50, 0));
            }
        }
        else if (isDrawing && (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetMouseButton(0)))
        {
            nonDrawingTime = 0.0f;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("DrawingBox"))
            {
                transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
                minX = Mathf.Min(minX, transform.position.x);
                maxX = Mathf.Max(maxX, transform.position.x);
                minY = Mathf.Min(minY, transform.position.y);
                maxY = Mathf.Max(maxY, transform.position.y);
                connectLine(raycastHit2D.point, (Vector3)raycastHit2D.point + new Vector3(50 * index, -50, 0));
                sketchedDigitsOnFrac[drawSpaceInFrac].camera.transform.position = new Vector3(((minX + maxX) / 2) + (50 * index), -50.0f + ((minY + maxY) / 2), -10);
                sketchedDigitsOnFrac[drawSpaceInFrac].camera.orthographicSize = Mathf.Max(Mathf.Lerp(0, 4.87f, ((maxY - minY) + 2.5f) / 10.0f), Mathf.Lerp(0, 4.87f, ((maxX - minX) + 2.5f) / 10.0f));
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
        string guessString = "";
        string ret = "";
        if (sketchedDigitsOnFrac[0].camera != null)
        {
            drawingCalculator.BindCamera(sketchedDigitsOnFrac[0].camera);
            guessString += ai.Calc(drawingCalculator).ToString();
            ret += ai.Calc(drawingCalculator).ToString();
        }
        if (sketchedDigitsOnFrac[1].camera != null)
        {
            drawingCalculator.BindCamera(sketchedDigitsOnFrac[1].camera);
            guessString += " + " + ai.Calc(drawingCalculator).ToString() + " / ";
            ret += "\\frac{" + ai.Calc(drawingCalculator).ToString() + "}";
        }
        if (sketchedDigitsOnFrac[2].camera != null)
        {
            drawingCalculator.BindCamera(sketchedDigitsOnFrac[2].camera);
            guessString += ai.Calc(drawingCalculator).ToString();
            ret += "{" + ai.Calc(drawingCalculator).ToString() + "}";
        }
        guessText.text = "Guess: " + guessString;

        return ret;
    }
    public string DrawDefault()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
        {
            Ray drawRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("DrawingBox"))
            {
                isDrawing = true;
                if (transform.position.x < minX - 0.5f || transform.position.x > maxX + 0.5f)
                {
                    index++;
                    minX = 100000;
                    maxX = -100000;
                    minY = 100000;
                    maxY = -100000;
                    Camera camera = Instantiate(cameraObject, this.transform.position + new Vector3(50 * index, -58.5f, -10), Quaternion.identity).GetComponent<Camera>();
                    sketchedDigits.Add(new SketchedDigit(transform.position.x, index, camera));
                    curSketchedDigit = sketchedDigits[sketchedDigits.Count - 1];
                }
                curLine = Instantiate(drawObj, this.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
                curLineToCalc = Instantiate(drawObjToCalc, this.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
                curLine.sortingOrder = lineOrderInLayer;
                curLine.gameObject.transform.SetParent(drawParent);
                curLineToCalc.gameObject.transform.SetParent(drawParent);

                nonDrawingTime = 0.0f;
                createLine(raycastHit2D.point, (Vector3)raycastHit2D.point + new Vector3(50 * index, -50, 0));
            }
        }
        else if (isDrawing && (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetMouseButton(0)))
        {
            nonDrawingTime = 0.0f;
            Ray drawRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("DrawingBox"))
            {
                transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
                minX = Mathf.Min(minX, transform.position.x);
                maxX = Mathf.Max(maxX, transform.position.x);
                minY = Mathf.Min(minY, transform.position.y);
                maxY = Mathf.Max(maxY, transform.position.y);
                connectLine(raycastHit2D.point, (Vector3)raycastHit2D.point + new Vector3(50 * index, -50, 0));
                curSketchedDigit.x = (minX + maxX) / 2;
                curSketchedDigit.camera.transform.position = new Vector3(((minX + maxX) / 2) + (50 * index), -50.0f + ((minY + maxY) / 2), -10);
                curSketchedDigit.camera.orthographicSize = Mathf.Max(Mathf.Lerp(0, 4.87f, ((maxY - minY) + 2.5f) / 10.0f), Mathf.Lerp(0, 4.87f, ((maxX - minX) + 2.5f) / 10.0f));
            }
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
        int predictNumber = 0;
        Sort();
        for (int i = 0; i < sketchedDigits.Count; i++)
        {
            drawingCalculator.BindCamera(sketchedDigits[i].camera);
            predictNumber = predictNumber * 10 + ai.Calc(drawingCalculator);
        }
        guessText.text = "Guess: " + predictNumber.ToString();
        return predictNumber.ToString();
    }

    void createLine(Vector3 mousePos, Vector3 calcPos)
    {
        positionCount = 2;

        curLine.SetPosition(0, mousePos);
        curLine.SetPosition(1, mousePos);
        curLineToCalc.SetPosition(0, calcPos);
        curLineToCalc.SetPosition(1, calcPos);

    }
    void connectLine(Vector3 mousePos, Vector3 calcPos)
    {
        if (PrevPos != null && Mathf.Abs(Vector3.Distance(PrevPos, mousePos)) >= 0.001f)
        {
            PrevPos = mousePos;
            positionCount++;
            curLine.positionCount = positionCount;
            curLine.SetPosition(positionCount - 1, mousePos);
            curLineToCalc.positionCount = positionCount;
            curLineToCalc.SetPosition(positionCount - 1, calcPos);
        }

    }
    public void EraseSketch()
    {
        if (onAnimation) return;
        index = 0;
        minX = 100000;
        maxX = -100000;
        minY = 100000;
        maxY = -100000;
        drawSpaceInFrac = -1;
        nonDrawingTime = -100000.0f;
        for (int i = 0; i < drawParent.childCount; i++)
            Destroy(drawParent.GetChild(i).gameObject);
        for (int i = 0; i < sketchedDigits.Count; i++)
            Destroy(sketchedDigits[i].camera.gameObject);
        for (int i = 0; i < 3; i++)
            if (sketchedDigitsOnFrac[i].camera != null)
                Destroy(sketchedDigitsOnFrac[i].camera.gameObject);
        sketchedDigits.Clear();
        for (int i = 0; i < 3; i++) sketchedDigitsOnFrac[i] = new SketchedDigit();
    }
    public void Sort()
    {
        for(int i=0;i<sketchedDigits.Count - 1;++i)
        {
            for(int j=i + 1;j<sketchedDigits.Count;++j)
            {
                if(sketchedDigits[i].x > sketchedDigits[j].x)
                {
                    SketchedDigit tmp = sketchedDigits[i];
                    sketchedDigits[i] = sketchedDigits[j];
                    sketchedDigits[j] = tmp;
                }
            }
        }
    }

}
