using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertMessage : MonoBehaviour, IOKNOBtn
{
    public GameObject messagePanel; // �˸�â���� ����� �г�
    public TextMeshProUGUI messageText; // �˸�â ���� �޽��� �ؽ�Ʈ 

    public void ShowMessagePanel(string message)
    {
        messageText.text = message; // �޽��� �ؽ�Ʈ ����
        messagePanel.SetActive(true); // �г� ǥ��
    }

    public void OnCancel()
    {
        messagePanel.SetActive(false); // �г� ����
    }

    public void OnConfirm()
    {
        messagePanel.SetActive(false); // �г� ����
    }
}
