using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageTextUI : LanguageText
{
    private TextMeshProUGUI textMeshProUI;
    // Start is called before the first frame update
    public override void Init()
    {
        textMeshProUI = GetComponent<TextMeshProUGUI>();
        BindText(LanguageManager.Instance.languageType);
    }
    public override void BindText(LanguageType type)
    {
        textMeshProUI.text = (type == LanguageType.Korean) ? koreanText : englishText;
        if (koreanFontSize != -1)
            textMeshProUI.fontSize = (type == LanguageType.Korean) ? koreanFontSize : englishFontSize;
    }
}
