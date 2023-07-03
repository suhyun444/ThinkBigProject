using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Monster : MonoBehaviour
{
    [SerializeField] private QuestionBox questionBox;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private AnimationCurve particleCurve;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
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
    private IEnumerator Attack()
    {
        animator.Play("JarAttack");
        yield return new WaitForSeconds(0.4f);
        player.Hitted();
        yield return new WaitForSeconds(0.6f);
        questionBox.SetProblemText();
    }
    public void Hitted()
    {
        animator.Play("JarHitted");
        StartCoroutine(DeadAnimation());
    }
    private IEnumerator DeadAnimation()
    {
        yield return new WaitForSeconds(0.45f);
        float time = 0;
        particleSystem.Play();
        while(time < 1)
        {
            time += Time.deltaTime;
            dissolveMaterialBlock.SetFloat("_DissolveAmount",Mathf.Lerp(1,0,particleCurve.Evaluate(time)));
            spriteRenderer.SetPropertyBlock(dissolveMaterialBlock);
            var emission = particleSystem.emission;
            emission.rateOverTime = Mathf.Lerp(0,100,time / 0.5f);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Spawn());
    }
    private IEnumerator Spawn()
    {
        animator.Play("JarIdle");
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
