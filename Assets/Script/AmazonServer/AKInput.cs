using UnityEngine;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Amazon.Runtime;
using Amazon;
using UnityEngine.UIElements;
using System.Threading;

[System.Serializable]
public class AKMessage
{
    public Vector2 Position; // ��ġ��ǥ
    public string UserId; // �������̵�
    public string UserNickname; // �г���
    public int HP; // HP
    public int MP; // MP
    public Vector2 MonsterPosition; // ���� ��ġ
    public int MonsterHP; // ���� ü��
}

public class AKInput : MonoBehaviour
{
    public GameObject monster; // ���� ��ü�� ���� ����

    private AmazonKinesisClient kinesisClient;

    private string streamName = "MetapleStory";  // AWS���� ������ Kinesis ��Ʈ�� �̸�

    float sendInterval = 0.1f;  // ���ϴ� �ֱ�� ���� (��: 5�ʸ��� ������ ������)

    void Start()
    {
        Amazon.AWSConfigs.AWSRegion = "ap-northeast-2";  // ����ϴ� AWS ���� ����
        //kinesisClient = new AmazonKinesisClient();

        // ���� ������ ��������� ����
        var credentials = new BasicAWSCredentials("", "");
        var config = new AmazonKinesisConfig
        {
            RegionEndpoint = RegionEndpoint.APNortheast2 // ���ϴ� AWS ���� ����
        };

        kinesisClient = new AmazonKinesisClient(credentials, config);

        // �ֱ����� ��ġ ���� ������ ���� �ڷ�ƾ ����
        Debug.Log($"AKInput ����");
        StartCoroutine(SendPlayerPositionPeriodically());
    }

    private IEnumerator SendPlayerPositionPeriodically()
    {
        //int count = 1;
        while (true)
        {
            // SendPositionData�� �����ϰ� �Ϸ�Ǳ⸦ ��ٸ�
            Task sendTask = SendPositionData();
            yield return new WaitUntil(() => sendTask.IsCompleted);
            //Debug.Log($"AKInput : {count}��° / �������� : {sendTask.IsCompleted}");
            //count++;
            // �ʿ��� ���, Task�� ����� ���� ó��
            if (sendTask.Exception != null)
            {
                Debug.LogError(sendTask.Exception);
            }

            // ���� ���� ���� (��: 5�ʸ��� ����)
            yield return new WaitForSeconds(sendInterval);
        }
    }

    private async Task SendPositionData()
    {
        //Vector2 position = transform.position;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 monsterPosition = monster.transform.position; // ������ ��ġ ��������

        AKMessage akMessage = new AKMessage
        {
            Position = position,
            UserId = PlayerManager.Instance.UserId,
            UserNickname = PlayerManager.Instance.UserNickName,
            HP = 100,
            MP = 100,
            MonsterPosition = monsterPosition,
            MonsterHP = SlimeMovement.Instance.health // ���� ���, ���� ü���� ���Ƿ� ����
        };

        string data = JsonUtility.ToJson(akMessage); // ��ġ ������ JSON �������� ��ȯ�մϴ�.
        MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data));

        PutRecordRequest request = new PutRecordRequest
        {
            StreamName = streamName,
            Data = memoryStream,
            PartitionKey = "partitionKey" // ���� �����͸� �й��ϴ� Ű
        };

        try
        {
            await kinesisClient.PutRecordAsync(request);
            //Debug.Log("Data sent: " + data);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}