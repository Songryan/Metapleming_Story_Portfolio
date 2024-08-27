using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProcess : MonoBehaviour
{
    public static SpawnProcess Instance { get; private set; }

    public GameObject objectToSpawn;  // 스폰할 오브젝트의 프리팹
    public Transform spawnPoint;      // 스폰 위치

    // 유저별로 관리하기 위한 데이터
    public Dictionary<string, GameObject> players;

    void Awake()
    {
        players = new Dictionary<string, GameObject>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 객체를 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재한다면 중복 생성된 현재 객체를 파괴
        }
    }

    public void Start()
    {
        //string userId = PlayerManager.Instance.UserId;
        //string userId = "abc";
        // SocketServer 접속
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
        // SocketServer 접속 종료
        //await SocketServer.DisconnectAsync();
        // SocketServer 접속
        //await SocketServer.Connect(userId);
    }

    // 받은 데이터를 처리하는 메서드
    public void ProcessReceivedData(string jsonData)
    {
        // 받은 JSON 데이터를 AKMessage 객체로 변환
        AKMessage receivedData = JsonUtility.FromJson<AKMessage>(jsonData);
        //Debug.Log($"Processing data for UserId: {receivedData.UserId}");
        // 데이터를 기반으로 게임 오브젝트 업데이트
        UpdateGameObject(receivedData);
    }

    // 게임 오브젝트 업데이트 또는 생성
    void UpdateGameObject(AKMessage data)
    {
        //Debug.Log($"Updating/Creating object for UserId: {data.UserId}");
        //Debug.Log($"Updating/Creating object for UserId: {!players.ContainsKey(data.UserId)}");
        if (!PlayerManager.Instance.UserId.Equals(data.UserId))
        {
            if (!players.ContainsKey(data.UserId))
            {
                Debug.Log("Creating new player object.");
                // 새로운 플레이어 객체 생성
                GameObject player = Instantiate(objectToSpawn, data.Position, Quaternion.identity);
                players.Add(data.UserId, player);
                player.GetComponent<MultiPlayersController>().UpdatePlayer(data);
            }
            else
            {
                //Debug.Log("Updating existing player object.");
                // 이미 존재하는 플레이어의 위치 및 상태 업데이트
                GameObject player = players[data.UserId];
                //player.transform.position = data.Position;  // 위치 업데이트
                //player.transform.position = Vector2.Lerp(player.transform.position, data.Position, 3.0f * Time.deltaTime);
                player.GetComponent<MultiPlayersController>().UpdatePlayer(data);  // 데이터 업데이트
            }
        }
    }
}