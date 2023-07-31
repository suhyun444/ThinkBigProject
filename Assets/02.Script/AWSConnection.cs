using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

public class AWSConnection : MonoBehaviour
{
    public string poolID;
    public string region;
    DynamoDBContext context;
    AmazonDynamoDBClient client;
    private CognitoAWSCredentials credentials;
    private void Awake() {
        UnityInitializer.AttachToGameObject(this.gameObject);
        credentials = new CognitoAWSCredentials(poolID, RegionEndpoint.APNortheast2);
        client = new AmazonDynamoDBClient(credentials,RegionEndpoint.APNortheast2);
        context = new DynamoDBContext(client);
        //CreateItem();
        FindItem();
    }
    [DynamoDBTable("ranking_info")]
    public class Ranking
    {
        [DynamoDBHashKey] // Hash key.
        public string name { get; set; }
        [DynamoDBProperty]
        public int score { get; set; }
    }
    private void CreateItem()
    {
        Debug.Log("Create");
        Ranking r = new Ranking
        {
            name = "suhyun",
            score = 100,
        };
        context.SaveAsync(r,(result) => 
        {
            if(result.Exception == null)
                Debug.Log("Success");
            else
                Debug.Log(result.Exception);
        });
    }
    public void FindItem() //DB에서 캐릭터 정보 받기
    {
        ScanCondition[] conditions = new ScanCondition[1];
        conditions[0] = new ScanCondition("score",Amazon.DynamoDBv2.DocumentModel.ScanOperator.GreaterThanOrEqual,0);
        context.ScanAsync<Ranking>(conditions);
        // .GetNextSetAsync((AmazonDynamoDBResult<List<Ranking>> result)=>{
        //     if(result.Exception != null)
        //     {
        //         Debug.Log("ASDASDAS");
        //         Debug.LogException(result.Exception);
        //     }
        //     Debug.Log(result.Result.Count);
        // });
        // Debug.Log("Find");
        // Ranking c;
        // context.LoadAsync<Ranking>("guest1", (AmazonDynamoDBResult<Ranking> result) =>
        // {
        //     // id가 abcd인 캐릭터 정보를 DB에서 받아옴
        //     if (result.Exception != null)
        //     {
        //         Debug.LogException(result.Exception);
        //         return;
        //     }
        //     c = result.Result;
        //     Debug.Log(c.name); //찾은 캐릭터 정보 중 아이템 정보 출력
        // }, null);
    }
}
