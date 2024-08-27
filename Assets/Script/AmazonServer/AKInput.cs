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
    public Vector2 Position; // 위치좌표
    public string UserId; // 유저아이디
    public string UserNickname; // 닉네임
    public int HP; // HP
    public int MP; // MP
    public Vector2 MonsterPosition; // 몬스터 위치
    public int MonsterHP; // 몬스터 체력
}

public class AKInput : MonoBehaviour
{
    public GameObject monster; // 몬스터 객체에 대한 참조

    private AmazonKinesisClient kinesisClient;

    private string streamName = "MetapleStory";  // AWS에서 설정한 Kinesis 스트림 이름

    float sendInterval = 0.1f;  // 원하는 주기로 설정 (예: 5초마다 데이터 보내기)

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

        // 주기적인 위치 정보 전송을 위한 코루틴 시작
        Debug.Log($"AKInput 시작");
        StartCoroutine(SendPlayerPositionPeriodically());
    }

    private IEnumerator SendPlayerPositionPeriodically()
    {
        //int count = 1;
        while (true)
        {
            // SendPositionData를 시작하고 완료되기를 기다림
            Task sendTask = SendPositionData();
            yield return new WaitUntil(() => sendTask.IsCompleted);
            //Debug.Log($"AKInput : {count}번째 / 성공여부 : {sendTask.IsCompleted}");
            //count++;
            // 필요한 경우, Task의 결과나 예외 처리
            if (sendTask.Exception != null)
            {
                Debug.LogError(sendTask.Exception);
            }

            // 전송 간격 설정 (예: 5초마다 전송)
            yield return new WaitForSeconds(sendInterval);
        }
    }

    private async Task SendPositionData()
    {
        //Vector2 position = transform.position;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 monsterPosition = monster.transform.position; // 몬스터의 위치 가져오기

        AKMessage akMessage = new AKMessage
        {
            Position = position,
            UserId = PlayerManager.Instance.UserId,
            UserNickname = PlayerManager.Instance.UserNickName,
            HP = 100,
            MP = 100,
            MonsterPosition = monsterPosition,
            MonsterHP = SlimeMovement.Instance.health // 예를 들어, 몬스터 체력을 임의로 설정
        };

        string data = JsonUtility.ToJson(akMessage); // 위치 정보를 JSON 형식으로 변환합니다.
        MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data));

        PutRecordRequest request = new PutRecordRequest
        {
            StreamName = streamName,
            Data = memoryStream,
            PartitionKey = "partitionKey" // 샤드 데이터를 분배하는 키
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