using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TexDrawLib;

public class QuestionBox : MonoBehaviour
{
    private TEXDraw3D question;
    [SerializeField] private GameObject box;
    private string curQuestion;
    // Start is called before the first frame update
    private void Awake() {
        question = GetComponent<TEXDraw3D>();
    }
    private void Start() {
        SetProblemText();
    }
    public void BindQuestion(string q)
    {
        if(q.Contains("hline"))
            q = SetQuestionVerticalToHorizontal(q);
        else
        {
            q = q.Replace("\\rightarrow","->").Replace('~', ' ').Replace("$x$", "x").Replace("\\left","")
                .Replace("\\right","").Replace("\\begin{align}","").Replace("\\end{align}","").Replace("\\\\","")
                .Replace("\\minus","-");

        }
        curQuestion = q;
        if(this.question.text == "Question")
        {
            SetProblemText();
        }
    }
    public void SetCorrectText(float solveTime)
    {
        if(solveTime < 5)
        {
            this.question.text = "PERFECT";
        }
        else if(solveTime < 7)
        {
            this.question.text = "GREAT";
        }
        else 
        {
            this.question.text = "GOOD";
        }
    }
    public void SetIncorrectText()
    {
        int type = Random.Range(0,3);
        if(type == 0)
        {
            this.question.text = "OOPS!";
        }
        else if (type == 1)
        {
            this.question.text = "BAD!";
        }
        else
        {
            this.question.text = "MISS!";
        }
    }
    public void SetProblemText()
    {
        this.question.text = curQuestion;
    }
    private string SetQuestionVerticalToHorizontal(string q)
    {
        Debug.Log(q);
        q = q.Replace("\\times", "*").Replace("&","").Replace("\\","").Replace(" ","");
        char oper = ' ';
        if(q.Contains('-'))
            oper = '-';
        else if (q.Contains('+'))
            oper = '+';
        else if (q.Contains('*'))
            oper = '*';
        string[] nums = q.Split(oper);
        for(int i=nums[0].Length - 1;i>=0;i--)
        {
            if(nums[0][i] < '0' || '9' < nums[0][i])
            {
                nums[0] = nums[0].Substring(i + 1,nums[0].Length - i - 1);
                break;
            }
        }
        for(int i=0;i<nums[1].Length;i++)
        {
            if (nums[1][i] < '0' || '9' < nums[1][i])
            {
                nums[1] = nums[1].Substring(0, i);
                break;
            }
        }
        return nums[0] + " " + oper + " " + nums[1] + " = ?";
    }
}
