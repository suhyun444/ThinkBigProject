using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerAnimationState{
    Idle,
    Attack,
    Hit,
}
public class Player : MonoBehaviour
{
    private Animator animator;
    public Vector3 effectPosition;
    public GameObject effectObject;
    [SerializeField] private Monster monster;

    private void Awake() {
        animator = GetComponent<Animator>();
    }
    public void Act(bool isCorrect)
    {
        if(isCorrect)
        {
            Attack();
        }
    }
    public void Attack()
    {
        PlayAnimation(PlayerAnimationState.Attack);
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
