using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pet : MonoBehaviour
{
    [SerializeField] private CustomButton earnedButton;
    private SpriteRenderer spriteRenderer;
    private bool isLeft = true;
    private DateTime lastEarnedTime;
    private float time = 0.0f;
    private float moveTerm = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastEarnedTime = SaveManager.Instance.GetLastEarnedTimeData();
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
            StartCoroutine(Move());
        }
    }
    private void EarnCrystal()
    {
        int curCrystal = SaveManager.Instance.GetCrystalData();
        SaveManager.Instance.SetCrystalData(curCrystal + 10);
        SaveManager.Instance.SetLastEarnedTimeDate(DateTime.Now);
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
            RaycastHit2D raycastHit2D = Physics2D.Raycast(destination, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("PetBoundary"))
            {
                return destination;
            }

        }
    }
    private void ChangeViewDirection(bool isLeft)
    {
        spriteRenderer.flipX = isLeft;
    }
    private IEnumerator Move()
    {
        Vector3 start = transform.position;
        Vector3 end = FindMoveDestination();
        ChangeViewDirection((end.x - start.x < 0)?false:true);
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
