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
    public string? Type { get; set; } // �޽��� ���� ("chat" �Ǵ� "gameControl")
    public string? Content { get; set; } // �޽��� ���� �Ǵ� ���� ���� ���
    public string? UserId { get; set; } // �������̵�...?
    public string? Role { get; set; }   // �й�� ���� ����
    public string? Result { get; set; } // ��ǥ��� ����.
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
        // ������ ������ URL
        var serverUri = "wss://5r5miqxusa.execute-api.ap-northeast-2.amazonaws.com/production?UserId=";
        //var serverUri = "wss://tu3k6aygs4.execute-api.ap-northeast-2.amazonaws.com/production?UserId=";
        sb.Append(serverUri);
        sb.Append(UserID);
        userName = UserID;
        try
        {
            await webSocket.ConnectAsync(new Uri(sb.ToString()), CancellationToken.None);

            Debug.Log($"������ ���� ���� : {webSocket.State == WebSocketState.Open}");

            Debug.Log("Before calling ReceiveMessages");
            var receiveTask = ReceiveMessages(webSocket);
            Debug.Log("After calling ReceiveMessages");

            await receiveTask;
        }
        catch (Exception ex)
        {
            Debug.Log($"Exception: {ex.Message}");
            // �̰��� ���ܸ� ó���ϴ� �Ϲ����� ������ �߰��ϼ���.
        }
        finally
        {
            // ���� ���� ó���� ���⿡ �߰��ϼ���.
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
                    // JSON �޽����� �Ľ��մϴ�.
                    var message = JsonConvert.DeserializeObject<Message>(messageJson);

                    // �޽��� ������ ���� ó���մϴ�.
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
        #region Test�� ���߿� �����
        //var serverUri = "wss://5r5miqxusa.execute-api.ap-northeast-2.amazonaws.com/production?UserId=" + userName;
        var serverUri = "wss://5r5miqxusa.execute-api.ap-northeast-2.amazonaws.com/production?UserId=abc";
        //var serverUri = "wss://tuwtdw5c40.execute-api.ap-northeast-2.amazonaws.com/production?UserId=" + userName;

        Debug.Log("Socket Server Disconnect �Ϸ�.");

        await webSocket.ConnectAsync(new Uri(serverUri), CancellationToken.None);
        //Console.WriteLine("Connected!");
        #endregion

        if (webSocket.State == WebSocketState.Open)
        {
            // ������ ������ ���������� �����մϴ�.
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
                // �� �� ���� �޽��� ���� ó��
                Console.WriteLine("Received unknown message type.");
                break;
        }
    }
}