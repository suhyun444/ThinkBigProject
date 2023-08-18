using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crystal : MonoBehaviour
{
    public Vector3 goalPosition;
    public SpriteRenderer sr;
    public TextMeshPro text;
    public AnimationCurve c;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Move());        
    }
    public void ChangeText(string s)
    {
        text.text = s;
    }
    public void ChangeSortingOrder(int order)
    {
        sr.sortingOrder = order;
        text.sortingOrder = order;
    }
    private IEnumerator Move()
    {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(start.x,start.y + 1.5f,start.z);
        // float startScale = transform.localScale.x;
        // float endScale = 0.2f;
        float time = 0;
        float t = 0.7f;
        while(time < 1)
        {
            time += Time.deltaTime / t;
            //float curScale = Mathf.Lerp(startScale,endScale,time);
            //transform.localScale = new Vector3(curScale,curScale,1);
            transform.position = Vector3.Lerp(start,end,time);
            sr.color = new Color(1,1,1,Mathf.Lerp(1,0,c.Evaluate(time)));
            text.color = new Color(1,1,1,Mathf.Lerp(1,0,c.Evaluate(time)));
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
