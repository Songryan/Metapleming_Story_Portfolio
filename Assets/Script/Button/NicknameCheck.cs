using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �߰�


public class NicknameCheck : MonoBehaviour
{
    public TMP_InputField InputNickName;

    public AlertMessage alertMessage; // AlertMessage ������Ʈ ����

    public void OnButtionClick()
    {
        string nickName = InputNickName.text.Trim();

        //Debug.Log("Ŭ��");

        if (nickName.Equals(""))
        {
            alertMessage.ShowMessagePanel("�г����� �Է����ּ���.");
            return;
        }
        
        SceneChanger(PlayerManager.Instance.UserId,nickName);
        //SceneChanger("abc", nickName);
    }

    private async void SceneChanger(string userId, string nickName)
    {
        DynamoDBManager dynamoDBManager = new DynamoDBManager();

        bool result = await dynamoDBManager.InsertNickname(userId, nickName);


        // ����� ���� �ٸ� ó���� �����մϴ�.
        switch (result)
        {
            case true:
                // Singleton �Ŵ����� ���̵� ����
                PlayerManager.Instance.UserNickName = nickName;
                // ���� �� �ε�
                SceneManager.LoadScene("HuntingGround");
                break;
            case false:
                // �г����� ������
                alertMessage.ShowMessagePanel("�ش� �г����� �����ϴ� �г����Դϴ�.");
                break;
            default:
                // �α��� ���� �޽��� ǥ��
                alertMessage.ShowMessagePanel("�������⿡ �����߽��ϴ�.");
                break;
        }
    }

}


