using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �߰�


public class LoginCheck : MonoBehaviour
{
    public TMP_InputField idInputField;
    public TMP_InputField passwordInputField;

    public AlertMessage alertMessage; // AlertMessage ������Ʈ ����

    public void OnButtionClick() 
    {
        string userId = idInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        //Debug.Log("Ŭ��");

        if (userId.Equals(""))
        {
            alertMessage.ShowMessagePanel("���̵� �Է����ּ���.");
            return;
        }

        if (password.Equals(""))
        {
            alertMessage.ShowMessagePanel("�н����带 �Է����ּ���.");
            return;
        }

        SceneChanger(userId, password);
    }



    public async void OnCreateAccount()
    {
        string userId = idInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        //Debug.Log("Ŭ��");

        if (userId.Equals(""))
        {
            alertMessage.ShowMessagePanel("���̵� �Է����ּ���.");
            return;
        }

        if (password.Equals(""))
        {
            alertMessage.ShowMessagePanel("�н����带 �Է����ּ���.");
            return;
        }

        DynamoDBManager dynamoDBManager = new DynamoDBManager();
        
        string result = await dynamoDBManager.CreateAccount(userId, password);

        //string result = "CreateNickname";

        // ����� ���� �ٸ� ó���� �����մϴ�.
        switch (result)
        {
            case "IDExists":
                // �ߺ� ���̵� ���Խ�
                alertMessage.ShowMessagePanel("�ش� ���̵�� ������Դϴ�.");
                break;
            case "SaveFail":
                // �α��� ���� �޽��� ǥ��
                alertMessage.ShowMessagePanel("ȸ�����Կ� �����߽��ϴ�.");
                break;
            case "CreateNickname":
                PlayerManager.Instance.UserId = userId;
                // NickName�� ������ �г��� â����
                SceneManager.LoadScene("Nickname");
                break;
            default:
                // �α��� ���� �޽��� ǥ��
                alertMessage.ShowMessagePanel("�α��ο� �����߽��ϴ�.");
                break;
        }
    }

    private async void SceneChanger(string userId, string password)
    {
        DynamoDBManager dynamoDBManager = new DynamoDBManager();
        string result = await dynamoDBManager.ValidateLogin(userId, password);

        //string result = "CreateNickname";

        // ����� ���� �ٸ� ó���� �����մϴ�.
        switch (result)
        {
            case "HuntingGround":
                // Singleton �Ŵ����� ���̵� ����
                PlayerManager.Instance.UserId = userId;
                // ���� �� �ε�
                SceneManager.LoadScene("HuntingGround");
                break;
            case "PwFail":
                // �н����� ���� �޽��� ǥ��
                alertMessage.ShowMessagePanel("�н����尡 �ùٸ��� �ʽ��ϴ�.");
                break;
            case "LoginFail":
                // �α��� ���� �޽��� ǥ��
                alertMessage.ShowMessagePanel("���Ե��� ���� ���̵� �Դϴ�.");
                break;
            case "CreateNickname":
                PlayerManager.Instance.UserId = userId;
                // NickName�� ������ �г��� â����
                SceneManager.LoadScene("Nickname");
                break;
            default:
                // �α��� ���� �޽��� ǥ��
                alertMessage.ShowMessagePanel("�α��ο� �����߽��ϴ�.");
                break;
        }
    }
}
