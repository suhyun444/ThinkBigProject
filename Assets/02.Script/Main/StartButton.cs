using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private Material progressMateiral;
    [SerializeField] private SpriteRenderer fadeOut;
    [SerializeField] private MagicBookUI magicBookUI;
    private float time;
    private readonly float needTimeToStart = 0.9f;
    private bool onPointer;
    private bool onEnter = false;
    // Start is called before the first frame update
    private void Awake()         
    {
        progressMateiral.SetFloat("_FillAmount",0.0f);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(onPointer && !onEnter)
        {  
            time += Time.deltaTime / needTimeToStart;
            progressMateiral.SetFloat("_FillAmount",time);
            if(time > 1.1f)
            {
                onEnter = true;
                StartCoroutine(LoadBattleScene());
            }
        }
    }
    private IEnumerator LoadBattleScene()
    {
        fadeOut.gameObject.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(1);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        float t = 0.5f;
        while (!op.isDone)
        {
            timer += Time.deltaTime / t;
            fadeOut.color = new Color(0,0,0,Mathf.Lerp(0,1,timer));
            if (timer >= 1.0f) 
            { 
                magicBookUI.Dispose();
                op.allowSceneActivation = true; 
                yield break; 
            }
            yield return null;
        }

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(!onEnter)
        {
            progressMateiral.SetFloat("_FillAmount",0.0f);
            onPointer = false;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        time = 0.0f;
        onPointer = true;
        Tutorial.Instance.Close();
        Tutorial.Instance.NextPage(4);
    }
}
