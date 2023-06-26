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
        //Instantiate(effectObject,effectPosition,Quaternion.identity);
    }

    private void PlayAnimation(PlayerAnimationState state)
    {
        animator.Play(state.ToString());
    }
}
