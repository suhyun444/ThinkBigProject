using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    bool isShaking = false;
    [SerializeField] private AnimationCurve curve;
    public void ShakeScreen(float strength,float duration)
    {
        StartCoroutine(Shaking(strength,duration));
    }
    private IEnumerator Shaking(float strength,float duration)
    {
        if(isShaking)yield break;
        float time = 0;
        while(time < 1.0f)
        {
            time += Time.deltaTime / duration;
            transform.position = new Vector3(0,0,-10) + (Vector3)Random.insideUnitCircle * curve.Evaluate(time) * strength; 
            yield return null;
        }
        transform.position = new Vector3(0,0,-10);
    }

}
