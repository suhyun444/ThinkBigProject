using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Utils;

public class MainManager : MonoBehaviour
{
    [SerializeField] private CustomButton startButton;
    [SerializeField] private TextMeshPro crystalText;
    [SerializeField] private Material expBarProgressMaterial;
    [SerializeField] private TextMeshPro levelText;
    // Start is called before the first frame update
    void Start()
    {
        levelText.text = SaveManager.Instance.GetLevelData().ToString();
        expBarProgressMaterial.SetFloat("_FillAmount",(float)SaveManager.Instance.GetExpAmountData() / 100.0f);
        startButton.BindClickEvent(()=>SceneManager.LoadScene(1));
    }

    // Update is called once per frame
    void Update()
    {
        crystalText.text = Util.SplitIntByComma(SaveManager.Instance.GetCrystalData());
    }
}
