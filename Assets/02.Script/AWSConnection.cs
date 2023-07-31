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
        if(x.score == y.score) return String.Compare(x.name,y.name);
        if(x.score == y.score) return 0;
        else if(x.score > y.score) return -1;
        else return 1;
    }
}
public class AWSConnection : MonoBehaviour
{
    public string poolID;
    public string region;
    DynamoDBContext context;
    AmazonDynamoDBClient client;
    private CognitoAWSCredentials credentials;
    private void Awake() {
        credentials = new CognitoAWSCredentials(poolID, RegionEndpoint.APNortheast2);
        client = new AmazonDynamoDBClient(credentials,RegionEndpoint.APNortheast2);
        context = new DynamoDBContext(client);
    }
    private void SaveItem()
    {
        //string name = 
    }
    public List<Ranking> GetRankingList() //DB에서 캐릭터 정보 받기
    {
        ScanCondition[] conditions = new ScanCondition[1];
        conditions[0] = new ScanCondition("score",Amazon.DynamoDBv2.DocumentModel.ScanOperator.GreaterThanOrEqual,0);
        var result = context.ScanAsync<Ranking>(conditions).GetNextSetAsync();
        return result.Result;
    }
}
