using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageText : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    [TextArea]
    public string koreanText;
    [TextArea]
    public string englishText;
    public void Init()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        BindText(LanguageManager.Instance.languageType);
    }
    public void BindText(LanguageType type)
    {
        textMeshPro.text = (type == LanguageType.Korean)? koreanText:englishText;
    }
}
