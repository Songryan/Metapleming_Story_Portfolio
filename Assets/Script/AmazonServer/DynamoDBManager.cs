using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class DynamoDBManager
{
    private AmazonDynamoDBClient client;
    private DynamoDBContext context;

    private void CredentialsIdentify()
    {
        // 자격 증명 정보 하드코딩
        string accessKeyId = "";
        string secretAccessKey = "";
        string region = "ap-northeast-2";

        var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
        var awsConfig = new AmazonDynamoDBConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(region)
        };
        client = new AmazonDynamoDBClient(credentials, awsConfig);
        context = new DynamoDBContext(client);
    }

    public async Task<string> SaveUserData(Userinfo userData)
    {
        CredentialsIdentify();

        // 중복된 ID 확인
        Userinfo existingUser = await LoadUserData(userData.ID);
        if (existingUser != null)
        {
            Debug.LogError("Failed to save user data: ID already exists.");
            return "IDExists";
        }

        try
        {
            await context.SaveAsync(userData);
            Debug.Log("User data saved successfully!");
            return "SaveSuccess";
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save user data: " + e.Message);
            return "SaveFailed";
        }
    }

    public async Task<Userinfo> LoadUserData(string userId)
    {
        CredentialsIdentify();
        try
        {
            Userinfo userData = await context.LoadAsync<Userinfo>(userId);
            if (userData != null)
            {
                Debug.Log("User data loaded: " + userData.UserName);
            }
            return userData;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load user data: " + e.Message);
            return null;
        }
    }

    public async Task<string> ValidateLogin(string userId, string password)
    {
        CredentialsIdentify();
        Userinfo existingUser = await LoadUserData(userId);

        if (existingUser != null)
        {
            if (existingUser.UserPw == password)
            {
                if (string.IsNullOrEmpty(existingUser.UserName))
                {
                    Debug.Log("User does not have a nickname assigned.");
                    return "CreateNickname"; // or appropriate action for successful login
                }
                else
                {
                    Debug.Log("Password matches and user has a nickname.");
                    PlayerManager.Instance.UserNickName = existingUser.UserName;
                    return "HuntingGround"; // or appropriate action for successful login
                }
            }
            else
            {
                Debug.LogError("Password does not match.");
                return "PwFail";
            }
        }
        else
        {
            return "LoginFail";
        }
    }

    public async Task<string> CreateAccount(string userId, string password)
    {
        Userinfo newUser = new Userinfo
        {
            ID = userId,
            UserPw = password,
            UserName = ""
        };

        string saveResult = await SaveUserData(newUser);

        if (saveResult == "SaveSuccess")
        {
            Debug.Log("New user created and data saved.");
            return "CreateNickname";
        }
        else
        {
            Debug.LogError("Failed to save new user data.");
            return saveResult; // Return the specific error message from SaveUserData
        }
    }
    public async Task<bool> InsertNickname(string userId, string nickName)
    {
        CredentialsIdentify();

        // 닉네임이 이미 존재하는지 확인
        var search = new ScanCondition("UserName", ScanOperator.Equal, nickName);
        var searchResults = await context.ScanAsync<Userinfo>(new List<ScanCondition> { search }).GetRemainingAsync();

        if (searchResults.Count > 0)
        {
            Debug.LogError("Nickname already exists.");
            return false;
        }

        try
        {
            // 사용자 데이터를 불러와서 닉네임 업데이트
            Userinfo user = await LoadUserData(userId);
            if (user != null)
            {
                user.UserName = nickName;
                await context.SaveAsync(user);
                Debug.Log("Nickname updated successfully.");
                return true;
            }
            else
            {
                Debug.LogError("User not found.");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to update nickname: " + e.Message);
            return false;
        }
    }
}

[System.Serializable]
public class AWSConfig
{
    public string accessKeyId;
    public string secretAccessKey;
    public string region;
}

[DynamoDBTable("Userinfo")]
public class Userinfo
{
    [DynamoDBHashKey]
    public string ID { get; set; }

    [DynamoDBProperty]
    public string UserPw { get; set; }

    [DynamoDBProperty]
    public string UserName { get; set; }
}
