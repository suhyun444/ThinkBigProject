using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pet : MonoBehaviour
{
    public int index = 0;
    [SerializeField] private CustomButton earnedButton;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isLeft = true;
    private DateTime lastEarnedTime;
    private float time = 0.0f;
    private float moveTerm;
    // Start is called before the first frame update
    void Start()
    {
        moveTerm = UnityEngine.Random.Range(2.0f,2.8f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastEarnedTime = SaveManager.Instance.GetLastEarnedTimeData(index);
        earnedButton.BindClickEvent(EarnCrystal);
    }

    // Update is called once per frame
    void Update()
    {
        DateTime now = DateTime.Now;
        TimeSpan ts = now - lastEarnedTime;
        if(ts.TotalSeconds > 10.0f)
        {
            earnedButton.gameObject.SetActive(true);
        }

        time += Time.deltaTime;
        if(time > moveTerm)
        {
            time = 0.0f;
            moveTerm = UnityEngine.Random.Range(2.0f, 2.8f);
            StartCoroutine(Move());
        }
    }
    private void EarnCrystal()
    {
        SoundManager.Instance.PlaySoundEffect(Sound.HarvestPetCrystal);
        int curCrystal = SaveManager.Instance.GetCrystalData();
        SaveManager.Instance.SetCrystalData(curCrystal + 10);
        SaveManager.Instance.SetLastEarnedTimeDate(index,DateTime.Now);
        SaveManager.Instance.SaveData();
        lastEarnedTime = DateTime.Now;
        earnedButton.gameObject.SetActive(false);
    }
    private Vector3 FindMoveDestination()
    {
        while(true)
        {
            float angle = UnityEngine.Random.Range(0,360);
            Vector3 destination = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),Mathf.Sin(angle * Mathf.Deg2Rad),0.0f) * UnityEngine.Random.Range(3.0f,4.0f);
            RaycastHit2D[] raycastHits2D = Physics2D.RaycastAll(destination, transform.forward, 999);
            for(int i=0;i<raycastHits2D.Length;i++)
            {
                if (raycastHits2D[i].transform.CompareTag("PetBoundary"))
                {
                    return destination;
                }
            }

        }
    }
    private void ChangeViewDirection(int direction)
    {
        if(!isLeft)direction *= -1;
        transform.localScale = new Vector3(direction * Math.Abs(transform.localScale.x),transform.localScale.y,1.0f);
    }
    private IEnumerator Move()
    {
        Vector3 start = transform.position;
        Vector3 end = FindMoveDestination();
        ChangeViewDirection((end.x - start.x < 0)?1:-1);
        float moveTime = 0.0f;
        float t = 1.0f;
        while(moveTime < 1)
        {
            moveTime += Time.deltaTime / t;
            transform.position = Vector3.Lerp(start,end,moveTime);
            yield return null;
        }
    }
}
