using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerAnimationState{
    Idle,
    Attack,
    Hitted,
}
[System.Serializable]
public struct EffectInfo{
    public Vector3 position;
    public GameObject prefab;
    public float duration;
    public float hitDelay;
}
public class Player : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float attackMotionDelay;
    [SerializeField] private EffectInfo effectInfo;
    [SerializeField] private GameObject hitParticle;
    private CostumeType costumeType;
    private Monster monster;
    private Vector3 hitParticlePivot;

    private void Awake() {
        animator = GetComponent<Animator>();
        costumeType = SaveManager.Instance.GetCostumeTypeData();
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
        StartCoroutine(Effect());
    }
    public void Hitted()
    {
        SoundManager.Instance.PlaySoundEffect(Sound.Hitted);
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

    private IEnumerator Effect()
    {
        yield return new WaitForSeconds(attackMotionDelay);
        GameObject effect = Instantiate(effectInfo.prefab,effectInfo.position,Quaternion.identity);
        SoundManager.Instance.PlaySoundEffect((Sound)System.Enum.Parse(typeof(Sound),costumeType.ToString()+"Attack"));
        Destroy(effect,effectInfo.duration);
        yield return new WaitForSeconds(effectInfo.hitDelay);
        monster.Hitted();
    }

    private void PlayAnimation(PlayerAnimationState state)
    {
        animator.Play(state.ToString());
    }
    public void BindMonster(Monster monster)
    {
        this.monster = monster;
    }
}
