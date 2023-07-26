using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public enum MonsterType
{
    Jar,
    End
}
public class Monster : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private QuestionBox questionBox;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private AnimationCurve particleCurve;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject crystal;
    private Player player;
    private MonsterType monsterType;
    private MaterialPropertyBlock dissolveMaterialBlock;
    private void Awake() {
        dissolveMaterialBlock = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(dissolveMaterialBlock);
    }
    public void Act(bool isCorrect,float solveTime)
    {
        if(isCorrect)questionBox.SetCorrectText(solveTime);
        else
        {
            StartCoroutine(Attack());
            questionBox.SetIncorrectText();
        } 
        
    }
    public void BindPlayer(Player player)
    {
        this.player = player;
    }
    private IEnumerator Attack()
    {
        animator.Play(monsterType.ToString() + "Attack");
        yield return new WaitForSeconds(Const.Battle.ATTACK_MOTION_DELAY[(int)monsterType]);
        player.Hitted();
        yield return new WaitForSeconds(Const.Battle.ATTACK_LEFT_MOTION[(int)monsterType]);
        questionBox.SetProblemText();
    }
    public void Hitted()
    {
        animator.Play(monsterType.ToString() + "Hitted");
        StartCoroutine(DeadAnimation());
    }
    public string GetProblemText(){return questionBox.GetProblemText();}
    private IEnumerator DeadAnimation()
    {
        yield return new WaitForSeconds(Const.Battle.DEAD_MOTION_DELAY[(int)monsterType]);
        float time = 0;
        particleSystem.Play();
        bool spawnCrystal = false;
        while(time < 1)
        {
            time += Time.deltaTime;
            dissolveMaterialBlock.SetFloat("_DissolveAmount",Mathf.Lerp(1,0,particleCurve.Evaluate(time)));
            spriteRenderer.SetPropertyBlock(dissolveMaterialBlock);
            var emission = particleSystem.emission;
            emission.rateOverTime = Mathf.Lerp(0,100,time / 0.5f);
            yield return null;
            if(time > 0.5f && !spawnCrystal)
            {
                Instantiate(crystal,transform.position  + new Vector3(Random.Range(-1.0f,1.0f),4.5f + Random.Range(-0.2f,0.2f),0),Quaternion.identity);
                spawnCrystal = true;
                battleManager.EarnCrystal();
            }
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Spawn());
    }
    private IEnumerator Spawn()
    {
        monsterType = (MonsterType)Random.Range(0,(int)MonsterType.End);
        animator.Play(monsterType.ToString() + "Idle");
        dissolveMaterialBlock.SetFloat("_DissolveAmount", 1);
        spriteRenderer.SetPropertyBlock(dissolveMaterialBlock);
        Vector3 start = new Vector3(4.37f,-4.48f,0.0f);
        Vector3 end = new Vector3(0,-0.4f,0.0f);
        spriteRenderer.transform.localPosition = start;
        float time = 0;
        float t = 0.25f;
        while(time < 1)
        {
            time += Time.deltaTime / t;
            spriteRenderer.transform.localPosition = Vector3.Lerp(start,end,time);
            yield return null;
        }
        questionBox.SetProblemText();
    }
}
