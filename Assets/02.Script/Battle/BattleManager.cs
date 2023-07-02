using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
struct BattleUI
{
    public Material staminaProgressMaterial;
    public TextMeshPro crystalText;
    public TextMeshPro ComboCountText;
    public TextMeshPro ComboText;
    public TextMeshPro ScoreText;
}
public class BattleManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Monster monster;
    [SerializeField] private MathpidManager mathpidManager;
    private string answer;

    [SerializeField] private BattleUI battleUI;
    private float leftTimeAmount = 1.0f;
    private int comboCount = 0;
    private int score = 0;
    private int totalCrystal;
    private int crystal = 0;

    
    private void Awake() {
        totalCrystal = SaveManager.Instance.GetCrystalData();
    }
    public void BindAnswer(string answer)
    {
        this.answer = answer;
    }
    private void Update() {
        leftTimeAmount -= Time.deltaTime / Const.Battle.BATTLETIME;
        battleUI.staminaProgressMaterial.SetFloat("_FillAmount",leftTimeAmount);
        battleUI.crystalText.text = "C " +totalCrystal.ToString();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            mathpidManager.SelectAnswer(true);
        }
    }
    public bool CheckAnswer(string answer)
    {
        bool isCorrect = this.answer == answer;
        float solveTime = mathpidManager.SelectAnswer(isCorrect);
        StartCoroutine(CorrectAnimation());
        monster.Act(isCorrect,solveTime);
        Act(isCorrect,solveTime);
        return isCorrect;
    }
    public IEnumerator CorrectAnimation()
    {
        yield return new WaitForSeconds(0.25f);
        player.Act(true);
    }
    private void Act(bool isCorrect,float solveTime)
    {
        if(isCorrect)
        {
            comboCount++;
            if(comboCount >= 3)
                ComboAnimation();
            GetScore(solveTime);
        }   
        
        else
        {
            battleUI.ComboCountText.gameObject.SetActive(false);
            battleUI.ComboText.gameObject.SetActive(false);
            comboCount = 0;
        }
        
    }
    private void GetScore(float solveTime)
    {
        int plusScore = 50000;
        if(solveTime < 5)
            plusScore += 50000;
        else if(solveTime < 7)
            plusScore += 30000;
        else
            plusScore += 15000;
        //if(tag == "다항식") 몬스터에서 문제 정보 가져와서 판정
        plusScore *= 1 + (int)((comboCount) / 5);
        score += plusScore;
        battleUI.ScoreText.text = "Score : " + score.ToString();
    }
    
    private void ComboAnimation()
    {
        battleUI.ComboCountText.gameObject.SetActive(true);
        battleUI.ComboText.gameObject.SetActive(true);
        battleUI.ComboCountText.text = comboCount.ToString();
        StartCoroutine(ComboHighLight());
    }
    private IEnumerator ComboHighLight()
    {
        float time = 0;
        float t = 0.1f;
        while(time < 1)
        {
            time += Time.deltaTime / t;
            battleUI.ComboCountText.fontSize = Mathf.Lerp(16.5f,23,time);
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
        while (time > 0)
        {
            time -= Time.deltaTime / t;
            battleUI.ComboCountText.fontSize = Mathf.Lerp(16.5f, 23, time);
            yield return null;
        }
    }
}
