using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Message
{
    public string? Type { get; set; } // 메시지 유형 ("chat" 또는 "gameControl")
    public string? Content { get; set; } // 메시지 내용 또는 게임 제어 명령
    public string? UserId { get; set; } // 유저아이디...?
    public string? Role { get; set; }   // 분배된 역할 리턴
    public string? Result { get; set; } // 투표결과 리턴.
}

public class SocketServer
{
    public static string userName;
    private static ClientWebSocket webSocket = new ClientWebSocket();
    public static async Task Connect(string UserID)
    {
        if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.Connecting)
        {
            Debug.Log("WebSocket is already connected or connecting");
            return;
        }

        StringBuilder sb = new StringBuilder();
        // 웹소켓 서버의 URL
        var serverUri = "wss://5r5miqxusa.execute-api.ap-northeast-2.amazonaws.com/production?UserId=";
        //var serverUri = "wss://tu3k6aygs4.execute-api.ap-northeast-2.amazonaws.com/production?UserId=";
        sb.Append(serverUri);
        sb.Append(UserID);
        userName = UserID;
        try
        {
            await webSocket.ConnectAsync(new Uri(sb.ToString()), CancellationToken.None);

            Debug.Log($"웹소켓 연결 상태 : {webSocket.State == WebSocketState.Open}");

            Debug.Log("Before calling ReceiveMessages");
            var receiveTask = ReceiveMessages(webSocket);
            Debug.Log("After calling ReceiveMessages");

            await receiveTask;
        }
        catch (Exception ex)
        {
            Debug.Log($"Exception: {ex.Message}");
            // 이곳에 예외를 처리하는 일반적인 로직을 추가하세요.
        }
        finally
        {
            // 연결 종료 처리를 여기에 추가하세요.
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                Debug.Log("WebSocket disconnected.");
            }
        }

    }
    static async Task ReceiveMessages(ClientWebSocket webSocket)
    {
        Debug.Log("0");
        var buffer = new byte[1024];
        try
        {
            Debug.Log("1");
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
                Debug.Log("2");
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).ConfigureAwait(false);
                }
                else
                {
                    var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    // JSON 메시지를 파싱합니다.
                    var message = JsonConvert.DeserializeObject<Message>(messageJson);

                    // 메시지 유형에 따라 처리합니다.
                    socketFuction.MessageDispenser(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    public static async Task DisconnectAsync()
    {
        #region Test용 나중에 지우기
        //var serverUri = "wss://5r5miqxusa.execute-api.ap-northeast-2.amazonaws.com/production?UserId=" + userName;
        var serverUri = "wss://5r5miqxusa.execute-api.ap-northeast-2.amazonaws.com/production?UserId=abc";
        //var serverUri = "wss://tuwtdw5c40.execute-api.ap-northeast-2.amazonaws.com/production?UserId=" + userName;

        Debug.Log("Socket Server Disconnect 완료.");

        await webSocket.ConnectAsync(new Uri(serverUri), CancellationToken.None);
        //Console.WriteLine("Connected!");
        #endregion

        if (webSocket.State == WebSocketState.Open)
        {
            // 웹소켓 연결을 정상적으로 종료합니다.
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Console.WriteLine("WebSocket disconnected!");
        }
    }

}

public class socketFuction
{
    public static void MessageDispenser(Message message)
    {
        switch (message.Type)
        {
            case "chat":
                Debug.Log(message.Content);
                Debug.Log(message.UserId);
                break;
            default:
                // 알 수 없는 메시지 유형 처리
                Console.WriteLine("Received unknown message type.");
                break;
        }
    }
}