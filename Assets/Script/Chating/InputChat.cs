using Amazon;
using Amazon.Kinesis;
using Amazon.Runtime;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Amazon.Kinesis.Model;

[System.Serializable]
public class ChatMessage
{
    public string UserId;
    public string Content;
    public string Type; // �� �ʵ�� Kinesis���� ���� �����Ϳ� ���ԵǾ�� ��
}

public class InputChat : MonoBehaviour
{
    public InputField chatInputField; // Inspector���� �Ҵ�
    public InputActionAsset inputActionsAsset; // InputActionAsset�� Inspector���� �Ҵ�

    private InputAction submitAction;
    private AmazonKinesisClient kinesisClient;
    private string streamName = "MetapleStory";  // AWS���� ������ Kinesis ��Ʈ�� �̸�

    private void Awake()
    {
        var chatActionMap = inputActionsAsset.FindActionMap("Chat", true);
        submitAction = chatActionMap.FindAction("Submit", true);

        // Kinesis Ŭ���̾�Ʈ �ʱ�ȭ
        var credentials = new BasicAWSCredentials("", "");
        var config = new AmazonKinesisConfig
        {
            RegionEndpoint = RegionEndpoint.APNortheast2
        };
        kinesisClient = new AmazonKinesisClient(credentials, config);
    }

    private void OnEnable()
    {
        submitAction.Enable();
        submitAction.performed += OnSubmitPerformed;

        // ���� �߰��ϴ� onEndEdit �̺�Ʈ ������
        chatInputField.onEndEdit.AddListener(OnSubmitEndEdit);
    }

    private void OnDisable()
    {
        submitAction.performed -= OnSubmitPerformed;
        submitAction.Disable();

        // ���� �߰��ϴ� onEndEdit �̺�Ʈ ������ ����
        chatInputField.onEndEdit.RemoveListener(OnSubmitEndEdit);
    }

    public void OnSubmitEndEdit(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            SendChatMessage(text);
            chatInputField.text = ""; // �޽��� ���� �� �ؽ�Ʈ �ʵ带 ���ϴ�.
            chatInputField.DeactivateInputField();
        }
    }

    public void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        // ���� ���ڿ��� ���� �ؽ�Ʈ�� �߰��ϰ� ��Ŀ���� �̵���ŵ�ϴ�.
        if (!string.IsNullOrEmpty(Input.compositionString))
        {
            chatInputField.text += Input.compositionString;
            // �� �κ��� �����մϴ�. Input.compositionString = "";
        }
    
    
        // �޽��� ���� �Ǵ� �Է� �ʵ� Ȱ��ȭ ����
        if (!chatInputField.isFocused && string.IsNullOrEmpty(chatInputField.text))
        {
            chatInputField.ActivateInputField();
        }
        else if (chatInputField.isFocused && !string.IsNullOrEmpty(chatInputField.text))
        {
            SendChatMessage(chatInputField.text);
            chatInputField.text = ""; // �޽��� ���� �� �ؽ�Ʈ �ʵ带 ���ϴ�.
            chatInputField.DeactivateInputField();
            //Debug.Log("����");
        }
    }

    //public void OnSubmitPerformed(InputAction.CallbackContext context)
    //{
    //    Debug.Log($"Composition String: {Input.compositionString}");
    //    Debug.Log($"InputField Text: {chatInputField.text}");
    //
    //    if (chatInputField.isFocused)
    //    {
    //        if (string.IsNullOrEmpty(Input.compositionString) && !string.IsNullOrEmpty(chatInputField.text))
    //        {
    //            // Ȯ���� �ؽ�Ʈ�� ó��
    //            SendChatMessage(chatInputField.text);
    //            chatInputField.text = ""; // �޽��� ���� �� �ؽ�Ʈ �ʵ带 ���ϴ�.
    //            chatInputField.DeactivateInputField();
    //        }
    //    }
    //    else
    //    {
    //        chatInputField.ActivateInputField();
    //    }
    //}

    private async void SendChatMessage(string message)
    {
        //Debug.Log("Chat message sent: " + message);

        ChatMessage cMessage = new ChatMessage
        {
            UserId = PlayerManager.Instance.UserId,
            Content = message,
            Type = "chat"
        };

        string jsonData = JsonUtility.ToJson(cMessage);
        MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
        PutRecordRequest request = new PutRecordRequest
        {
            StreamName = streamName,
            Data = memoryStream,
            PartitionKey = "chatPartitionKey"
        };

        try
        {
            await kinesisClient.PutRecordAsync(request);
            Debug.Log("Kinesis: Chat message sent");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send message to Kinesis: " + e.Message);
        }
    }
}
