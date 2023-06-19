using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] UnityEngine.UI.Text guessText;
    public GameObject drawObj;
    public Transform drawParent;
    public Plane drawBGplane;
    public GameObject cameraObject;
    [SerializeField] RecognizeDigitsAI ai;
    Vector3 drawStartPos;
    
    DrawingCalculator drawingCalculator;
    float nonDrawingTime = -100000.0f;
    public int index = 0;
    GameObject drawStroke;
    GameObject drawStrokeToCalc;
    List<SketchedDigit> sketchedDigits = new List<SketchedDigit>();
    public float minX = 100000,maxX = -100000;
    public float minY = 100000,maxY = -100000;
    public bool isDrawing = false;


    // Start is called before the first frame update
    void Start()
    {
        drawBGplane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
        drawingCalculator = new DrawingCalculator();
    }

    // Update is called once per frame
    void Update()
    {
        nonDrawingTime += Time.deltaTime;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
        if(-8.47f <= transform.position.x && transform.position.x <= 1.79 && -14.42f <= transform.position.y && transform.position.y <= -4.08f)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
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
                drawStroke = (GameObject)Instantiate(drawObj, this.transform.position, Quaternion.identity);
                drawStrokeToCalc = (GameObject)Instantiate(drawObj, this.transform.position + new Vector3(50 * index ,-50,0), Quaternion.identity);
                drawStroke.transform.SetParent(drawParent);
                drawStrokeToCalc.transform.SetParent(drawParent);

                Ray drawRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                float distance;
                nonDrawingTime = 0.0f;
                if (drawBGplane.Raycast(drawRay, out distance))
                {
                    drawStartPos = drawRay.GetPoint(distance);
                }
            }
            else if (isDrawing && (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetMouseButton(0)))
            {
                Debug.Log(isDrawing + "isdra");
                transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
                minX = Mathf.Min(minX,transform.position.x);
                maxX = Mathf.Max(maxX,transform.position.x);
                minY = Mathf.Min(minY,transform.position.y);
                maxY = Mathf.Max(maxY,transform.position.y);
                Ray drawRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                float distance;
                nonDrawingTime = 0.0f;
                if (drawBGplane.Raycast(drawRay, out distance))
                {
                    drawStroke.transform.position = drawRay.GetPoint(distance);
                    drawStrokeToCalc.transform.position = drawRay.GetPoint(distance) + new Vector3(50 * index,-50,0);
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
        guessText.text = "Guess : " + predictNumber.ToString();
        if(nonDrawingTime > 1.0f)
        {
            ai.Commit(predictNumber);
            EraseSketch();
            nonDrawingTime = -100000.0f;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            EraseSketch();
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
