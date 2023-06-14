using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Monster : MonoBehaviour
{
    [SerializeField] TextMeshPro questionText;
    Dictionary<string,int> questions = new Dictionary<string, int>();
    List<string> keys;
    int answer;
    int prevQuestion = -1;
    [SerializeField] GameObject ac;
    [SerializeField] GameObject wa;
    // Start is called before the first frame update
    void Start()
    {
        questions.Add("2 + 2 * 2",6);       
        questions.Add("2 * 3 + 2",8);
        questions.Add("2 + 4 - 1",5);
        questions.Add("5 * 2 - 6",4);
        questions.Add("2 + 4 * 3",14);
        questions.Add("4 + 7 + 5",16);
        questions.Add("3 * 3 + 1",10);
        questions.Add("3 + 6 * 2",15);
        keys = questions.Keys.ToList<string>();
        BindQuestion();
    }
    void BindQuestion()
    {
        int index = Random.Range(0,keys.Count);
        while(index == prevQuestion)index = Random.Range(0,keys.Count);
        prevQuestion = index;
        string curKey = keys[index];
        answer =questions[curKey];
        questionText.text = curKey + " = ?"; 
    }
    public void CheckAnswer(int input)
    {
        Debug.Log(answer + " answer " + input + " input");
        if(answer == input)
        {
            ac.SetActive(true);
            StartCoroutine(SetFalse(ac));
            BindQuestion();
        }
        else
        {
            wa.SetActive(true);
            StartCoroutine(SetFalse(wa));
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
