using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine; // Unity ���� ����� ���� �߰�



public class AKOutput : MonoBehaviour
{
    public GameObject monster; // ���� ��ü�� ���� ����

    private AmazonKinesisClient kinesisClient; // Kinesis Ŭ���̾�Ʈ ��ü

    private string streamName = "MetapleStory"; // ��Ʈ�� �̸�

    private string shardId = "shardId-000000000000"; // ���� ID

    float readInterval = 0.1f;  // ���ϴ� �ֱ�� ���� (��: 5�ʸ��� ������ �б�)

    float realTimedelay = -12.0f;  // ���ϴ� �ֱ�� ���� (��: 5�ʸ��� ������ �б�)

    public TextMeshProUGUI chatText;
    public GameObject Wordballoon;

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

        // �ֱ������� �����͸� �б� ���� �ڷ�ƾ�� ����
        //Debug.Log($"AKOutput ����");
        StartCoroutine(ReadDataPeriodically());
    }

    // Kinesis���� �����͸� �ֱ������� �д� �ڷ�ƾ
    private IEnumerator ReadDataPeriodically()
    {
        //1�� �����̷� ����
        //yield return new WaitForSeconds(1.0f);

        //int count = 1;
        string lastShardIterator = string.Empty;
        while (true)
        {
            // ReadDataAsync�� �����ϰ� �Ϸ�Ǳ⸦ ��ٸ�
            Task<string> readTask = ReadDataAsync(lastShardIterator);
            yield return new WaitUntil(() => readTask.IsCompleted);
            //Debug.Log($"AKOutput : {count}��° / �������� : {readTask.IsCompleted}");
            //count++;
            // �ʿ��� ���, Task�� ����� ���� ó��
            if (readTask.Exception != null)
            {
                Debug.LogError(readTask.Exception);
            }

            // ���� �б���� ���
            yield return new WaitForSeconds(readInterval);
        }
    }

    // ���� UTC �ð�
    //DateTime currentUtcTime = DateTime.UtcNow.AddSeconds(-63);

    // Kinesis ��Ʈ������ �����͸� �񵿱������� �д� �޼���
    public async Task<string> ReadDataAsync(string lastShardIterator)
    {
        // ���� UTC �ð�
        //DateTime currentUtcTime = DateTime.UtcNow.AddSeconds(realTimedelay);

        // ������ �޾Ƶ� ���� �ݺ��ڰ� ���ٸ�, ó������ �����մϴ�.
        string shardIterator = string.Empty;
        if (string.IsNullOrEmpty(lastShardIterator))
        {
            // ���� �ݺ��ڸ� ��� ���� ��û ����
            var shardIteratorRequest = new GetShardIteratorRequest
            {
                StreamName = streamName,                        // ��Ʈ�� �̸� ����
                ShardId = shardId,                              // ���� ID ����
                ShardIteratorType = ShardIteratorType.LATEST// ������ �ð� ���� �����͸� ����
                //Timestamp = currentUtcTime
            };
            //var shardIteratorResponse = await kinesisClient.GetShardIteratorAsync(shardIteratorRequest);
            try
            {
                // ���� �ݺ��ڸ� ��� ���� �񵿱� ��û ����
                var shardIteratorResponse = await kinesisClient.GetShardIteratorAsync(shardIteratorRequest);
                // ������ ó�� ����
                shardIterator = shardIteratorResponse.ShardIterator;
            }
            catch (AmazonKinesisException e)
            {
                if (e.ErrorCode == "InvalidArgumentException")
                {
                    Debug.LogError("������ Ÿ�ӽ������� ������ ���� �ð����� �̷��Դϴ�. Ÿ�ӽ������� �����ϼ���.");
                }
                else
                {
                    Debug.LogError($"AWS Kinesis ����: {e.Message}");
                }
            }
        }
        else
        {
            // ���� �۾����� �������� ���� ���� �ݺ��ڸ� �̾ ����մϴ�.
            shardIterator = lastShardIterator;
        }

        var getRecordsRequest = new GetRecordsRequest
        {
            ShardIterator = shardIterator
        };

        var getRecordsResponse = await kinesisClient.GetRecordsAsync(getRecordsRequest);
        var nextShardIterator = getRecordsResponse.NextShardIterator;

        if (getRecordsResponse.Records.Count == 0)
        {
            //Debug.Log("No records received.");
            return getRecordsResponse.NextShardIterator;
        }

        // �о�� ���ڵ���� ��ȸ�ϸ鼭 ������ ó��
        foreach (var record in getRecordsResponse.Records)
        {
            string data = System.Text.Encoding.UTF8.GetString(record.Data.ToArray()); // �����͸� ���ڿ��� ��ȯ
            // ��ȯ�� �����͸� �ܼ�(����� â)�� ���
            Debug.Log("Received data: " + data);
            // �̱������� ����
            SpawnProcess.Instance.ProcessReceivedData(data);
            ProcessMonsterData(data);
            //EventManager.TriggerDataReceived(data);
        }

        // ���� ���� �ݺ��ڸ� ��ȯ�մϴ�. �� ���� �����ϰ� ���� �б⿡ ����ؾ� �մϴ�.
        return nextShardIterator;
    }

    public void ProcessMonsterData(string data)
    {
        //var message = JsonConvert.DeserializeObject<ChatMessage>(data);

        //MessageDispenser(message);

        AKMessage receivedMessage = JsonUtility.FromJson<AKMessage>(data);

        // ���� ��ġ ������Ʈ
        monster.transform.position = new Vector2(receivedMessage.MonsterPosition.x, receivedMessage.MonsterPosition.y);

        int damage = SlimeMovement.Instance.health - receivedMessage.MonsterHP;

        // ���� ü�� ������Ʈ
        if (damage > 0 && !SlimeMovement.Instance.isHit)
        {
            SlimeMovement.Instance.TakeDamage(damage);
        }
        else
        {
            // ü�� ������ ������Ʈ
            SlimeMovement.Instance.health = receivedMessage.MonsterHP;
        }
    }

    public void MessageDispenser(ChatMessage message)
    {
        switch (message.Type)
        {
            case "chat":
                chatDespenser(message.UserId, message.Content);
                break;
            default:
                // �� �� ���� �޽��� ���� ó��
                Console.WriteLine("Received unknown message type.");
                break;
        }
    }

    public void chatDespenser(string userid, string content)
    {
        if (userid.Equals(PlayerManager.Instance.UserId))
        {
            DisplayChatMessage(content);
        }
        else
        {
            // �������̸�
            GameObject player = SpawnProcess.Instance.players[userid];
            player.GetComponent<MultiPlayersController>().chating(content);
        }
    }


    // ��ǳ���� ǥ���ϰ� ���� �ð� �� ��Ȱ��ȭ�ϴ� �ڷ�ƾ
    private IEnumerator DisplayWordBalloonCoroutine(string text, float delay)
    {
        chatText.text = text;
        Wordballoon.SetActive(true); // ��ǳ�� Ȱ��ȭ
        yield return new WaitForSeconds(delay); // ������ �ð���ŭ ���
        Wordballoon.SetActive(false); // ��ǳ�� ��Ȱ��ȭ
    }

    private void DisplayChatMessage(string message)
    {
        // ������ �������� ��ǳ�� �ڷ�ƾ�� �ִٸ� ����
        StopCoroutine("DisplayWordBalloonCoroutine");
        // ���ο� ��ǳ�� �ڷ�ƾ ����
        StartCoroutine(DisplayWordBalloonCoroutine(message, 3.0f));
    }
}
