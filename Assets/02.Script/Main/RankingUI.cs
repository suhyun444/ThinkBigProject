using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingUI : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshPro[] names;
    [SerializeField] private TextMeshPro[] scores;
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
        awsConnection = GetComponent<AWSConnection>();
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
            if(i >= rankings.Count || rankings[i].score == 0)
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
                myRank.text = (i + 1).ToString("N0");
                myName.text = name;
                myScore.text = rankings[i].score.ToString("N0");
                break;
            }
        }
    }


}
