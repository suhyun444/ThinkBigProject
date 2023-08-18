using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CustomUtils{

    public static class Util 
    {
        public static string SplitIntByComma(int num)
        {
            string resultText = "";
            while (num >= 1000)
            {
                int temp = num % 1000;
                if(temp != 0)
                    resultText = temp.ToString() + resultText;
                int digit = 0;
                while(temp > 0)
                {
                    temp /= 10;
                    digit++;
                }
                for (int i = 0; i < 3 - digit; i++)
                    resultText = "0" + resultText;
                num /= 1000;
                resultText = "," + resultText;
            }
            return num.ToString() + resultText;
        }
        public static string SplitRemainZero(string s)
        {
            int index = s.Length - 1;
            while(index > 0 && (s[index] == '0' || s[index] == '.'))
            {
                --index;
            }
            return s.Substring(0,index + 1);
        }
        public static IEnumerator HighlightObject(GameObject gameObject,AnimationCurve curve)
        {
            Vector3 start = new Vector3(0.0f,0.0f,1.0f);
            Vector3 end = gameObject.transform.localScale;
            float time = 0.0f;
            float t = 0.25f;
            while(time < 1)
            {
                time += Time.deltaTime / t;
                gameObject.transform.localScale = Vector3.LerpUnclamped(start,end,curve.Evaluate(time));
                yield return null;
            }
            yield break;
        }
        public static IEnumerator HighlightObjects(GameObject[] gameObject, AnimationCurve curve)
        {
            Vector3 start = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3[] end = new Vector3[gameObject.Length];
            for(int i=0;i<gameObject.Length;++i)
            {
                end[i] = gameObject[i].transform.localScale;
            }
            float time = 0.0f;
            float t = 0.25f;
            while (time < 1)
            {
                time += Time.deltaTime / t;
                for(int i=0;i<gameObject.Length;++i)
                {
                    gameObject[i].transform.localScale = Vector3.LerpUnclamped(start, end[i], curve.Evaluate(time));
                }
                yield return null;
            }
            yield break;
        }
        public static IEnumerator HighLightText(TextMeshPro tmp,float highLightSize)
        {
            float start = tmp.fontSize;
            float time = 0;
            float t = 0.1f;
            while (time < 1)
            {
                time += Time.deltaTime / t;
                tmp.fontSize = Mathf.Lerp(start, highLightSize, time);
                yield return null;
            }
            yield return new WaitForSeconds(0.05f);
            while (time > 0)
            {
                time -= Time.deltaTime / t;
                tmp.fontSize = Mathf.Lerp(start, highLightSize, time);
                yield return null;
            }
        }
    }
}

