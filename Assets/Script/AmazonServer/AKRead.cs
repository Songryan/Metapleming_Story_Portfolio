using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AKRead : MonoBehaviour
{
    private AKOutput akOutput;
    private string streamName = "MetapleStory"; // 스트림 이름
    private string shardId = "shardId-000000000000"; // 샤드 ID

    void Start()
    {
        // Kinesis Data Reader 인스턴스 생성
        //akOutput = new AKOutput(streamName, shardId);

        // 주기적으로 데이터를 읽기 위해 코루틴을 시작
        //StartCoroutine(ReadDataPeriodically());
    }

    // Kinesis에서 데이터를 주기적으로 읽는 코루틴
    IEnumerator ReadDataPeriodically()
    {
        // 원하는 주기로 설정 (예: 5초마다 데이터 읽기)
        var readInterval = 5.0f;
        string lastShardIterator = string.Empty;
        while (true)
        {
            // ReadDataAsync를 시작하고 완료되기를 기다림
            Task<string> readTask = akOutput.ReadDataAsync(lastShardIterator);
            yield return new WaitUntil(() => readTask.IsCompleted);

            // 필요한 경우, Task의 결과나 예외 처리
            if (readTask.Exception != null)
            {
                Debug.LogError(readTask.Exception);
            }

            // 다음 읽기까지 대기
            yield return new WaitForSeconds(readInterval);
        }
    }
}
