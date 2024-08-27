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
    private string shardIterator; // shardIterator 맴버 변수 추가
    private string targetPartitionKey = "chatPartitionKey"; // 대상 파티션 키

    float sendInterval = 0.1f;  // 원하는 주기로 설정 (예: 5초마다 데이터 보내기)

    public TextMeshProUGUI chatText;
    public GameObject Wordballoon;

    void Start()
    {
        //끄고시작
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
                // 알 수 없는 메시지 유형 처리
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
            // 딴새끼이면
            GameObject player = SpawnProcess.Instance.players[userid];
            player.GetComponent<MultiPlayersController>().chating(content);
        }
    }


    // 말풍선을 표시하고 일정 시간 후 비활성화하는 코루틴
    private IEnumerator DisplayWordBalloonCoroutine(string text, float delay)
    {
        chatText.text = text;
        Wordballoon.SetActive(true); // 말풍선 활성화
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        Wordballoon.SetActive(false); // 말풍선 비활성화
    }

    private void DisplayChatMessage(string message)
    {
        // 이전에 실행중인 말풍선 코루틴이 있다면 멈춤
        StopCoroutine("DisplayWordBalloonCoroutine");
        // 새로운 말풍선 코루틴 시작
        StartCoroutine(DisplayWordBalloonCoroutine(message, 3.0f));
    }

}
