using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class SaveData
{
    public int crystal;
    public SaveData()
    {
        crystal = 0;
    }
}
public class SaveManager : Singleton<SaveManager>
{
    private SaveData data;
    private void Awake() {
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
}
