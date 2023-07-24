using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Utils;

[System.Serializable]
struct BattleUI
{
    public Material staminaProgressMaterial;
    public TextMeshPro crystalText;
    public ComboText comboText;
    public TextMeshPro ScoreText;
}
[System.Serializable]
struct ResultUI
{
    public GameObject parentObject;
    public SpriteRenderer padeOutRenderer;
    public TextMeshPro scoreText;
    public TextMeshPro comboText;
    public TextMeshPro accuracyText;
    public TextMeshPro crystalText;
    public CustomButton exitButton;
}
[System.Serializable]
struct PauseUI
{
    public GameObject pauseObject;
    public CustomButton pauseButton; 
    public CustomButton resumeButton;
    public CustomButton exitButton;
}
public class BattleManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Monster monster;
    [SerializeField] private MathpidManager mathpidManager;
    [SerializeField] private BattleSketchManager sketchManager;
    [SerializeField] private SpriteRenderer fadeIn;
    private string answer;

    [SerializeField] private BattleUI battleUI;
    [SerializeField] private ResultUI resultUI;
    [SerializeField] private PauseUI pauseUI;
    private float leftTimeAmount = 1.0f;
    bool isInHitAnimation = false;
    private int maxCombo = 0;
    private int comboCount = 0;
    private int score = 0;
    private int totalCrystal;
    private int crystal = 0;
    private bool isEnd = false;
    private float correctCount = 0;
    private float problemCount = 0;
    private void Awake() {
        totalCrystal = SaveManager.Instance.GetCrystalData();
        InitPauseUI();
        StartCoroutine(FadeIn());
    }
    public void BindAnswer(string answer)
    {
        this.answer = answer;
        sketchManager.SetDrawType(answer.Contains("frac"));
    }
    private IEnumerator FadeIn()
    {
        fadeIn.gameObject.SetActive(true);
        float time = 0.0f;
        float t = 0.5f;
        while(time < 1)
        {
            time += Time.deltaTime /t;
            fadeIn.color = new Color(0,0,0,Mathf.Lerp(1,0,time));
            yield return null;
        }
        fadeIn.gameObject.SetActive(false);
    }
    private void Update() {
        if(!isInHitAnimation) leftTimeAmount -= (Time.deltaTime / Const.Battle.BATTLETIME) * (1.0f - (float)(Const.Skill.effectIncreaseAmount[(int)SkillType.Stamina] * SaveManager.Instance.GetSkillLevel(SkillType.Stamina)) * 0.01f);
        if(!isEnd && leftTimeAmount < 0)
        {
            isEnd = true;
            sketchManager.isEnd = true;
            StartCoroutine(LoadResultScene());
        }
        battleUI.staminaProgressMaterial.SetFloat("_FillAmount",leftTimeAmount);
        battleUI.crystalText.text = "x" +totalCrystal.ToString();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            mathpidManager.SelectAnswer(true);
        }
    }
    private void InitPauseUI()
    {
        pauseUI.pauseButton.BindClickEvent(PauseOn);
        pauseUI.resumeButton.BindClickEvent(PauseOff);
        pauseUI.exitButton.BindClickEvent(()=>StartCoroutine(LoadMainScene()));
    }
    private void PauseOn()
    {
        Time.timeScale = 0.0f;
        pauseUI.pauseObject.SetActive(true);
    }
    private void PauseOff()
    {
        Time.timeScale = 1.0f;
        pauseUI.pauseObject.SetActive(false);
    }
    private IEnumerator LoadResultScene()
    {
        yield return new WaitForSeconds(0.1f);
        float time = 0;
        float t = 1f;
        resultUI.padeOutRenderer.gameObject.SetActive(true);
        while(time < 1)
        {
            time += Time.deltaTime / t;
            resultUI.padeOutRenderer.color = new Color(0,0,0,Mathf.Lerp(0,130.0f/255.0f,time));
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        resultUI.scoreText.text = Util.SplitIntByComma(score);
        resultUI.comboText.text = ((LanguageManager.Instance.languageType == LanguageType.Korean) ? "최대 콤보 : " : "Max Combo : ") + maxCombo.ToString();
        resultUI.accuracyText.text = ((LanguageManager.Instance.languageType == LanguageType.Korean) ? "정확도 : " : "Accuracy : ") + ((problemCount == 0) ? "0" : ((int)((correctCount / problemCount) * 100)).ToString());
        resultUI.crystalText.text = "+" + crystal.ToString();
        resultUI.exitButton.BindClickEvent(()=>StartCoroutine(LoadMainScene()));
        SaveManager.Instance.SetCrystalData(totalCrystal);
        SaveManager.Instance.SetExpAmountData(100);
        SaveManager.Instance.SaveData();
        SaveManager.Instance.SaveMagicBookData();
        resultUI.parentObject.SetActive(true);
    }
    private IEnumerator LoadMainScene()
    {
        fadeIn.gameObject.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(0);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        float t = 0.5f;
        while (!op.isDone)
        {
            timer += Time.unscaledDeltaTime / t;
            fadeIn.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer));
            if (timer >= 1.0f)
            {
                sketchManager.Dispose();
                Time.timeScale = 1.0f;
                op.allowSceneActivation = true;
                yield break;
            }
            yield return null;
        }

    }
    public bool CheckAnswer(string answer)
    {
        bool isCorrect = this.answer == answer;
        if(!isCorrect)
        {
            AddMagicBookData();
            StartCoroutine(DecreaseStamina());
        }
        float solveTime = mathpidManager.SelectAnswer(isCorrect);
        player.Act(isCorrect);
        monster.Act(isCorrect,solveTime);
        Act(isCorrect,solveTime);
        return isCorrect;
    }
    private void AddMagicBookData()
    {
        string problem = monster.GetProblemText();
        SaveManager.Instance.AddMagicBookData(problem,answer);
    }
    private void Act(bool isCorrect,float solveTime)
    {
        problemCount++;
        if(isCorrect)
        {
            correctCount++;
            comboCount++;
            maxCombo = Mathf.Max(maxCombo,comboCount);
            if(comboCount >= 3)
                battleUI.comboText.ComboAnimation(comboCount);
            GetScore(solveTime);
            GetCrystal();
        }   
        else
        {
            battleUI.comboText.ResetCombo();
            comboCount = 0;
        }
        
    }
    private void GetCrystal()
    {
        int earnCrystal = (int)(10.0f * (1 + (float)(Const.Skill.effectIncreaseAmount[(int)SkillType.Combo] * SaveManager.Instance.GetSkillLevel(SkillType.Combo)) * 0.01f));
        crystal += earnCrystal;
        totalCrystal += earnCrystal;
    }
    private IEnumerator DecreaseStamina()
    {
        float decreaseAmount = (1.0f / 12.0f) * (1.0f - (float)(Const.Skill.effectIncreaseAmount[(int)SkillType.Defense] * SaveManager.Instance.GetSkillLevel(SkillType.Stamina)) * 0.01f);
        yield return new WaitForSeconds(0.4f);
        isInHitAnimation = true;
        float start = leftTimeAmount;
        float end = leftTimeAmount - decreaseAmount;
        float time = 0;
        float t = 0.3f;
        while(time < 1)
        {
            time += Time.deltaTime / t;
            leftTimeAmount = Mathf.Lerp(start,end,time);
            yield return null;
        }
        isInHitAnimation = false;
    }
    private void GetScore(float solveTime)
    {
        float plusScore = 50000;
        if(solveTime < 5)
            plusScore += 50000;
        else if(solveTime < 7)
            plusScore += 30000;
        else
            plusScore += 15000;
        //if(tag == "다항식") 몬스터에서 문제 정보 가져와서 판정
        plusScore *= (1.0f + (float)((comboCount) / 5)) * 1 + (float)(Const.Skill.effectIncreaseAmount[(int)SkillType.Combo] * SaveManager.Instance.GetSkillLevel(SkillType.Combo)) * 0.01f;
        score += (int)plusScore;
        battleUI.ScoreText.text = "Score : " + score.ToString();
    }
    
}
