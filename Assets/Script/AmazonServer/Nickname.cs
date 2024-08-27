using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;

public class Nickname
{
    private static AmazonDynamoDBClient client = new AmazonDynamoDBClient(new BasicAWSCredentials("", ""), RegionEndpoint.APNortheast2);
    public static async Task<bool> InsertNickname(string userId,string nickName)
    {
        try
        {
            // Users ���̺� ���� UpdateItem ��û ����
            var request = new UpdateItemRequest
            {
                TableName = "Users",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "UserId", new AttributeValue { S = userId } } // 'UserId'�� ��Ƽ�� Ű�� �̸��̾�� �մϴ�.
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#N", "Nickname"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":n", new AttributeValue { S = nickName }},
                    {":empty", new AttributeValue { S = "" }}
                },
                // 'Nickname' �ʵ尡 ����ִ� ��쿡�� ������Ʈ�մϴ�. �̹� ���� �ִٸ� ������Ʈ���� �ʽ��ϴ�.
                // 'attribute_not_exists' ������ ����ϰų� 'attribute_exists'�� �Բ� �ٸ� ������ ����� �� �ֽ��ϴ�.
                ConditionExpression = "attribute_not_exists(#N) OR #N = :empty",
                UpdateExpression = "SET #N = :n"
            };

            await client.UpdateItemAsync(request);

            return true;
        }
        catch (Exception ex)
        {
            // ���� �α� �Ǵ� ó��
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
}

