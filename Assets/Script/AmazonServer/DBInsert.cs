using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class DBInsert
{
    private static AmazonDynamoDBClient _client = new AmazonDynamoDBClient(new BasicAWSCredentials("", ""), RegionEndpoint.APNortheast2);
    private readonly string _tableName;

    public DBInsert(string tableName)
    {
        _tableName = tableName;
    }

    public async Task InsertItemAsync(string userId, string arry)
    {
        string[] arr = arry.Split("||");

        switch (_tableName)
        {
            case "Vote":
                try
                {
                    var putItemRequest = new PutItemRequest
                    {
                        TableName = _tableName,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { "UserId", new AttributeValue { S = userId } },
                            { "Voted", new AttributeValue { S = arr[0] }}
                        }
                    };

                    var response = await _client.PutItemAsync(putItemRequest).ConfigureAwait(false);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Item inserted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error inserted item: {response.HttpStatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserted item: {ex.Message}");
                }
                break;
            case "IsAlive":
                try
                {
                    foreach (var UserId in arr)
                    {
                        var putItemRequest = new PutItemRequest
                        {
                            TableName = _tableName,
                            Item = new Dictionary<string, AttributeValue>
                            {
                                { "UserId", new AttributeValue { S = UserId }}
                            }
                        };

                        var response = await _client.PutItemAsync(putItemRequest).ConfigureAwait(false);

                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("Item inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Error inserted item: {response.HttpStatusCode}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserted item: {ex.Message}");
                }
                break;
            case "SpecialVote":
                try
                {
                    var putItemRequest = new PutItemRequest
                    {
                        TableName = _tableName,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { "UserRole", new AttributeValue { S = userId } },
                            { "Target", new AttributeValue { S = arr[0] }}
                        }
                    };

                    var response = await _client.PutItemAsync(putItemRequest).ConfigureAwait(false);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Item inserted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error inserted item: {response.HttpStatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserted item: {ex.Message}");
                }
                break;
            case "MidS":
                try
                {
                    var putItemRequest = new PutItemRequest
                    {
                        TableName = _tableName,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            { "UserId", new AttributeValue { S = userId } },
                        }
                    };

                    var response = await _client.PutItemAsync(putItemRequest).ConfigureAwait(false);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Item inserted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error inserted item: {response.HttpStatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserted item: {ex.Message}");
                }
                break;
            default:
                break;
        }


    }
}