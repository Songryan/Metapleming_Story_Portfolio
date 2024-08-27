using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public string UserId { get; set; }
    public string UserNickName { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Hierarchy에서 이 게임 오브젝트를 최상단 레벨로 설정
            transform.parent = null;

            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 방지
        }
    }


}
