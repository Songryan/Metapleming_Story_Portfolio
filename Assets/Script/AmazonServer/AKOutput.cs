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
using UnityEngine; // Unity 엔진 사용을 위해 추가



public class AKOutput : MonoBehaviour
{
    public GameObject monster; // 몬스터 객체에 대한 참조

    private AmazonKinesisClient kinesisClient; // Kinesis 클라이언트 객체

    private string streamName = "MetapleStory"; // 스트림 이름

    private string shardId = "shardId-000000000000"; // 샤드 ID

    float readInterval = 0.1f;  // 원하는 주기로 설정 (예: 5초마다 데이터 읽기)

    float realTimedelay = -12.0f;  // 원하는 주기로 설정 (예: 5초마다 데이터 읽기)

    public TextMeshProUGUI chatText;
    public GameObject Wordballoon;

    void Start()
    {
        Amazon.AWSConfigs.AWSRegion = "ap-northeast-2";  // 사용하는 AWS 리전 설정
        //kinesisClient = new AmazonKinesisClient();

        // 인증 정보를 명시적으로 제공
        var credentials = new BasicAWSCredentials("", "");
        var config = new AmazonKinesisConfig
        {
            RegionEndpoint = RegionEndpoint.APNortheast2 // 원하는 AWS 리전 설정
        };

        kinesisClient = new AmazonKinesisClient(credentials, config);

        // 주기적으로 데이터를 읽기 위해 코루틴을 시작
        //Debug.Log($"AKOutput 시작");
        StartCoroutine(ReadDataPeriodically());
    }

    // Kinesis에서 데이터를 주기적으로 읽는 코루틴
    private IEnumerator ReadDataPeriodically()
    {
        //1초 딜레이로 시작
        //yield return new WaitForSeconds(1.0f);

        //int count = 1;
        string lastShardIterator = string.Empty;
        while (true)
        {
            // ReadDataAsync를 시작하고 완료되기를 기다림
            Task<string> readTask = ReadDataAsync(lastShardIterator);
            yield return new WaitUntil(() => readTask.IsCompleted);
            //Debug.Log($"AKOutput : {count}번째 / 성공여부 : {readTask.IsCompleted}");
            //count++;
            // 필요한 경우, Task의 결과나 예외 처리
            if (readTask.Exception != null)
            {
                Debug.LogError(readTask.Exception);
            }

            // 다음 읽기까지 대기
            yield return new WaitForSeconds(readInterval);
        }
    }

    // 현재 UTC 시간
    //DateTime currentUtcTime = DateTime.UtcNow.AddSeconds(-63);

    // Kinesis 스트림에서 데이터를 비동기적으로 읽는 메서드
    public async Task<string> ReadDataAsync(string lastShardIterator)
    {
        // 현재 UTC 시간
        //DateTime currentUtcTime = DateTime.UtcNow.AddSeconds(realTimedelay);

        // 이전에 받아둔 샤드 반복자가 없다면, 처음부터 시작합니다.
        string shardIterator = string.Empty;
        if (string.IsNullOrEmpty(lastShardIterator))
        {
            // 샤드 반복자를 얻기 위한 요청 생성
            var shardIteratorRequest = new GetShardIteratorRequest
            {
                StreamName = streamName,                        // 스트림 이름 지정
                ShardId = shardId,                              // 샤드 ID 지정
                ShardIteratorType = ShardIteratorType.LATEST// 설정한 시간 이후 데이터만 수집
                //Timestamp = currentUtcTime
            };
            //var shardIteratorResponse = await kinesisClient.GetShardIteratorAsync(shardIteratorRequest);
            try
            {
                // 샤드 반복자를 얻기 위한 비동기 요청 수행
                var shardIteratorResponse = await kinesisClient.GetShardIteratorAsync(shardIteratorRequest);
                // 데이터 처리 로직
                shardIterator = shardIteratorResponse.ShardIterator;
            }
            catch (AmazonKinesisException e)
            {
                if (e.ErrorCode == "InvalidArgumentException")
                {
                    Debug.LogError("제공된 타임스탬프가 서버의 현재 시간보다 미래입니다. 타임스탬프를 조정하세요.");
                }
                else
                {
                    Debug.LogError($"AWS Kinesis 오류: {e.Message}");
                }
            }
        }
        else
        {
            // 이전 작업에서 다음으로 읽을 샤드 반복자를 이어서 사용합니다.
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

        // 읽어온 레코드들을 순회하면서 데이터 처리
        foreach (var record in getRecordsResponse.Records)
        {
            string data = System.Text.Encoding.UTF8.GetString(record.Data.ToArray()); // 데이터를 문자열로 변환
            // 변환된 데이터를 콘솔(디버그 창)에 출력
            Debug.Log("Received data: " + data);
            // 싱글톤으로 관리
            SpawnProcess.Instance.ProcessReceivedData(data);
            ProcessMonsterData(data);
            //EventManager.TriggerDataReceived(data);
        }

        // 다음 샤드 반복자를 반환합니다. 이 값을 유지하고 다음 읽기에 사용해야 합니다.
        return nextShardIterator;
    }

    public void ProcessMonsterData(string data)
    {
        //var message = JsonConvert.DeserializeObject<ChatMessage>(data);

        //MessageDispenser(message);

        AKMessage receivedMessage = JsonUtility.FromJson<AKMessage>(data);

        // 몬스터 위치 업데이트
        monster.transform.position = new Vector2(receivedMessage.MonsterPosition.x, receivedMessage.MonsterPosition.y);

        int damage = SlimeMovement.Instance.health - receivedMessage.MonsterHP;

        // 몬스터 체력 업데이트
        if (damage > 0 && !SlimeMovement.Instance.isHit)
        {
            SlimeMovement.Instance.TakeDamage(damage);
        }
        else
        {
            // 체력 정보만 업데이트
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
