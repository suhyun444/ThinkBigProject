using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Monster : MonoBehaviour
{
    int answer;
    int prevQuestion = -1;
    [SerializeField] GameObject ac;
    [SerializeField] GameObject wa;
    public delegate void CorrectAnswer(bool flag);
    public CorrectAnswer correctAnswerCallBack;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void BindQuestion(int answer)
    {
        this.answer = answer;
    }
    public void CheckAnswer(int input)
    {
        Debug.Log(answer + " answer " + input + " input");
        if(answer == input)
        {
            ac.SetActive(true);
            StartCoroutine(SetFalse(ac));
            correctAnswerCallBack.Invoke(true);
        }
        else
        {
            wa.SetActive(true);
            StartCoroutine(SetFalse(wa));
            correctAnswerCallBack.Invoke(false);
        }
    }
    IEnumerator SetFalse(GameObject gameObject)
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
