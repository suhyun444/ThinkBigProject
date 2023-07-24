using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : Singleton<LanguageManager>
{
    public LanguageType languageType;
    public List<LanguageText> languageTexts;
    private void Awake() {
        languageType = SaveManager.Instance.GetLanguageTypeData();
        LanguageText[] temps = (LanguageText[])Resources.FindObjectsOfTypeAll(typeof(LanguageText));
        for(int i=0;i<temps.Length;++i)
            if(temps[i] != null)
            {
                languageTexts.Add(temps[i]);
                temps[i].Init();
            }
    }
    public void AddText(LanguageText text)
    {
        languageTexts.Add(text);
        text.Init();
    }
    public void ChangeLanguage(LanguageType type)
    {
        languageType = type;
        for(int i=0;i<languageTexts.Count;++i)
        {
            languageTexts[i].BindText(type);
        }
    }
}
