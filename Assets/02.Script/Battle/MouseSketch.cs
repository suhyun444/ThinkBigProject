using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

struct SketchedDigit
{
    public float x;
    public int index;
    public Camera camera;
    public SketchedDigit(float x,int index,Camera camera)
    {
        this.x = x;
        this.index = index;
        this.camera = camera;
    }
    public void CopyStruct(SketchedDigit other){
        other.x = x;
        other.index = index;
        other.camera = camera;
    }
};
public class MouseSketch : MonoBehaviour
{
    [SerializeField] private TextMeshPro guessText;
    [SerializeField] private EraseButton eraseButton;
    public GameObject drawObj;
    public GameObject drawObjToCalc;
    public Transform drawParent;
    public Plane drawBGplane;
    public GameObject cameraObject;
    [SerializeField] RecognizeDigitsAI ai;
    [SerializeField] private Material drawingBookMaterial;
    [SerializeField] private Material drawingMaterial;
    Vector3 drawStartPos;
    
    private LineRenderer curLine;  //Line which draws now
    private LineRenderer curLineToCalc;
    private int positionCount = 2;  //Initial start and end position
    private Vector3 PrevPos = Vector3.zero; // 0,0,0 position variable

    private DrawingCalculator drawingCalculator;
    private float nonDrawingTime = -100000.0f;
    private int index = 0;
    private List<SketchedDigit> sketchedDigits = new List<SketchedDigit>();
    private float minX = 100000,maxX = -100000;
    private float minY = 100000,maxY = -100000;
    private bool isDrawing = false;
    private bool onAnimation = false;


    // Start is called before the first frame update
    void Start()
    {
        drawBGplane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
        drawingCalculator = new DrawingCalculator();
        eraseButton.BindClickEvent(EraseSketch);
    }

    // Update is called once per frame
    void Update()
    {
        if(onAnimation)return;
        nonDrawingTime += Time.deltaTime;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
        if(-8.47f <= transform.position.x && transform.position.x <= 1.79 && -14.42f <= transform.position.y && transform.position.y <= -4.08f)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
            {
                Ray drawRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position,transform.forward,999);
                if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("DrawingBox"))
                {
                    isDrawing = true;
                    if(transform.position.x < minX - 0.5f || transform.position.x > maxX + 0.5f)
                    {
                        index++;
                        minX = 100000;
                        maxX = -100000;
                        minY = 100000;
                        maxY = -100000;
                        Camera camera = Instantiate(cameraObject,this.transform.position + new Vector3(50 * index,-58.5f,-10),Quaternion.identity).GetComponent<Camera>();
                        sketchedDigits.Add(new SketchedDigit(transform.position.x,index,camera));
                    }
                    curLine = Instantiate(drawObj,this.transform.position,Quaternion.identity).GetComponent<LineRenderer>();
                    curLineToCalc = Instantiate(drawObjToCalc,this.transform.position,Quaternion.identity).GetComponent<LineRenderer>();
                    curLine.gameObject.transform.SetParent(drawParent);
                    curLineToCalc.gameObject.transform.SetParent(drawParent);

                    nonDrawingTime = 0.0f;
                    createLine(raycastHit2D.point,(Vector3)raycastHit2D.point + new Vector3(50 * index, -50, 0));
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
                    minX = Mathf.Min(minX,transform.position.x);
                    maxX = Mathf.Max(maxX,transform.position.x);
                    minY = Mathf.Min(minY,transform.position.y);
                    maxY = Mathf.Max(maxY,transform.position.y);
                    connectLine(raycastHit2D.point,(Vector3)raycastHit2D.point + new Vector3(50 * index, -50, 0));
                    sketchedDigits[index - 1].camera.transform.position = new Vector3(((minX + maxX) / 2) + (50 * index),-50.0f + ((minY + maxY) / 2),-10);
                    sketchedDigits[index - 1].camera.orthographicSize = Mathf.Max( Mathf.Lerp(0,4.87f,((maxY - minY) + 2.5f) / 10.0f),Mathf.Lerp(0, 4.87f, ((maxX - minX) + 2.5f) / 10.0f));
                }
            }
        }
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
        SortDigits();
        int predictNumber = 0;
        for (int i = 0; i < sketchedDigits.Count; i++)
        {
            drawingCalculator.BindCamera(sketchedDigits[i].camera);
            predictNumber = predictNumber * 10 + ai.Calc(drawingCalculator);
        }
        guessText.text = "Guess: " + predictNumber.ToString();
        if(nonDrawingTime > 1.0f)
        {
            nonDrawingTime = -100000.0f;
            if(ai.Commit(predictNumber))
            {
                StartCoroutine(RuneEngrave());
            }
            else{
                EraseSketch();
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            EraseSketch();
        }


    }
    IEnumerator RuneEngrave()
    {
        onAnimation = true;
        float time = 0;
        float t = 0.35f;
        while(time < 1)
        {
            time += Time.deltaTime / t;
            drawingMaterial.SetFloat("_HighLightedAmount",Mathf.Lerp(1,0.4f,time));
            drawingBookMaterial.SetFloat("_OutLineAlpha",Mathf.Lerp(0,1,time));
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
        EraseSketch();
        drawingMaterial.SetFloat("_Alpha", 1);
        drawingMaterial.SetFloat("_HighLightedAmount", 1);
        onAnimation = false;
    }
    void createLine(Vector3 mousePos,Vector3 calcPos)
    {
        positionCount = 2;

        curLine.SetPosition(0, mousePos);
        curLine.SetPosition(1, mousePos);
        curLineToCalc.SetPosition(0,calcPos);
        curLineToCalc.SetPosition(1,calcPos);

    }
    void connectLine(Vector3 mousePos,Vector3 calcPos)
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
        index = 0;
        minX = 100000;
        maxX = -100000;
        minY = 100000;
        maxY = -100000;
        nonDrawingTime = -100000.0f;
        for (int i = 0; i < drawParent.childCount; i++)
            Destroy(drawParent.GetChild(i).gameObject);
        for(int i=0;i<sketchedDigits.Count;i++)
            Destroy(sketchedDigits[i].camera.gameObject);
        sketchedDigits.Clear();
    }
    private void SortDigits()
    {
        for(int i=0;i<sketchedDigits.Count;i++)
        {
            for(int j=0;j<sketchedDigits.Count-1;j++)
            {
                if(sketchedDigits[j].x > sketchedDigits[j + 1].x)
                {
                    SketchedDigit temp = new SketchedDigit();
                    sketchedDigits[j].CopyStruct(temp);
                    sketchedDigits[j+1].CopyStruct(sketchedDigits[j]);
                    temp.CopyStruct(sketchedDigits[j]);
                }
            }
        }
    }

}
