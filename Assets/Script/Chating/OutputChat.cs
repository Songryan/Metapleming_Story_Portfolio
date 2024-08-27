using System.Collections;
using UnityEngine;
using Amazon.Kinesis;
using Amazon.Runtime;
using Amazon;
using System.Text;
using Amazon.Kinesis.Model;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using TMPro;

public class OutputChat : MonoBehaviour
{
    private AmazonKinesisClient kinesisClient;
    private string streamName = "MetapleStory";
    private string shardId = "shardId-000000000000";
    private string shardIterator; // shardIterator �ɹ� ���� �߰�
    private string targetPartitionKey = "chatPartitionKey"; // ��� ��Ƽ�� Ű

    float sendInterval = 0.1f;  // ���ϴ� �ֱ�� ���� (��: 5�ʸ��� ������ ������)

    public TextMeshProUGUI chatText;
    public GameObject Wordballoon;

    void Start()
    {
        //�������
        Wordballoon.SetActive(false);

        InitializeKinesisClient();
        StartKinesisPolling();
    }

    void InitializeKinesisClient()
    {
        var credentials = new BasicAWSCredentials("", "");
        var config = new AmazonKinesisConfig
        {
            RegionEndpoint = RegionEndpoint.APNortheast2
        };
        kinesisClient = new AmazonKinesisClient(credentials, config);
    }

    void StartKinesisPolling()
    {
        StartCoroutine(ReceiveMessagesFromKinesis());
    }

    IEnumerator ReceiveMessagesFromKinesis()
    {
        while (true)
        {
            if (string.IsNullOrEmpty(shardIterator))
            {
                // Create the task to get a new shard iterator
                var shardIteratorRequest = new GetShardIteratorRequest
                {
                    StreamName = streamName,
                    ShardId = shardId,
                    ShardIteratorType = ShardIteratorType.LATEST
                };
                Task<GetShardIteratorResponse> shardIteratorTask = kinesisClient.GetShardIteratorAsync(shardIteratorRequest);
                yield return new WaitUntil(() => shardIteratorTask.IsCompleted);
                shardIterator = shardIteratorTask.Result.ShardIterator;
            }

            // Create the task to read Kinesis records
            var getRequest = new GetRecordsRequest { ShardIterator = shardIterator };
            Task<GetRecordsResponse> getRecordsTask = kinesisClient.GetRecordsAsync(getRequest);
            yield return new WaitUntil(() => getRecordsTask.IsCompleted);

            GetRecordsResponse getResponse = getRecordsTask.Result;
            foreach (Record record in getResponse.Records)
            {
                string messageJson = Encoding.UTF8.GetString(record.Data.ToArray());
                //Debug.Log("Received message: " + messageJson);
                Debug.Log("Received message : " + messageJson);

                var message = JsonConvert.DeserializeObject<ChatMessage>(messageJson);
                MessageDispenser(message);
            }

            shardIterator = getResponse.NextShardIterator;
            yield return new WaitForSeconds(sendInterval); // Set your polling interval here
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
