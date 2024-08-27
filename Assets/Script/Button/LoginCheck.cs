using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가


public class LoginCheck : MonoBehaviour
{
    public TMP_InputField idInputField;
    public TMP_InputField passwordInputField;

    public AlertMessage alertMessage; // AlertMessage 컴포넌트 참조

    public void OnButtionClick() 
    {
        string userId = idInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        //Debug.Log("클릭");

        if (userId.Equals(""))
        {
            alertMessage.ShowMessagePanel("아이디를 입력해주세요.");
            return;
        }

        if (password.Equals(""))
        {
            alertMessage.ShowMessagePanel("패스워드를 입력해주세요.");
            return;
        }

        SceneChanger(userId, password);
    }



    public async void OnCreateAccount()
    {
        string userId = idInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        //Debug.Log("클릭");

        if (userId.Equals(""))
        {
            alertMessage.ShowMessagePanel("아이디를 입력해주세요.");
            return;
        }

        if (password.Equals(""))
        {
            alertMessage.ShowMessagePanel("패스워드를 입력해주세요.");
            return;
        }

        DynamoDBManager dynamoDBManager = new DynamoDBManager();
        
        string result = await dynamoDBManager.CreateAccount(userId, password);

        //string result = "CreateNickname";

        // 결과에 따라 다른 처리를 실행합니다.
        switch (result)
        {
            case "IDExists":
                // 중복 아이디 가입시
                alertMessage.ShowMessagePanel("해당 아이디는 사용중입니다.");
                break;
            case "SaveFail":
                // 로그인 실패 메시지 표시
                alertMessage.ShowMessagePanel("회원가입에 실패했습니다.");
                break;
            case "CreateNickname":
                PlayerManager.Instance.UserId = userId;
                // NickName만 없으면 닉네임 창으로
                SceneManager.LoadScene("Nickname");
                break;
            default:
                // 로그인 실패 메시지 표시
                alertMessage.ShowMessagePanel("로그인에 실패했습니다.");
                break;
        }
    }

    private async void SceneChanger(string userId, string password)
    {
        DynamoDBManager dynamoDBManager = new DynamoDBManager();
        string result = await dynamoDBManager.ValidateLogin(userId, password);

        //string result = "CreateNickname";

        // 결과에 따라 다른 처리를 실행합니다.
        switch (result)
        {
            case "HuntingGround":
                // Singleton 매니저에 아이디 저장
                PlayerManager.Instance.UserId = userId;
                // 게임 씬 로드
                SceneManager.LoadScene("HuntingGround");
                break;
            case "PwFail":
                // 패스워드 실패 메시지 표시
                alertMessage.ShowMessagePanel("패스워드가 올바르지 않습니다.");
                break;
            case "LoginFail":
                // 로그인 실패 메시지 표시
                alertMessage.ShowMessagePanel("가입되지 않은 아이디 입니다.");
                break;
            case "CreateNickname":
                PlayerManager.Instance.UserId = userId;
                // NickName만 없으면 닉네임 창으로
                SceneManager.LoadScene("Nickname");
                break;
            default:
                // 로그인 실패 메시지 표시
                alertMessage.ShowMessagePanel("로그인에 실패했습니다.");
                break;
        }
    }
}
