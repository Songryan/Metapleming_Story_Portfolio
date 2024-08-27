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
            // Users 테이블에 대한 UpdateItem 요청 생성
            var request = new UpdateItemRequest
            {
                TableName = "Users",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "UserId", new AttributeValue { S = userId } } // 'UserId'는 파티션 키의 이름이어야 합니다.
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
                // 'Nickname' 필드가 비어있는 경우에만 업데이트합니다. 이미 값이 있다면 업데이트하지 않습니다.
                // 'attribute_not_exists' 조건을 사용하거나 'attribute_exists'와 함께 다른 조건을 사용할 수 있습니다.
                ConditionExpression = "attribute_not_exists(#N) OR #N = :empty",
                UpdateExpression = "SET #N = :n"
            };

            await client.UpdateItemAsync(request);

            return true;
        }
        catch (Exception ex)
        {
            // 오류 로깅 또는 처리
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
}

