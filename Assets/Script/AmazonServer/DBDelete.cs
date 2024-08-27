using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


public class DBDelete
{
    private static AmazonDynamoDBClient _client = new AmazonDynamoDBClient(new BasicAWSCredentials("", ""), RegionEndpoint.APNortheast2);
    private readonly string _tableName;

    public DBDelete(string tableName)
    {
        _tableName = tableName;
    }

    public async Task DeleteItemAsync(string arry)
    {

        string[] arr = arry.Split("||");

        switch (_tableName)
        {
            case "RoomInfo":
                try
                {
                    var deleteItemRequest = new DeleteItemRequest
                    {
                        TableName = _tableName,
                        Key = new Dictionary<string, AttributeValue>
                    {
                        { "UserId", new AttributeValue { S = arr[0] } }
                    }
                    };

                    var response = await _client.DeleteItemAsync(deleteItemRequest).ConfigureAwait(false);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Item deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error deleting item: {response.HttpStatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting item: {ex.Message}");
                }
                break;
            case "RoleInfo":    //TestQA
                try
                {
                    var deleteItemRequest = new DeleteItemRequest
                    {
                        TableName = _tableName,
                        Key = new Dictionary<string, AttributeValue>
                    {
                        { "UserId", new AttributeValue { S = arr[0] } }
                    }
                    };

                    var response = await _client.DeleteItemAsync(deleteItemRequest).ConfigureAwait(false);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Item deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error deleting item: {response.HttpStatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting item: {ex.Message}");
                }
                break;
            case "Vote":
                foreach (var item in arr)
                {
                    try
                    {
                        var deleteItemRequest = new DeleteItemRequest
                        {
                            TableName = _tableName,
                            Key = new Dictionary<string, AttributeValue>
                            {
                                { "UserId", new AttributeValue { S = item } }
                            }
                        };

                        var response = await _client.DeleteItemAsync(deleteItemRequest).ConfigureAwait(false);

                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("Item deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Error deleting item: {response.HttpStatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting item: {ex.Message}");
                    }
                }
                break;
            case "SpecialVote":
                foreach (var item in arr)
                {
                    try
                    {
                        var deleteItemRequest = new DeleteItemRequest
                        {
                            TableName = _tableName,
                            Key = new Dictionary<string, AttributeValue>
                            {
                                { "UserRole", new AttributeValue { S = item } }
                            }
                        };

                        var response = await _client.DeleteItemAsync(deleteItemRequest).ConfigureAwait(false);

                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("Item deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Error deleting item: {response.HttpStatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting item: {ex.Message}");
                    }
                }
                break;
            case "IsAlive":
                foreach (var item in arr)
                {
                    try
                    {
                        var deleteItemRequest = new DeleteItemRequest
                        {
                            TableName = _tableName,
                            Key = new Dictionary<string, AttributeValue>
                            {
                                { "UserId", new AttributeValue { S = item } }
                            }
                        };

                        var response = await _client.DeleteItemAsync(deleteItemRequest).ConfigureAwait(false);

                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("Item deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Error deleting item: {response.HttpStatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting item: {ex.Message}");
                    }
                }
                break;
            case "MidS":
                foreach (var item in arr)
                {
                    try
                    {
                        var deleteItemRequest = new DeleteItemRequest
                        {
                            TableName = _tableName,
                            Key = new Dictionary<string, AttributeValue>
                            {
                                { "UserId", new AttributeValue { S = item } }
                            }
                        };

                        var response = await _client.DeleteItemAsync(deleteItemRequest).ConfigureAwait(false);

                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("Item deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Error deleting item: {response.HttpStatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting item: {ex.Message}");
                    }
                }
                break;
            default:
                break;
        }
    }
}
