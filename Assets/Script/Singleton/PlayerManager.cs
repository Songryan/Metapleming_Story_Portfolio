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

            // Hierarchy���� �� ���� ������Ʈ�� �ֻ�� ������ ����
            transform.parent = null;

            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
        }
    }


}
