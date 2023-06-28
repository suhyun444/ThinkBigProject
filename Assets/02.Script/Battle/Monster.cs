using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Monster : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private AnimationCurve particleCurve;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock dissolveMaterialBlock;
    private void Awake() {
        dissolveMaterialBlock = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(dissolveMaterialBlock);
    }
    public void Act(bool isCorrect)
    {
        Dead();
    }
    private void Attack()
    {

    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Q))
            Dead();
        if(Input.GetKeyDown(KeyCode.P))
        {
            dissolveMaterialBlock.SetFloat("_DissolveAmount", 1);
            spriteRenderer.SetPropertyBlock(dissolveMaterialBlock);
        }
    }
    private void Dead()
    {
        StartCoroutine(DeadAnimation());
    }
    private IEnumerator DeadAnimation()
    {
        float time = 0;
        particleSystem.Play();
        while(time < 1)
        {
            time += Time.deltaTime;
            dissolveMaterialBlock.SetFloat("_DissolveAmount",Mathf.Lerp(1,0,time));
            Debug.Log(dissolveMaterialBlock.GetFloat("_DissolveAmount"));
            spriteRenderer.SetPropertyBlock(dissolveMaterialBlock);
            var emission = particleSystem.emission;
            emission.rateOverTime = Mathf.Lerp(20,60,time / 0.5f);
            yield return null;
        }
    }
}
