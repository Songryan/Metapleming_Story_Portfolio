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
    public string Type; // 이 필드는 Kinesis에서 오는 데이터에 포함되어야 함
}

public class InputChat : MonoBehaviour
{
    public InputField chatInputField; // Inspector에서 할당
    public InputActionAsset inputActionsAsset; // InputActionAsset을 Inspector에서 할당

    private InputAction submitAction;
    private AmazonKinesisClient kinesisClient;
    private string streamName = "MetapleStory";  // AWS에서 설정한 Kinesis 스트림 이름

    private void Awake()
    {
        var chatActionMap = inputActionsAsset.FindActionMap("Chat", true);
        submitAction = chatActionMap.FindAction("Submit", true);

        // Kinesis 클라이언트 초기화
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

        // 새로 추가하는 onEndEdit 이벤트 리스너
        chatInputField.onEndEdit.AddListener(OnSubmitEndEdit);
    }

    private void OnDisable()
    {
        submitAction.performed -= OnSubmitPerformed;
        submitAction.Disable();

        // 새로 추가하는 onEndEdit 이벤트 리스너 해제
        chatInputField.onEndEdit.RemoveListener(OnSubmitEndEdit);
    }

    public void OnSubmitEndEdit(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            SendChatMessage(text);
            chatInputField.text = ""; // 메시지 전송 후 텍스트 필드를 비웁니다.
            chatInputField.DeactivateInputField();
        }
    }

    public void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        // 조합 문자열을 현재 텍스트에 추가하고 포커스를 이동시킵니다.
        if (!string.IsNullOrEmpty(Input.compositionString))
        {
            chatInputField.text += Input.compositionString;
            // 이 부분은 제거합니다. Input.compositionString = "";
        }
    
    
        // 메시지 전송 또는 입력 필드 활성화 로직
        if (!chatInputField.isFocused && string.IsNullOrEmpty(chatInputField.text))
        {
            chatInputField.ActivateInputField();
        }
        else if (chatInputField.isFocused && !string.IsNullOrEmpty(chatInputField.text))
        {
            SendChatMessage(chatInputField.text);
            chatInputField.text = ""; // 메시지 전송 후 텍스트 필드를 비웁니다.
            chatInputField.DeactivateInputField();
            //Debug.Log("전송");
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
    //            // 확정된 텍스트만 처리
    //            SendChatMessage(chatInputField.text);
    //            chatInputField.text = ""; // 메시지 전송 후 텍스트 필드를 비웁니다.
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
