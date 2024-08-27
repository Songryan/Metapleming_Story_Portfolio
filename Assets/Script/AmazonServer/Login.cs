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
            // DynamoDB���� �񵿱������� �׸��� ��������, ���ؽ�Ʈ�� ĸó���� �ʽ��ϴ�.
            var document = await table.GetItemAsync(userId).ConfigureAwait(false);

            if (document != null)
            {
                //PW üũ
                string storedPassword = document["Password"];
                bool pwCheck = password == storedPassword;

                //�г��� üũ
                bool nickCheck = document.ContainsKey("NickName") && !string.IsNullOrEmpty(document["NickName"].AsString());

                if (pwCheck && nickCheck)
                {
                    PlayerManager.Instance.UserNickName = document["NickName"]; ;
                    // ������ ���� Scene����
                    return "HuntingGround";
                }else if (!pwCheck)    // pwCheck�� false��
                {
                    // pw�� ���������� PwFail��
                    return "PwFail";
                }

                // nickCheck false�� 
                return "CreateNickname";
            }
            else
            {
                // ����� ID�� ���̺� �������� ���� ��� ����� �߰�
                await AddUser(userId, password);
                Console.WriteLine($"New user added: {userId}");

                // �ű� �߰��� ����ڷμ� �α��� �������� �������� ���� �ʿ�
                return "CreateNickname";
            }
        }
        catch (Exception ex)
        {
            // ���� �α� �Ǵ� ó��
            Console.WriteLine(ex.ToString());
            return "False";
        }
    }
    private static async Task AddUser(string userId, string password)
    {
        var table = Table.LoadTable(client, "Users");

        var document = new Document();
        document["UserId"] = userId;
        document["Password"] = password; // ���� ���ø����̼ǿ����� ��й�ȣ�� �ؽ��Ͽ� �����ϴ� ���� �����ϴ�.

        await table.PutItemAsync(document);
    }
}

