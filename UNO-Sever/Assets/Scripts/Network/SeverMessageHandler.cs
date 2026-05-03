using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Collections.Concurrent;
using UnityEngine;

public class ServerMessageHandler
{
    private RoomManager roomManager;

    private ConcurrentDictionary<TcpClient, string> clientPlayers;
    private ConcurrentDictionary<string, TcpClient> playerToClient;
    private ConcurrentDictionary<TcpClient, string> clientRooms;

    private Action<TcpClient, string> sendToClient;
    private Action<string, string> sendToRoom;

    public ServerMessageHandler(
        RoomManager roomManager,
        ConcurrentDictionary<TcpClient, string> clientPlayers,
        ConcurrentDictionary<string, TcpClient> playerToClient,
        ConcurrentDictionary<TcpClient, string> clientRooms,
        Action<TcpClient, string> sendToClient,
        Action<string, string> sendToRoom)
    {
        this.roomManager = roomManager;
        this.clientPlayers = clientPlayers;
        this.playerToClient = playerToClient;
        this.clientRooms = clientRooms;
        this.sendToClient = sendToClient;
        this.sendToRoom = sendToRoom;
    }

    public void HandleMessage(TcpClient client, string json)
    {
        Wrapper wrapper;

        try
        {
            wrapper = Protocol.ParseWrapper(json);
        }
        catch
        {
            sendToClient(client, Protocol.CreateMessage(
                Protocol.ERROR,
                new { message = "Invalid JSON" }
            ));
            return;
        }

        switch (wrapper.type)
        {
            case Protocol.PLAY_CARD:
                HandlePlayCard(client, wrapper.payload);
                break;

            case Protocol.DRAW_CARD:
                HandleDrawCard(client);
                break;

            case Protocol.CALL_UNO:
                HandleCallUno(client);
                break;
        }
    }

    private void HandlePlayCard(TcpClient client, string payload)
    {
        PlayCardMsg msg;

        try
        {
            msg = Protocol.FromJson<PlayCardMsg>(payload);
        }
        catch
        {
            sendToClient(client, Protocol.CreateMessage(
                Protocol.ERROR,
                new { message = "Invalid payload" }
            ));
            return;
        }

        if (!clientPlayers.TryGetValue(client, out var playerId))
            return;

        if (!clientRooms.TryGetValue(client, out var roomId))
            return;

        var room = roomManager.GetRoom(roomId);
        if (room == null) return;

        lock (room)
        {
            // 🔥 FIX: check turn
            if (!room.GameManager.IsPlayerTurn(playerId))
            {
                sendToClient(client, Protocol.CreateMessage(
                    Protocol.ERROR,
                    new { message = "Not your turn" }
                ));
                return;
            }

            bool success = room.GameManager.PlayCard(playerId, msg.card);

            if (!success)
            {
                sendToClient(client, Protocol.CreateMessage(
                    Protocol.ERROR,
                    new { message = "Invalid move" }
                ));
                return;
            }

            sendToRoom(roomId, Protocol.CreateMessage(
                Protocol.GAME_STATE,
                room.GameManager.GetState()
            ));
        }
    }

    private void HandleDrawCard(TcpClient client)
    {
        if (!clientPlayers.TryGetValue(client, out var playerId)) return;
        if (!clientRooms.TryGetValue(client, out var roomId)) return;

        var room = roomManager.GetRoom(roomId);
        if (room == null) return;

        lock (room)
        {
            // 🔥 FIX: check turn
            if (!room.GameManager.IsPlayerTurn(playerId))
            {
                sendToClient(client, Protocol.CreateMessage(
                    Protocol.ERROR,
                    new { message = "Not your turn" }
                ));
                return;
            }

            room.GameManager.DrawCard(playerId);

            sendToRoom(roomId, Protocol.CreateMessage(
                Protocol.GAME_STATE,
                room.GameManager.GetState()
            ));
        }
    }

    private void HandleCallUno(TcpClient client)
    {
        if (!clientPlayers.TryGetValue(client, out var playerId)) return;
        if (!clientRooms.TryGetValue(client, out var roomId)) return;

        var room = roomManager.GetRoom(roomId);
        if (room == null) return;

        lock (room)
        {
            room.GameManager.CallUno(playerId);

            sendToRoom(roomId, Protocol.CreateMessage(
                Protocol.GAME_STATE,
                room.GameManager.GetState()
            ));
        }
    }
}