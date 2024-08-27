using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using System.Threading.Tasks;

public class Login
{
    private static AmazonDynamoDBClient client = new AmazonDynamoDBClient(new BasicAWSCredentials("", ""), RegionEndpoint.APNortheast2);
    public static async Task<string> ValidateLogin(string userId, string password)
    {
        try
        {
            var table = Table.LoadTable(client, "Users");
            // DynamoDB에서 비동기적으로 항목을 가져오되, 컨텍스트를 캡처하지 않습니다.
            var document = await table.GetItemAsync(userId).ConfigureAwait(false);

            if (document != null)
            {
                //PW 체크
                string storedPassword = document["Password"];
                bool pwCheck = password == storedPassword;

                //닉네임 체크
                bool nickCheck = document.ContainsKey("NickName") && !string.IsNullOrEmpty(document["NickName"].AsString());

                if (pwCheck && nickCheck)
                {
                    PlayerManager.Instance.UserNickName = document["NickName"]; ;
                    // 같으면 게임 Scene으로
                    return "HuntingGround";
                }else if (!pwCheck)    // pwCheck가 false면
                {
                    // pw가 맞지않으면 PwFail로
                    return "PwFail";
                }

                // nickCheck false면 
                return "CreateNickname";
            }
            else
            {
                // 사용자 ID가 테이블에 존재하지 않을 경우 사용자 추가
                await AddUser(userId, password);
                Console.WriteLine($"New user added: {userId}");

                // 신규 추가된 사용자로서 로그인 성공으로 간주할지 결정 필요
                return "CreateNickname";
            }
        }
        catch (Exception ex)
        {
            // 오류 로깅 또는 처리
            Console.WriteLine(ex.ToString());
            return "False";
        }
    }
    private static async Task AddUser(string userId, string password)
    {
        var table = Table.LoadTable(client, "Users");

        var document = new Document();
        document["UserId"] = userId;
        document["Password"] = password; // 실제 애플리케이션에서는 비밀번호를 해시하여 저장하는 것이 좋습니다.

        await table.PutItemAsync(document);
    }
}

