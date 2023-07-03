using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerAnimationState{
    Idle,
    Attack,
    Hitted,
}
public class Player : MonoBehaviour
{
    private Animator animator;
    public Vector3 effectPosition;
    public GameObject effectObject;
    [SerializeField] private Monster monster;
    [SerializeField] private GameObject hitParticle;
    private Vector3 hitParticlePivot;

    private void Awake() {
        animator = GetComponent<Animator>();
        hitParticlePivot = hitParticle.transform.position;
    }
    public void Act(bool isCorrect)
    {
        if(isCorrect)
        {
            StartCoroutine(Attack());
        }
    }
    public IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.25f);
        PlayAnimation(PlayerAnimationState.Attack);
    }
    public void Hitted()
    {
        PlayAnimation(PlayerAnimationState.Hitted);
        hitParticle.transform.localPosition = Random.insideUnitCircle * 0.1f;
        hitParticle.SetActive(true);
        StartCoroutine(DisableHitParticle());
    }
    public IEnumerator DisableHitParticle()
    {
        yield return new WaitForSeconds(0.5f);
        hitParticle.SetActive(false);
    }
    public void InsEffect()
    {
        StartCoroutine(effect());
    }
    private IEnumerator effect()
    {
        GameObject effect = Instantiate(effectObject,effectPosition,Quaternion.identity);
        Destroy(effect,0.5f);
        yield return new WaitForSeconds(0.3f);
        monster.Hitted();
    }

    private void PlayAnimation(PlayerAnimationState state)
    {
        animator.Play(state.ToString());
    }
}
