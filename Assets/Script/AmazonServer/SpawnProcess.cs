using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProcess : MonoBehaviour
{
    public static SpawnProcess Instance { get; private set; }

    public GameObject objectToSpawn;  // ������ ������Ʈ�� ������
    public Transform spawnPoint;      // ���� ��ġ

    // �������� �����ϱ� ���� ������
    public Dictionary<string, GameObject> players;

    void Awake()
    {
        players = new Dictionary<string, GameObject>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ü�� �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����Ѵٸ� �ߺ� ������ ���� ��ü�� �ı�
        }
    }

    public void Start()
    {
        //string userId = PlayerManager.Instance.UserId;
        //string userId = "abc";
        // SocketServer ����
        //ConnectionProcess(userId);

        //
        //StartCoroutine(SpawnAfterDelay());
    }

/*    void OnEnable()
    {
        EventManager.OnDataReceived += ProcessReceivedData;
    }

    void OnDisable()
    {
        EventManager.OnDataReceived -= ProcessReceivedData;
    }*/

    public async void ConnectionProcess(string userId)
    {
        // SocketServer ���� ����
        //await SocketServer.DisconnectAsync();
        // SocketServer ����
        //await SocketServer.Connect(userId);
    }

    // ���� �����͸� ó���ϴ� �޼���
    public void ProcessReceivedData(string jsonData)
    {
        // ���� JSON �����͸� AKMessage ��ü�� ��ȯ
        AKMessage receivedData = JsonUtility.FromJson<AKMessage>(jsonData);
        //Debug.Log($"Processing data for UserId: {receivedData.UserId}");
        // �����͸� ������� ���� ������Ʈ ������Ʈ
        UpdateGameObject(receivedData);
    }

    // ���� ������Ʈ ������Ʈ �Ǵ� ����
    void UpdateGameObject(AKMessage data)
    {
        //Debug.Log($"Updating/Creating object for UserId: {data.UserId}");
        //Debug.Log($"Updating/Creating object for UserId: {!players.ContainsKey(data.UserId)}");
        if (!PlayerManager.Instance.UserId.Equals(data.UserId))
        {
            if (!players.ContainsKey(data.UserId))
            {
                Debug.Log("Creating new player object.");
                // ���ο� �÷��̾� ��ü ����
                GameObject player = Instantiate(objectToSpawn, data.Position, Quaternion.identity);
                players.Add(data.UserId, player);
                player.GetComponent<MultiPlayersController>().UpdatePlayer(data);
            }
            else
            {
                //Debug.Log("Updating existing player object.");
                // �̹� �����ϴ� �÷��̾��� ��ġ �� ���� ������Ʈ
                GameObject player = players[data.UserId];
                //player.transform.position = data.Position;  // ��ġ ������Ʈ
                //player.transform.position = Vector2.Lerp(player.transform.position, data.Position, 3.0f * Time.deltaTime);
                player.GetComponent<MultiPlayersController>().UpdatePlayer(data);  // ������ ������Ʈ
            }
        }
    }
}