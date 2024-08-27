using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon;
using Amazon.Runtime;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Runtime.Internal;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class GameMessage
{
    public string? Type { get; set; } // 메시지 유형 ("chat" 또는 "gameControl")
    public string? Content { get; set; } // 메시지 내용 또는 게임 제어 명령
    public string? UserId { get; set; } // 메시지 내용 또는 게임 제어 명령
}
public class Function
{
    // DynamoDB 클라이언트를 초기화합니다. 이 클라이언트는 메시지를 DynamoDB 테이블에 저장하는 데 사용됩니다.
    private static AmazonDynamoDBClient client = new AmazonDynamoDBClient(new BasicAWSCredentials("", ""), RegionEndpoint.APNortheast2);
    private string tableName_M = "Messages";
    private string tableName_U = "Users";

    // Lambda 함수의 핸들러 메서드입니다. API Gateway를 통해 이 함수가 호출될 때 실행됩니다.
    // public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
    public async Task FunctionHandler(string[] input)
    {
        // 입력으로 받은 메시지의 본문을 추출합니다.
        string messageContent = input[1];

        GameMessage gamemessage = new GameMessage { Type = "chat", Content = messageContent , UserId = input[0]};

        // JSON으로 Parsing
        var jsonMessage = JsonConvert.SerializeObject(gamemessage);

        // 메시지 ID를 생성합니다. 각 메시지가 고유하도록 하기 위해 GUID를 사용합니다.
        var messageId = Guid.NewGuid().ToString();
        // 현재시간을 저장합니다.
        var timeStamp = DateTime.Now.ToString("HH:mm:ss");
        // 채팅방 이름
        var roomID = "A";
        // 메시지를 보낼 아이디
        var userID = input[0];

        // DynamoDB에 저장할 메시지를 생성합니다.
        var putItemRequest = new PutItemRequest
        {
            TableName = tableName_M,
            Item = new Dictionary<string, AttributeValue>
            {
                // 메시지 ID와 메시지 내용을 항목으로 추가합니다.
                { "MessageId", new AttributeValue { S = messageId }},
                { "Message", new AttributeValue { S = messageContent }},
                { "RoomId", new AttributeValue { S = roomID }},
                { "Timestamp", new AttributeValue { S = timeStamp }},
                { "UserId", new AttributeValue { S = userID }}
            }
        };
        // 생성한 메시지 항목을 DynamoDB 테이블에 비동기적으로 저장합니다.
        await client.PutItemAsync(putItemRequest).ConfigureAwait(false);

        // UserID를 받아오지 않고, 그냥 불특정 다수에게 뿌리기.
        List<Document> documentList = await GetUserID();

        var credentials = new BasicAWSCredentials("", "");
        var config = new AmazonApiGatewayManagementApiConfig
        {
            ServiceURL = "https://5r5miqxusa.execute-api.ap-northeast-2.amazonaws.com/production"
        };

        var apiClient = new AmazonApiGatewayManagementApiClient(credentials, config);

        //var apiClient = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
        //{
        //    ServiceURL = "https://5r5miqxusa.execute-api.ap-northeast-2.amazonaws.com/production/"
        //});

        foreach (var document in documentList)
        {
            string connectionId = document["ConnectionId"];
            //context.Logger.LogLine($"Sending message to ConnectionId: {connectionId}");
            Debug.Log($"Sending message to ConnectionId: {connectionId}");
            try
            {
                var messageToSend = Encoding.UTF8.GetBytes(jsonMessage);
                await apiClient.PostToConnectionAsync(new PostToConnectionRequest
                {
                    ConnectionId = connectionId,
                    Data = new MemoryStream(messageToSend)
                }).ConfigureAwait(false);
            }
            catch (AmazonServiceException e)
            {
                if (e.StatusCode == HttpStatusCode.Gone)
                {
                    // 클라이언트 연결이 끊긴 경우, DynamoDB에서 해당 연결 ID를 제거
                    //await RemoveConnectionId(connectionId);
                }
            }
        }
    }
    public async Task<List<Document>> GetUserID()
    {
        var table = Table.LoadTable(client, tableName_U);

        var scanFilter = new ScanFilter();
        var search = table.Scan(scanFilter);

        List<Document> documentList = new List<Document>();

        do
        {
            documentList.AddRange(await search.GetNextSetAsync().ConfigureAwait(false));
        }
        while (!search.IsDone);

        return documentList;
    }

}
