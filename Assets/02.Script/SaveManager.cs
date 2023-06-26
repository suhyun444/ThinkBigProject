using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class SaveData
{

}
public class SaveManager : Singleton<SaveManager>
{
    public readonly string path = Application.persistentDataPath + "/UserData.json";
    public SaveData data;
    // Start is called before the first frame update
    private void Init()
    {
        if (File.Exists(path))
        {
            string loadJson = File.ReadAllText(path);
            GetData(loadJson);
        }
        else
        {
            InitData();
        }
    }
    public void GetData(string json)
    {
        data = JsonUtility.FromJson<SaveData>(json);
    }
    public void InitData()
    {
        SaveData initData = new SaveData();
        string json = JsonUtility.ToJson(initData);
        File.WriteAllText(path, json);
    }
    public void SaveData()
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path,json);
    }
}
