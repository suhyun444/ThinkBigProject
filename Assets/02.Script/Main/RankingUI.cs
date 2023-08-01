using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshPro[] names;
    [SerializeField] private TextMeshPro[] scores;
    [SerializeField] private Sprite[] crownSprites;
    [SerializeField] private SpriteRenderer myRankSprite;
    [SerializeField] private TextMeshPro myRank;
    [SerializeField] private TextMeshPro myName;
    [SerializeField] private TextMeshPro myScore;
    [SerializeField] private CustomButton exitButton;
    AWSConnection awsConnection;
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<CustomButton>().BindClickEvent(OpenUI);
        exitButton.BindClickEvent(()=>ui.SetActive(false));
        awsConnection = GameObject.FindObjectOfType<AWSConnection>();
        UpdateRanking();
    }
    private void OpenUI()
    {
        ui.SetActive(true);
        UpdateRanking();
    }
    private void UpdateRanking()
    {
        List<Ranking> rankings = awsConnection.GetRankingList();
        rankings.Sort(new RankingComparer());
        for(int i=0;i<10;++i)
        {
            if(i >= rankings.Count)
            {
                names[i].text = "-";
                scores[i].text = "-";
            }
            else
            {
                names[i].text = rankings[i].name;
                scores[i].text = rankings[i].score.ToString("N0");
            }
        }
        string name = SaveManager.Instance.GetNameData();
        for(int i=0;i<rankings.Count;++i)
        {
            if(rankings[i].name == name)
            {
                if(i < 3)
                {
                    myRankSprite.sprite = crownSprites[i];
                    myRank.gameObject.SetActive(false);
                    myRankSprite.gameObject.SetActive(true);
                }
                else
                {
                    myRank.text = (i + 1).ToString("N0");
                    myRank.gameObject.SetActive(true);
                    myRankSprite.gameObject.SetActive(false);
                }
                myName.text = name;
                myScore.text = rankings[i].score.ToString("N0");
                return;
            }
        }
        myRankSprite.gameObject.SetActive(false);
        myRank.gameObject.SetActive(true);
        myRank.text = "-";
        myName.text = name;
        myScore.text = "-";
    }


}