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
    public string? Type { get; set; } // �޽��� ���� ("chat" �Ǵ� "gameControl")
    public string? Content { get; set; } // �޽��� ���� �Ǵ� ���� ���� ���
    public string? UserId { get; set; } // �޽��� ���� �Ǵ� ���� ���� ���
}
public class Function
{
    // DynamoDB Ŭ���̾�Ʈ�� �ʱ�ȭ�մϴ�. �� Ŭ���̾�Ʈ�� �޽����� DynamoDB ���̺� �����ϴ� �� ���˴ϴ�.
    private static AmazonDynamoDBClient client = new AmazonDynamoDBClient(new BasicAWSCredentials("", ""), RegionEndpoint.APNortheast2);
    private string tableName_M = "Messages";
    private string tableName_U = "Users";

    // Lambda �Լ��� �ڵ鷯 �޼����Դϴ�. API Gateway�� ���� �� �Լ��� ȣ��� �� ����˴ϴ�.
    // public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
    public async Task FunctionHandler(string[] input)
    {
        // �Է����� ���� �޽����� ������ �����մϴ�.
        string messageContent = input[1];

        GameMessage gamemessage = new GameMessage { Type = "chat", Content = messageContent , UserId = input[0]};

        // JSON���� Parsing
        var jsonMessage = JsonConvert.SerializeObject(gamemessage);

        // �޽��� ID�� �����մϴ�. �� �޽����� �����ϵ��� �ϱ� ���� GUID�� ����մϴ�.
        var messageId = Guid.NewGuid().ToString();
        // ����ð��� �����մϴ�.
        var timeStamp = DateTime.Now.ToString("HH:mm:ss");
        // ä�ù� �̸�
        var roomID = "A";
        // �޽����� ���� ���̵�
        var userID = input[0];

        // DynamoDB�� ������ �޽����� �����մϴ�.
        var putItemRequest = new PutItemRequest
        {
            TableName = tableName_M,
            Item = new Dictionary<string, AttributeValue>
            {
                // �޽��� ID�� �޽��� ������ �׸����� �߰��մϴ�.
                { "MessageId", new AttributeValue { S = messageId }},
                { "Message", new AttributeValue { S = messageContent }},
                { "RoomId", new AttributeValue { S = roomID }},
                { "Timestamp", new AttributeValue { S = timeStamp }},
                { "UserId", new AttributeValue { S = userID }}
            }
        };
        // ������ �޽��� �׸��� DynamoDB ���̺� �񵿱������� �����մϴ�.
        await client.PutItemAsync(putItemRequest).ConfigureAwait(false);

        // UserID�� �޾ƿ��� �ʰ�, �׳� ��Ư�� �ټ����� �Ѹ���.
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
                    // Ŭ���̾�Ʈ ������ ���� ���, DynamoDB���� �ش� ���� ID�� ����
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
