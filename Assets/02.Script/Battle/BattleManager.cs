using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using CustomUtils;

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
    public GameObject backGround;
    public AnimationCurve backGroundAnimCurve;
    public GameObject titleText;
    public GameObject levelBG;
    public TextMeshPro levelText;
    public GameObject expBarObject;
    public Material expBar;
    public GameObject parentObject;
    public SpriteRenderer padeOutRenderer;
    public GameObject scoreHeader;
    public TextMeshPro scoreText;
    public TextMeshPro comboText;
    public TextMeshPro accuracyText;
    public GameObject crystalParent;
    public TextMeshPro crystalText;
    public GameObject exitButtonParent;
    public CustomButton exitButton;
    public GameObject crystalPrefab;
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
    private Player player;
    [SerializeField] private Monster monster;
    [SerializeField] private MathpidManager mathpidManager;
    [SerializeField] private BattleSketchManager sketchManager;
    [SerializeField] private SpriteRenderer fadeIn;
    private string answer;

    [SerializeField] private BattleUI battleUI;
    [SerializeField] private ResultUI resultUI;
    [SerializeField] private PauseUI pauseUI;
    private CameraShaking cameraShaking;
    private int earnExpAmount = 0;
    private float totalTime = 0.0f;
    private float leftTimeAmount = 1.0f;
    bool isInHitAnimation = false;
    private int maxCombo = 0;
    private int comboCount = 0;
    private int score = 0;
    private int totalCrystal;
    private int crystal = 0;
    private bool isEnd = false;
    private bool onLoadMainScene = false;
    private float correctCount = 0;
    private float problemCount = 0;
    private void Awake() {
        SoundManager.Instance.PlayBGM(Sound.BattleBGM);
        battleUI.ScoreText.text = "Score : 0";
        AWSConnection.Instance.FindPlayer(SaveManager.Instance.GetNameData());
        totalCrystal = SaveManager.Instance.GetCrystalData();
        SpawnPlayer();
        InitPauseUI();
        StartCoroutine(FadeIn());
        cameraShaking = Camera.main.GetComponent<CameraShaking>();
    }
    private void SpawnPlayer()
    {
        GameObject playerObject = (GameObject)Resources.Load<GameObject>("Players/"+SaveManager.Instance.GetCostumeTypeData().ToString());
        player = Instantiate(playerObject).GetComponent<Player>();
        player.BindMonster(monster);
        monster.BindPlayer(player);
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
        if(isEnd)return;
        totalTime += Time.deltaTime;
        if(!isInHitAnimation) leftTimeAmount -= (Time.deltaTime / Const.Battle.BATTLE_TIME) * (1.0f - (Const.Skill.EFFECT_INCREASE_AMOUNT[(int)SkillType.Stamina] * (float)SaveManager.Instance.GetSkillLevel(SkillType.Stamina)) * 0.01f);
        if(!isEnd && leftTimeAmount < 0)
        {
            StartCoroutine(LoadResultScene());
        }
        battleUI.staminaProgressMaterial.SetFloat("_FillAmount",leftTimeAmount);
        battleUI.crystalText.text = ": " +totalCrystal.ToString();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            mathpidManager.SelectAnswer(true);
        }
    }
    private void InitPauseUI()
    {
        pauseUI.pauseButton.BindClickEvent(PauseOn);
        pauseUI.resumeButton.BindClickEvent(PauseOff);
        pauseUI.exitButton.BindClickEvent(()=>StartCoroutine(LoadResultScene()));
        pauseUI.exitButton.AddClickEvent(PauseOff);
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
    private void SaveGame()
    {
        AWSConnection.Instance.UpdateScore(score);
        SaveManager.Instance.SetCrystalData(totalCrystal);
        int curLevel = SaveManager.Instance.GetLevelData();
        while (curLevel < 99 && Const.Skill.LEVEL_REQUIREMENT_EXP[curLevel] <= earnExpAmount)
        {
            earnExpAmount -= Const.Skill.LEVEL_REQUIREMENT_EXP[curLevel++];
            if (curLevel % 10 == 0)
            {
                crystal += 500;
                totalCrystal += 500;
            }
            else
            {
                crystal += 100;
                totalCrystal += 100;
            }
        }
        if (curLevel == 99) earnExpAmount = 1;
        SaveManager.Instance.SetLevelData(curLevel);
        SaveManager.Instance.SetExpAmountData(earnExpAmount);
        SaveManager.Instance.SaveData();
        SaveManager.Instance.SaveMagicBookData();
    }
    private IEnumerator LoadResultScene()
    {
        isEnd = true;
        sketchManager.isEnd = true;
        int bonusCrystal = (int)((float)crystal * (Const.Skill.EFFECT_INCREASE_AMOUNT[(int)SkillType.Crystal] * (float)SaveManager.Instance.GetSkillLevel(SkillType.Crystal)) * 0.01f);
        crystal += bonusCrystal;
        totalCrystal += bonusCrystal;
        earnExpAmount += (int)totalTime * 5;
        earnExpAmount = (int)(((float)earnExpAmount) * (1 + Const.Skill.EFFECT_INCREASE_AMOUNT[(int)SkillType.Exp] * (float)SaveManager.Instance.GetSkillLevel(SkillType.Exp) * 0.01f));
        float earnExpAmountForAnim = (float)earnExpAmount;
        SaveGame();
        yield return new WaitForSeconds(0.1f);
        float time = 0;
        float t = 0.2f;
        resultUI.parentObject.SetActive(true);
        while(time < 1)
        {
            time += Time.deltaTime / t;
            resultUI.backGround.transform.localScale = Vector3.LerpUnclamped(new Vector3(0.0f,0.0f,1.0f),new Vector3(8.0f,8.0f,1.0f),resultUI.backGroundAnimCurve.Evaluate(time));
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        resultUI.titleText.SetActive(true);
        yield return StartCoroutine(Util.HighlightObject(resultUI.titleText,resultUI.backGroundAnimCurve));
        yield return new WaitForSeconds(0.1f);
        resultUI.scoreText.text = Util.SplitIntByComma(score);
        resultUI.scoreHeader.SetActive(true);
        resultUI.scoreText.gameObject.SetActive(true);
        yield return StartCoroutine(Util.HighlightObjects(new GameObject[]{resultUI.scoreHeader,resultUI.scoreText.gameObject},resultUI.backGroundAnimCurve));
        yield return new WaitForSeconds(0.1f);
        int curLevel = SaveManager.Instance.GetLevelData();
        resultUI.levelText.text = curLevel.ToString();
        resultUI.expBar.SetFloat("_FillAmount", SaveManager.Instance.GetExpAmountData() / (float)Const.Skill.LEVEL_REQUIREMENT_EXP[curLevel]);
        resultUI.levelBG.SetActive(true);
        resultUI.levelText.gameObject.SetActive(true);
        resultUI.expBarObject.SetActive(true);
        float expAnimAmount = 0.0f;
        float curExp = (float)SaveManager.Instance.GetExpAmountData();
        float expAnimSpeed = 1000.0f;
        while(curLevel < 99 && expAnimAmount <= earnExpAmountForAnim)
        {
            float amount = Time.deltaTime * expAnimSpeed;
            if(amount > earnExpAmountForAnim - expAnimAmount)
            {
                curExp += (earnExpAmountForAnim - expAnimAmount);
                expAnimAmount = earnExpAmountForAnim;
            }
            curExp += amount;
            expAnimAmount += amount;
            if(curExp > Const.Skill.LEVEL_REQUIREMENT_EXP[curLevel])
            {
                curExp -= Const.Skill.LEVEL_REQUIREMENT_EXP[curLevel];
                curLevel++;
                resultUI.levelText.text = curLevel.ToString();
                StartCoroutine(Util.HighLightText(resultUI.levelText,15.2f));
                int getCrystalAmount = 0;
                if(curLevel % 10 == 0)
                {
                    getCrystalAmount = 500;
                }
                else
                {
                    getCrystalAmount = 100;
                }
                Crystal crystalObject = Instantiate(resultUI.crystalPrefab,new Vector3(1.5f,2.75f,1.0f),Quaternion.identity).GetComponent<Crystal>();
                crystalObject.transform.localScale = new Vector3(7.0f,7.0f);
                crystalObject.ChangeText("+"+getCrystalAmount.ToString()); 
                crystalObject.ChangeSortingOrder(110);
            }
            resultUI.expBar.SetFloat("_FillAmount",curExp / (float)Const.Skill.LEVEL_REQUIREMENT_EXP[curLevel]);
            yield return null;
        }
        if(curLevel == 99)curExp = 1;
        resultUI.levelText.text = curLevel.ToString();
        resultUI.expBar.SetFloat("_FillAmount",curExp / (float)Const.Skill.LEVEL_REQUIREMENT_EXP[curLevel]);
        yield return new WaitForSeconds(0.1f);

        resultUI.comboText.gameObject.SetActive(true);
        resultUI.comboText.text = ((LanguageManager.Instance.languageType == LanguageType.Korean) ? "최대 콤보 : " : "Max Combo : ") + maxCombo.ToString();
        resultUI.comboText.fontSize = ((LanguageManager.Instance.languageType == LanguageType.Korean) ?  7.5f : 5.75f);
        yield return StartCoroutine(Util.HighlightObject(resultUI.comboText.gameObject, resultUI.backGroundAnimCurve));

        resultUI.accuracyText.gameObject.SetActive(true);
        resultUI.accuracyText.text = ((LanguageManager.Instance.languageType == LanguageType.Korean) ? "정확도 : " : "Accuracy : ") + ((problemCount == 0) ? "0" : ((int)((correctCount / problemCount) * 100)).ToString());
        resultUI.accuracyText.fontSize = ((LanguageManager.Instance.languageType == LanguageType.Korean) ?  7.5f : 5.75f);
        yield return StartCoroutine(Util.HighlightObject(resultUI.accuracyText.gameObject, resultUI.backGroundAnimCurve));
        
        yield return new WaitForSeconds(0.1f);
        resultUI.crystalParent.SetActive(true);
        resultUI.crystalText.text = "+" + crystal.ToString();
        yield return StartCoroutine(Util.HighlightObject(resultUI.crystalParent, resultUI.backGroundAnimCurve));

        yield return new WaitForSeconds(0.1f);
        resultUI.exitButtonParent.gameObject.SetActive(true);
        resultUI.exitButton.BindClickEvent(()=>StartCoroutine(LoadMainScene()));
        yield return StartCoroutine(Util.HighlightObject(resultUI.exitButtonParent, resultUI.backGroundAnimCurve));

        
    }
    private IEnumerator LoadMainScene()
    {
        if(onLoadMainScene)yield break;
        onLoadMainScene = true;
        SoundManager.Instance.PlaySoundEffect(Sound.ButtonClick);
        fadeIn.gameObject.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(1);
        SoundManager.Instance.StopBGM();
        op.allowSceneActivation = false;
        float timer = 0.0f;
        float t = 0.5f;
        while (!op.isDone)
        {
            timer += Time.deltaTime / t;
            fadeIn.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer));
            if (timer >= 1.0f)
            {
                sketchManager.Dispose();
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
        }   
        else
        {
            battleUI.comboText.ResetCombo();
            comboCount = 0;
        }
        
    }
    public void GetReward()
    {
        crystal += 10;
        totalCrystal += 10;
        earnExpAmount += 100;
    }
    private IEnumerator DecreaseStamina()
    {
        float decreaseAmount = (1.0f / 12.0f) * (1.0f - (Const.Skill.EFFECT_INCREASE_AMOUNT[(int)SkillType.Defense] * (float)SaveManager.Instance.GetSkillLevel(SkillType.Defense)) * 0.01f);
        yield return new WaitForSeconds(0.4f);
        cameraShaking.ShakeScreen(1.0f,0.2f);
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
        float plusScore = 50000 * (1 + Const.Skill.EFFECT_INCREASE_AMOUNT[(int)SkillType.Kill]  * (float)SaveManager.Instance.GetSkillLevel(SkillType.Kill) * 0.01f);
        float speedBonus;
        if(solveTime < 2.5f)
            speedBonus = 50000;
        else if(solveTime < 4)
            speedBonus = 30000;
        else
            speedBonus = 15000;
        speedBonus *= (1 + Const.Skill.EFFECT_INCREASE_AMOUNT[(int)SkillType.Speed] * (float)SaveManager.Instance.GetSkillLevel(SkillType.Speed) * 0.01f);
        //if(tag == "다항식") 몬스터에서 문제 정보 가져와서 판정
        float comboBonus = 0;
        if(comboCount >= 5)
            comboBonus = 5000.0f *  ((((float)comboCount) / 5)) * (1 + (Const.Skill.EFFECT_INCREASE_AMOUNT[(int)SkillType.Combo] * (float)SaveManager.Instance.GetSkillLevel(SkillType.Combo)) * 0.01f);
        int prevScore = score;
        score += (int)plusScore + (int)speedBonus + (int)comboBonus;
        StartCoroutine(ScoreAnim(prevScore));
    }
    private IEnumerator ScoreAnim(int prevScore)
    {
        float time = 0;
        float t = 0.3f;
        while(time < 1)
        {
            time += Time.deltaTime / t;
            battleUI.ScoreText.text = "Score : " + ((int)Mathf.Lerp(prevScore,score,time)).ToString();
            yield return null;
        }
        battleUI.ScoreText.text = "Score : " + score.ToString();
    }
    
}
