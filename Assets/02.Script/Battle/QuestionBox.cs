using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TexDrawLib;

public class QuestionBox : MonoBehaviour
{
    private TEXDraw3D question;
    [SerializeField] private GameObject box;
    // Start is called before the first frame update
    private void Awake() {
        question = GetComponent<TEXDraw3D>();
    }
    public void BindQuestion(string q)
    {
        this.question.text = q;
        if(q.Contains("frac") || q.Contains("hline"))
            SetDefaultPreset();
        else
            SetHorizontalPreset();
    }
    private void SetDefaultPreset()
    {
        question.size = 0.6f;
        question.rectTransform.localPosition = new Vector3(-0.6f,1.31f,0.0f);
        question.rectTransform.sizeDelta = new Vector2(2.52f,2.07f);
        box.transform.localPosition = new Vector3(-0.6f,1.26f,0.0f);
        box.transform.localScale = new Vector3(2.6f,2.2f,0.0f);
    }
    private void SetHorizontalPreset()
    {
        question.size = 0.36f;
        question.rectTransform.localPosition = new Vector3(-1.26f,1.02f,0.0f);
        question.rectTransform.sizeDelta = new Vector2(4.04f,0.83f);
        box.transform.localPosition = new Vector3(-1.26f,1.03f,0.0f);
        box.transform.localScale = new Vector3(4.03f,0.8f,0.0f);
    }

}
