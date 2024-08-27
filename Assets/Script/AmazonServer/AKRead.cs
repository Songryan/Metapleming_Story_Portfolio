using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AKRead : MonoBehaviour
{
    private AKOutput akOutput;
    private string streamName = "MetapleStory"; // ��Ʈ�� �̸�
    private string shardId = "shardId-000000000000"; // ���� ID

    void Start()
    {
        // Kinesis Data Reader �ν��Ͻ� ����
        //akOutput = new AKOutput(streamName, shardId);

        // �ֱ������� �����͸� �б� ���� �ڷ�ƾ�� ����
        //StartCoroutine(ReadDataPeriodically());
    }

    // Kinesis���� �����͸� �ֱ������� �д� �ڷ�ƾ
    IEnumerator ReadDataPeriodically()
    {
        // ���ϴ� �ֱ�� ���� (��: 5�ʸ��� ������ �б�)
        var readInterval = 5.0f;
        string lastShardIterator = string.Empty;
        while (true)
        {
            // ReadDataAsync�� �����ϰ� �Ϸ�Ǳ⸦ ��ٸ�
            Task<string> readTask = akOutput.ReadDataAsync(lastShardIterator);
            yield return new WaitUntil(() => readTask.IsCompleted);

            // �ʿ��� ���, Task�� ����� ���� ó��
            if (readTask.Exception != null)
            {
                Debug.LogError(readTask.Exception);
            }

            // ���� �б���� ���
            yield return new WaitForSeconds(readInterval);
        }
    }
}
