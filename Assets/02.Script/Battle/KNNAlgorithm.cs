using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public struct DataPair{
    public double distance;
    public int label;
    public DataPair(double distance,int label)
    {
        this.distance = distance;
        this.label = label;
    }
};
public class DataCompare : IComparer<DataPair>
{
    // 문자열의 길이에 따라 오름차순
    public int Compare(DataPair x, DataPair y)
    {
        double result = x.distance - y.distance;
        if(result < 0)return -1;
        else return 1;
        
    }
}
public class KNNAlgorithm
{
    float[,] dataSet = new float[100 * 10, 28*28 + 1];
    // Start is called before the first frame update
    public KNNAlgorithm() {
        TextAsset theList = (TextAsset)Resources.Load("dataset" , typeof(TextAsset));
        string str = theList.text;
        StringReader streamReader = new StringReader(str);
        for(int i=0;i<100* 10;i++)
        {
            string curLine = streamReader.ReadLine();
            string[] curData = curLine.Split(',');
            for(int j=0;j<28*28+1;j++)
            {
                float.TryParse(curData[j], out dataSet[i, j]);
            }
        }
        // SortedSet<DataPair> pq = new SortedSet<DataPair>(new DataCompare());
        // pq.Add(new DataPair(0,0));
        // pq.Add(new DataPair(1,0));
        // pq.Add(new DataPair(2,0));
        // pq.Add(new DataPair(3,0));
        // pq.Add(new DataPair(4,0));
        // Debug.Log(pq.Max.distance);

    }
    public int Calc(float[] data,int k)
    {
        SortedList pq = new SortedList();
        for(int i=9;i<100*10;i++)
        {
            float curDistance = 0;
            for(int j=0;j<28*28;j++)
            {
                curDistance += Mathf.Pow(data[j] - dataSet[i,j],2);
            }
            curDistance = Mathf.Sqrt(curDistance);

            pq.Add(curDistance,(int)dataSet[i,784]);
            if(pq.Count > k)
            {
                pq.RemoveAt(pq.Count - 1);
            }
        }
        int[] numCount = new int[10];
        for(int i=0;i<10;i++)numCount[i] = 0;
        for(int i=0;i<k;i++)
        {
            numCount[(int)pq.GetByIndex(i)]++;
        }
        int maxCount = numCount[0];
        int result = 0;
        for(int i=1;i<10;i++)
        {  
            if(maxCount < numCount[i])
            {
                maxCount = numCount[i];
                result = i;
            }
        }
        return result;
    }
}
