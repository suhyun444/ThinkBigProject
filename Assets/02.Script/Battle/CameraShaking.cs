using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    bool isShaking = false;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private LineRenderer[] fracHelper;
    private List<Vector3> fracLines = new List<Vector3>();
    private void Awake() {
        for (int i = 0; i < fracHelper.Length; ++i)
        {
            for (int j = 0; j < fracHelper[i].positionCount; ++j)
            {
                fracLines.Add(fracHelper[i].GetPosition(j));
            }
        }
    }
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
            AdjustFracHelperPosition();
            yield return null;
        }
        transform.position = new Vector3(0,0,-10);
        AdjustFracHelperPosition();
    }
    private void AdjustFracHelperPosition()
    {
        for(int i=0;i<fracHelper.Length;++i)
        {
            for(int j=0;j<fracHelper[i].positionCount;++j)
            {
                fracHelper[i].SetPosition(j, fracLines[i*2 + j] + new Vector3(transform.position.x,transform.position.y));
            }
        }
    }

}
