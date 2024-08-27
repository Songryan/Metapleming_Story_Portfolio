using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertMessage : MonoBehaviour, IOKNOBtn
{
    public GameObject messagePanel; // 알림창으로 사용할 패널
    public TextMeshProUGUI messageText; // 알림창 내의 메시지 텍스트 

    public void ShowMessagePanel(string message)
    {
        messageText.text = message; // 메시지 텍스트 설정
        messagePanel.SetActive(true); // 패널 표시
    }

    public void OnCancel()
    {
        messagePanel.SetActive(false); // 패널 숨김
    }

    public void OnConfirm()
    {
        messagePanel.SetActive(false); // 패널 숨김
    }
}
