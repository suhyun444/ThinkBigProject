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
public class MagicBookData
{
    public int gage;
    public List<string> problems;
    public List<string> answers;
    public MagicBookData()
    {
        gage = 0;
        problems = new List<string>();
        answers = new List<string>();
    }
}
public class SaveManager : Singleton<SaveManager>
{
    private SaveData data;
    private MagicBookData magicBookData;
    private void Awake() {
        Init();
    }
    private void Init()
    {
        if (File.Exists(Const.data.USERDATA_SAVE_PATH) && File.Exists(Const.data.MAGICBOOKDATA_SAVE_PATH))
        {
            string userDataLoadJson = File.ReadAllText(Const.data.USERDATA_SAVE_PATH);
            string magicBookDataLoadJson = File.ReadAllText(Const.data.MAGICBOOKDATA_SAVE_PATH);
            LoadData(userDataLoadJson,magicBookDataLoadJson);
        }
        else
        {
            InitData();
        }
    }
    public void LoadData(string userDataJson,string magicBookDataJson)
    {
        data = JsonUtility.FromJson<SaveData>(userDataJson);
        magicBookData = JsonUtility.FromJson<MagicBookData>(magicBookDataJson);
    }
    public void InitData()
    {
        SaveData initData = new SaveData();
        string json = JsonUtility.ToJson(initData);
        File.WriteAllText(Const.data.USERDATA_SAVE_PATH, json);
        MagicBookData initMagicBookData = new MagicBookData();
        string magicBookJson = JsonUtility.ToJson(initMagicBookData);
        File.WriteAllText(Const.data.MAGICBOOKDATA_SAVE_PATH,magicBookJson);
    }
    public void SaveData()
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Const.data.USERDATA_SAVE_PATH,json);
    }
    public void SaveMagicBookData()
    {
        string json = JsonUtility.ToJson(magicBookData);
        File.WriteAllText(Const.data.MAGICBOOKDATA_SAVE_PATH,json);
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
    public int GetGageData()
    {
        return magicBookData.gage;
    }
    public void SetGageData(int gage)
    {
        magicBookData.gage = gage;
    }
    public void AddMagicBookData(string problem,string answer)
    {
        if(magicBookData.problems.Count >= 15)
        {
            RemoveMagicBookData(0);
        }
        magicBookData.problems.Add(problem);
        magicBookData.answers.Add(answer);
    }
    public string GetProblemData(int index)
    {
        return magicBookData.problems[index];
    }
    public string GetAnswerData(int index)
    {
        return magicBookData.answers[index];
    }
    public int GetMagicBookLength()
    {
        return magicBookData.problems.Count;
    }
    public void RemoveMagicBookData(int index)
    {
        magicBookData.problems.RemoveAt(index);
        magicBookData.answers.RemoveAt(index);
    }
}
