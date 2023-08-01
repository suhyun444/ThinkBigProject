using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageText : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    [TextArea]
    public string koreanText;
    public float koreanFontSize = -1;
    [TextArea]
    public string englishText;
    public float englishFontSize;
    public virtual void Init()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        BindText(LanguageManager.Instance.languageType);
    }
    public virtual void BindText(LanguageType type)
    {
        textMeshPro.text = (type == LanguageType.Korean)? koreanText:englishText;
        if(koreanFontSize != -1)
            textMeshPro.fontSize = (type == LanguageType.Korean) ? koreanFontSize : englishFontSize;
    }
}
