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
        if(q.Contains("hline"))
            q = SetQuestionVerticalToHorizontal(q);
        else
        {
            q = q.Replace("\\rightarrow","->").Replace('~', ' ').Replace("$x$", "x").Replace("\\left","")
                .Replace("\\right","").Replace("\\begin{align}","").Replace("\\end{align}","").Replace("\\\\","")
                .Replace("\\minus","-");

        }
        this.question.text = q;
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
