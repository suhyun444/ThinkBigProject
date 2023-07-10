using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils{

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
    }
}

