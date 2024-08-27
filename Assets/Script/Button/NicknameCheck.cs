using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가


public class NicknameCheck : MonoBehaviour
{
    public TMP_InputField InputNickName;

    public AlertMessage alertMessage; // AlertMessage 컴포넌트 참조

    public void OnButtionClick()
    {
        string nickName = InputNickName.text.Trim();

        //Debug.Log("클릭");

        if (nickName.Equals(""))
        {
            alertMessage.ShowMessagePanel("닉네임을 입력해주세요.");
            return;
        }
        
        SceneChanger(PlayerManager.Instance.UserId,nickName);
        //SceneChanger("abc", nickName);
    }

    private async void SceneChanger(string userId, string nickName)
    {
        DynamoDBManager dynamoDBManager = new DynamoDBManager();

        bool result = await dynamoDBManager.InsertNickname(userId, nickName);


        // 결과에 따라 다른 처리를 실행합니다.
        switch (result)
        {
            case true:
                // Singleton 매니저에 아이디 저장
                PlayerManager.Instance.UserNickName = nickName;
                // 게임 씬 로드
                SceneManager.LoadScene("HuntingGround");
                break;
            case false:
                // 닉네임이 있을때
                alertMessage.ShowMessagePanel("해당 닉네임은 존재하는 닉네임입니다.");
                break;
            default:
                // 로그인 실패 메시지 표시
                alertMessage.ShowMessagePanel("서버동기에 실패했습니다.");
                break;
        }
    }

}


