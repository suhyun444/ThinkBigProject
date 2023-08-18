using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;

[DynamoDBTable("ranking_info")]
public class Ranking
{
    [DynamoDBHashKey] // Hash key.
    public string name { get; set; }
    [DynamoDBProperty]
    public int score { get; set; }
}
public class RankingComparer : IComparer<Ranking>
{
    public int Compare(Ranking x, Ranking y)
    {
        //if(x.score == y.score) return String.Compare(x.name,y.name);
        if(x.score == y.score) return 0;
        else if(x.score > y.score) return -1;
        else return 1;
    }
}
public class AWSConnection : Singleton<AWSConnection>
{
    public string poolID;
    DynamoDBContext context;
    AmazonDynamoDBClient client;
    private CognitoAWSCredentials credentials;
    private void Awake() {
        InitSingleTon(this);
        credentials = new CognitoAWSCredentials(poolID, RegionEndpoint.APNortheast2);
        client = new AmazonDynamoDBClient(credentials,RegionEndpoint.APNortheast2);
        context = new DynamoDBContext(client);
    }
    public bool FindPlayer(string name)
    {
        var result = context.LoadAsync<Ranking>(name);
        if(result.Result != null)
        {
            return true;
        }
        return false;
    }
    public void UpdateName(string prevName, string name)
    {
        int score = context.LoadAsync<Ranking>(prevName).Result.score;
        context.DeleteAsync<Ranking>(prevName);
        Ranking r = new Ranking{
            name = name,
            score = score,
        };
        SaveItem(r);
    }
    public void UpdateScore(int score)
    {
        string name= SaveManager.Instance.GetNameData();
        int maxScore = context.LoadAsync<Ranking>(name).Result.score;
        if(maxScore >= score)return;
        Ranking r = new Ranking
        {
            name = name,
            score = score,
        };
        SaveItem(r);
    }
    public void InitPlayer(string name)
    {
        Ranking r = new Ranking{
            name = name,
            score = 0,
        };
        SaveItem(r);
    }
    private void SaveItem(Ranking r)
    {
        context.SaveAsync<Ranking>(r);
    }
    public List<Ranking> GetRankingList() //DB에서 캐릭터 정보 받기
    {
        ScanCondition[] conditions = new ScanCondition[1];
        conditions[0] = new ScanCondition("score",Amazon.DynamoDBv2.DocumentModel.ScanOperator.GreaterThan,0);
        var result = context.ScanAsync<Ranking>(conditions).GetRemainingAsync();
        return result.Result;
    }
}
