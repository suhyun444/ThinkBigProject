using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public enum CostumeType
{
    Magician,
    Witch,
    Thief
}
public class SaveData
{
    public string name;
    public CostumeType costumeType;
    public List<int> havingCostumeList;
    public int level;
    public int usedSkillPoint;
    public List<int> skillLevels;
    public int expAmount;
    public int crystal;
    public List<string> lastEarnedTimeList;
    public List<int> petList;
    public List<int> havingPetList;
    public SaveData()
    {
        name = "";
        costumeType = CostumeType.Magician;
        havingCostumeList = new List<int>();
        havingCostumeList.Add(0);
        level = 0;
        usedSkillPoint = 0;
        expAmount = 0;
        crystal = 0;
        skillLevels = new List<int>((int)SkillType.End);
        lastEarnedTimeList = new List<string>(4); 
        petList = new List<int>(4);
        havingPetList = new List<int>();
        for(int i=0;i<(int)SkillType.End;++i)
            skillLevels.Add(0);
        for(int i=0;i<4;++i)
        {
            lastEarnedTimeList.Add(DateTime.Now.ToString());
            petList.Add(-1);
        }
    }
}
public enum LanguageType
{
    Korean,
    English
}
public class OptionData
{
    public bool endTutorial;
    public float volume;
    public LanguageType languageType; 
    public OptionData()
    {
        endTutorial = false;
        volume = 1.0f;
        languageType = LanguageType.Korean;
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
public class MathpidData
{
    public string memberId;
    public string authorization;
    public MathpidData()
    {
        memberId = "null";
        authorization = "null";
    }
}
public class SaveManager : Singleton<SaveManager>
{
    private SaveData data;
    private MagicBookData magicBookData;
    private OptionData optionData;
    private MathpidData mathpidData;
    private void Awake() {
        Init();
    }
    private void Init()
    {
        InitSingleTon(this);
        if (File.Exists(Const.Data.USERDATA_SAVE_PATH) && File.Exists(Const.Data.MAGICBOOKDATA_SAVE_PATH) && File.Exists(Const.Data.OPTIONDATA_SAVE_PATH) && File.Exists(Const.Data.MATHPIDDATA_SAVE_PATH))
        {
            string userDataLoadJson = File.ReadAllText(Const.Data.USERDATA_SAVE_PATH);
            string magicBookDataLoadJson = File.ReadAllText(Const.Data.MAGICBOOKDATA_SAVE_PATH);
            string optionDataLoadJson = File.ReadAllText(Const.Data.OPTIONDATA_SAVE_PATH);
            string mathpidDataLoadJson = File.ReadAllText(Const.Data.MATHPIDDATA_SAVE_PATH);
            LoadData(userDataLoadJson,magicBookDataLoadJson,optionDataLoadJson, mathpidDataLoadJson);
        }
        else
        {
            InitData();
        }
    }
    public void LoadData(string userDataJson,string magicBookDataJson,string optionDataLoadJson, string mathpidDataLoadJson)
    {
        data = JsonUtility.FromJson<SaveData>(userDataJson);
        magicBookData = JsonUtility.FromJson<MagicBookData>(magicBookDataJson);
        optionData = JsonUtility.FromJson<OptionData>(optionDataLoadJson);
        mathpidData = JsonUtility.FromJson<MathpidData>(mathpidDataLoadJson);
    }
    public void InitData()
    {
        SaveData initData = new SaveData();
        string json = JsonUtility.ToJson(initData);
        File.WriteAllText(Const.Data.USERDATA_SAVE_PATH, json);
        MagicBookData initMagicBookData = new MagicBookData();
        string magicBookJson = JsonUtility.ToJson(initMagicBookData);
        File.WriteAllText(Const.Data.MAGICBOOKDATA_SAVE_PATH,magicBookJson);
        OptionData initOptionData = new OptionData();
        string optionJson = JsonUtility.ToJson(initOptionData);
        File.WriteAllText(Const.Data.OPTIONDATA_SAVE_PATH, optionJson);
        MathpidData initMathpidData = new MathpidData();
        string mathpidjson = JsonUtility.ToJson(initMathpidData);
        File.WriteAllText(Const.Data.MATHPIDDATA_SAVE_PATH,mathpidjson);
    }
    public void SaveData()
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Const.Data.USERDATA_SAVE_PATH,json);
    }
    public void SaveMagicBookData()
    {
        string json = JsonUtility.ToJson(magicBookData);
        File.WriteAllText(Const.Data.MAGICBOOKDATA_SAVE_PATH,json);
    }
    public void SaveOptionData()
    {
        string optionJson = JsonUtility.ToJson(optionData);
        File.WriteAllText(Const.Data.OPTIONDATA_SAVE_PATH, optionJson);
    }
    public void SaveMathpidData()
    {
        string mathpidjson = JsonUtility.ToJson(mathpidData);
        File.WriteAllText(Const.Data.MATHPIDDATA_SAVE_PATH, mathpidjson);
    }
    public void SetEndTutorialData(bool end)
    {
        optionData.endTutorial = end;
    }
    public bool GetEndTutorialData()
    {
        return optionData.endTutorial;
    }
    public void SetMemberIdData(string memberId)
    {
        mathpidData.memberId = memberId;
    }
    public string GetMemberIdData()
    {
        return mathpidData.memberId;
    }
    public void SetAuthorizationData(string authorization)
    {
        mathpidData.authorization = authorization;
    }
    public string GetAuthorizationData()
    {
        return mathpidData.authorization;
    }
    public string GetNameData()
    {
        return data.name;
    }
    public void SetNameDate(string name)
    {
        data.name = name;
    }
    public void SetCostumeTypeData(CostumeType type)
    {
        data.costumeType = type;
    }
    public CostumeType GetCostumeTypeData()
    {
        return data.costumeType;
    }
    public void AddHavingCostumeList(int item)
    {
        data.havingCostumeList.Add(item);
    }
    public List<int> GetHavingCostumeList()
    {
        return data.havingCostumeList;
    }
    public void SetVolumeData(float volume)
    {
        optionData.volume = volume;
    }
    public float GetVolumeData()
    {
        return optionData.volume;
    }
    public void SetLanguagueTypeData(LanguageType languageType)
    {
        optionData.languageType = languageType;
    }
    public LanguageType GetLanguageTypeData()
    {
        return optionData.languageType;
    }
    public void ResetSkillPoint()
    {
        data.usedSkillPoint = 0;
        for(int i=0;i<data.skillLevels.Count;++i)
        {
            data.skillLevels[i] = 0;
        }
    }
    public void UseSkillPoint(SkillType skillType)
    {
        ++data.usedSkillPoint;
        ++data.skillLevels[(int)skillType];
    }
    public int GetUsedSkillPoint()
    {
        return data.usedSkillPoint;
    }
    public int GetSkillLevel(SkillType skillType)
    {
        return data.skillLevels[(int)skillType];
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
