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
    public string lastEarnedTime;
    public SaveData()
    {
        level = 0;
        expAmount = 0;
        crystal = 0;
        lastEarnedTime = DateTime.Now.ToString();
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
    public void SetLastEarnedTimeDate(DateTime time)
    {
        data.lastEarnedTime = time.ToString();
    }
    public DateTime GetLastEarnedTimeData()
    {
        return Convert.ToDateTime(data.lastEarnedTime);
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
}
