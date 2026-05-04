using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using UnityEngine;

public class ServerNetworkManager
{
    private TcpListener server;
    private List<TcpClient> clients = new();
    private ServerMessageHandler messageHandler;
    private RoomManager roomManager = new();

    // mapping
    private ConcurrentDictionary<TcpClient, string> clientPlayers = new();
    private ConcurrentDictionary<string, TcpClient> playerToClient = new();
    private ConcurrentDictionary<TcpClient, string> clientRooms = new();

    private bool isRunning = false;

    // ================= INIT =================
    public ServerNetworkManager(int port = 7777)
    {
        messageHandler = new ServerMessageHandler(
            roomManager,
            clientPlayers,
            playerToClient,
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

        Debug.Log("[SERVER] STARTING SERVER...");
        Debug.Log($"[SERVER] Listening on port {7777}");

        Thread acceptThread = new Thread(AcceptClients);
        acceptThread.Start();
    }

    // ================= ACCEPT CLIENT =================
    private void AcceptClients()
    {
        while (isRunning)
        {
            try
            {
                Debug.Log("[SERVER] Waiting for client...");
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("[SERVER] CLIENT CONNECTED FROM: " + client.Client.RemoteEndPoint);

                lock (clients)
                {
                    clients.Add(client);
                }

                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
                Debug.Log("[SERVER] Client thread started");
            }
            catch
            {
                if (!isRunning) break;
            }
        }
    }

    // ================= HANDLE CLIENT =================
    private void HandleClient(TcpClient client)
    {
        Debug.Log("[SERVER] HANDLE CLIENT START: " + client.Client.RemoteEndPoint);
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];
        StringBuilder sb = new StringBuilder();

        try
        {
            while (isRunning && client.Connected)
            {
                Debug.Log("[SERVER] Waiting data from client...");
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                Debug.Log("[SERVER] BYTES RECEIVED: " + bytesRead);
                if (bytesRead <= 0) break;

                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                Debug.Log("[SERVER] RAW MESSAGE BUFFER: " + sb.ToString());
                while (true)
                {
                    string content = sb.ToString();
                    int index = content.IndexOf('\n');

                    if (index < 0) break;

                    string message = content.Substring(0, index);
                    sb.Remove(0, index + 1);

                    messageHandler.HandleMessage(client, message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Client error: " + e.Message);
        }
        finally
        {
            DisconnectClient(client);
        }
    }

    // ================= DISCONNECT =================

    private void DisconnectClient(TcpClient client)
    {
        Debug.Log("[SERVER] DISCONNECT CLIENT: " + client.Client.RemoteEndPoint);

        if (clientPlayers.TryGetValue(client, out var playerId))
        {
            clientPlayers.TryRemove(client, out _);
            playerToClient.TryRemove(playerId, out _);

            // remove khỏi room
            if (clientRooms.TryGetValue(client, out var roomId))
            {
                var room = roomManager.GetRoom(roomId);
                room?.RemovePlayer(playerId);

                clientRooms.TryRemove(client, out _);
                if (roomId != null)
                {
                    SendToRoom(roomId, Protocol.CreateMessage(
                        Protocol.PLAYER_LEFT,
                        new { playerId = playerId }
                    ));
                }
            }
        }

        lock (clients)
        {
            clients.Remove(client);
        }
        Debug.Log("[SERVER] Client removed from list");
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

        Debug.Log("Server stopped");
    }

    // ================= SEND =================

    private void SendToClient(TcpClient client, string message)
    {
        Debug.Log("[SERVER] SEND TO CLIENT: " + message);
        try
        {
            if (!client.Connected)
                return;
            var stream = client.GetStream();
            var data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
        }
        catch
        {
            // ignore send error (client might have disconnected)
        }
    }

    private void SendToRoom(string roomId, string message)
    {
        var room = roomManager.GetRoom(roomId);
        if (room == null) return;

        var playersSnapshot = room.PlayerIds.ToList();
        Debug.Log($"[SERVER] SEND TO ROOM {roomId}, PLAYERS: {playersSnapshot.Count}");

        foreach (var playerId in playersSnapshot)
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
        clientPlayers.AddOrUpdate(client, playerId, (_, _) => playerId);
        playerToClient.AddOrUpdate(playerId, client, (_, _) => client);
    }

    public void BindRoom(TcpClient client, string roomId)
    {
        clientRooms.AddOrUpdate(client, roomId, (_, _) => roomId);
    }
}