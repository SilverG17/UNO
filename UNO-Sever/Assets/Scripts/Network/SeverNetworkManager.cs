using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerNetworkManager
{
    private TcpListener server;
    private List<TcpClient> clients = new();

    private MessageManager messageManager;
    private RoomManager roomManager = new();

    // mapping
    private Dictionary<TcpClient, string> clientPlayers = new();
    private Dictionary<string, TcpClient> playerToClient = new(); // 🔥 FIX O(n²)
    private Dictionary<TcpClient, string> clientRooms = new();

    private bool isRunning = false;

    // ================= INIT =================

    public ServerNetworkManager(int port = 7777)
    {
        this.messageManager = new MessageManager(
            roomManager,
            clientPlayers,
            clientRooms,
            SendToClient,
            SendToRoom
        );

        server = new TcpListener(IPAddress.Any, port);
    }

    // ================= START =================

    public void Start()
    {
        server.Start();
        isRunning = true;

        Console.WriteLine("Server started...");

        Thread acceptThread = new Thread(AcceptClients);
        acceptThread.Start();
    }

    // ================= ACCEPT CLIENT =================

    private void AcceptClients()
    {
        while (isRunning)
        {
            TcpClient client = server.AcceptTcpClient();

            lock (clients)
            {
                clients.Add(client);
            }

            Console.WriteLine("Client connected");

            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    // ================= HANDLE CLIENT =================

    private void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];
        StringBuilder sb = new StringBuilder();

        try
        {
            while (isRunning && client.Connected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                while (true)
                {
                    string content = sb.ToString();
                    int index = content.IndexOf('\n');

                    if (index < 0) break;

                    string message = content.Substring(0, index);
                    sb.Remove(0, index + 1);

                    // ✅ KHÔNG lock toàn bộ server nữa
                    messageManager.HandleMessage(client, message);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Client error: " + e.Message);
        }
        finally
        {
            DisconnectClient(client);
        }
    }

    // ================= DISCONNECT =================

    private void DisconnectClient(TcpClient client)
    {
        Console.WriteLine("Client disconnected");

        if (clientPlayers.TryGetValue(client, out var playerId))
        {
            clientPlayers.Remove(client);
            playerToClient.Remove(playerId); // 🔥 FIX leak

            // remove khỏi room
            if (clientRooms.TryGetValue(client, out var roomId))
            {
                var room = roomManager.GetRoom(roomId);
                room?.RemovePlayer(playerId);

                clientRooms.Remove(client);
            }
        }

        lock (clients)
        {
            clients.Remove(client);
        }

        client.Close();
    }

    // ================= STOP =================

    public void Stop()
    {
        isRunning = false;

        server.Stop();

        lock (clients)
        {
            foreach (var c in clients)
                c.Close();

            clients.Clear();
        }

        Console.WriteLine("Server stopped");
    }

    // ================= SEND =================

    private void SendToClient(TcpClient client, string message)
    {
        try
        {
            var stream = client.GetStream();
            var data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
        }
        catch
        {
            DisconnectClient(client);
        }
    }

    private void SendToRoom(string roomId, string message)
    {
        var room = roomManager.GetRoom(roomId);
        if (room == null) return;

        foreach (var playerId in room.PlayerIds)
        {
            if (playerToClient.TryGetValue(playerId, out var client))
            {
                SendToClient(client, message);
            }
        }
    }

    // ================= PUBLIC HELPERS =================
    // gọi từ MessageManager khi login / join

    public void BindPlayer(TcpClient client, string playerId)
    {
        clientPlayers[client] = playerId;
        playerToClient[playerId] = client;
    }

    public void BindRoom(TcpClient client, string roomId)
    {
        clientRooms[client] = roomId;
    }
}