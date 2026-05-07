using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientNetworkManager : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;

    private Thread receiveThread;
    private bool isRunning = false;

    private readonly StringBuilder sb = new StringBuilder();

    public Action<string> OnMessageReceived;
    public Action OnConnected;
    public Action OnDisconnected;
    public bool IsConnected => client != null && client.Connected;
    // ================= CONNECT =================
    public void Connect(string host, int port)
    {
        try
        {
            if (IsConnected)
            {
                Debug.Log("[CLIENT] ALREADY CONNECTED");
                return;
            }
            Debug.Log($"[CLIENT] CONNECTING TO {host}:{port}");

            client = new TcpClient();
            client.Connect(host, port);

            Debug.Log("[CLIENT] TCP CONNECT SUCCESS");

            stream = client.GetStream();
            isRunning = true;

            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("[CLIENT] RECEIVE THREAD STARTED");

            OnConnected?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError("[CLIENT] CONNECT FAILED: " + e.Message);
        }
    }

    // ================= RECEIVE =================
    private void ReceiveLoop()
    {
        byte[] buffer = new byte[4096];

        Debug.Log("[CLIENT] RECEIVE LOOP STARTED");

        try
        {
            while (isRunning)
            {
                Debug.Log("[CLIENT] WAITING FOR DATA...");

                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                Debug.Log("[CLIENT] BYTES RECEIVED: " + bytesRead);

                if (bytesRead <= 0)
                {
                    Debug.Log("[CLIENT] SERVER CLOSED CONNECTION");
                    break;
                }

                string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                sb.Append(chunk);

                Debug.Log("[CLIENT] RAW BUFFER: " + sb);

                while (true)
                {
                    string content = sb.ToString();
                    int index = content.IndexOf('\n');

                    if (index < 0)
                        break;

                    string message = content.Substring(0, index);
                    sb.Remove(0, index + 1);

                    Debug.Log("[CLIENT] MESSAGE: " + message);

                    OnMessageReceived?.Invoke(message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[CLIENT] RECEIVE ERROR: " + e.Message);
        }
        finally
        {
            DisconnectInternal();
        }
    }

    // ================= SEND =================
    public void Send(string message)
    {
        try
        {
            if (client == null || !client.Connected)
            {
                Debug.LogWarning("[CLIENT] SEND FAILED - NOT CONNECTED");
                return;
            }

            Debug.Log("[CLIENT] SEND: " + message);

            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[CLIENT] SEND ERROR: " + e.Message);
            Disconnect();
        }
    }

    // ================= DISCONNECT =================
    public void Disconnect()
    {
        Debug.Log("[CLIENT] DISCONNECT CALLED");
        isRunning = false;
        DisconnectInternal();
    }

    private void DisconnectInternal()
    {
        try
        {
            isRunning = false;

            stream?.Close();
            client?.Close();
        }
        catch { }

        Debug.Log("[CLIENT] DISCONNECTED FROM SERVER");

        OnDisconnected?.Invoke();
    }

    // ================= UNITY =================
    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
