using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class SaveData
{
    public int level;
    public int expAmount;
    public int crystal;
    public List<string> lastEarnedTimeList;
    public List<int> petList;
    public List<int> havingPetList;
    public SaveData()
    {
        level = 0;
        expAmount = 0;
        crystal = 0;
        lastEarnedTimeList = new List<string>(); 
        petList = new List<int>();
        havingPetList = new List<int>();
        for(int i=0;i<4;i++)
        {
            lastEarnedTimeList.Add(DateTime.Now.ToString());
            petList.Add(-1);
        }
    }
}
public class SaveManager : Singleton<SaveManager>
{
    private SaveData data;
    private void Awake() {
        //InitData();
        Init();
    }
    // Start is called before the first frame update
    private void Init()
    {
        if (File.Exists(Const.data.SAVEPATH))
        {
            string loadJson = File.ReadAllText(Const.data.SAVEPATH);
            LoadData(loadJson);
        }
        else
        {
            InitData();
        }
    }
    public void LoadData(string json)
    {
        data = JsonUtility.FromJson<SaveData>(json);
    }
    public void InitData()
    {
        SaveData initData = new SaveData();
        string json = JsonUtility.ToJson(initData);
        File.WriteAllText(Const.data.SAVEPATH, json);
    }
    public void SaveData()
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Const.data.SAVEPATH,json);
    }
    public void SetCrystalData(int crystal)
    {
        data.crystal = crystal;
    }
    public int GetCrystalData()
    {
        return data.crystal;
    }
    public void SetLastEarnedTimeDate(int index,DateTime time)
    {
        data.lastEarnedTimeList[index] = time.ToString();
    }
    public DateTime GetLastEarnedTimeData(int index)
    {
        return Convert.ToDateTime(data.lastEarnedTimeList[index]);
    }
    public void SetLevelData(int level)
    {
        data.level = level;
    }
    public int GetLevelData()
    {
        return data.level;
    }
    public void SetExpAmountData(int amount)
    {
        data.expAmount = amount;
    }
    public int GetExpAmountData()
    {
        return data.expAmount;
    }
    public void ChangePetList(int index,int item)
    {
        data.petList[index] = item;
    }
    public List<int> GetPetList()
    {
        return data.petList;
    }
    public void AddHavingPetList(int item)
    {
        data.havingPetList.Add(item);
    }
    public List<int> GetHavingPetList()
    {
        return data.havingPetList;
    }
}
