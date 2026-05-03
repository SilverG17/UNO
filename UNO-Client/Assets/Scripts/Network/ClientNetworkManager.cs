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

    private StringBuilder sb = new StringBuilder();

    public Action<string> OnMessageReceived;
    public Action OnConnected;
    public Action OnDisconnected;

    // ================= CONNECT =================

    public void Connect(string host, int port)
    {
        try
        {
            client = new TcpClient();
            client.Connect(host, port);

            stream = client.GetStream();
            isRunning = true;

            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("Connected to server");
            OnConnected?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError("Connect failed: " + e.Message);
        }
    }

    // ================= RECEIVE =================

    private void ReceiveLoop()
    {
        byte[] buffer = new byte[4096];

        try
        {
            while (isRunning)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead <= 0)
                    break;

                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                while (true)
                {
                    string content = sb.ToString();
                    int index = content.IndexOf('\n');

                    if (index < 0)
                        break;

                    string message = content.Substring(0, index);
                    sb.Remove(0, index + 1);

                    OnMessageReceived?.Invoke(message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Receive error: " + e.Message);
        }
        finally
        {
            DisconnectInternal();
        }
    }

    // ================= SEND =================

    public void Send(string message)
    {
        if (client == null || !client.Connected)
            return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Send error: " + e.Message);
            Disconnect();
        }
    }

    // ================= DISCONNECT =================

    public void Disconnect()
    {
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

        Debug.Log("Disconnected from server");
        OnDisconnected?.Invoke();
    }

    // ================= UNITY =================

    private void OnApplicationQuit()
    {
        Disconnect();
    }
}