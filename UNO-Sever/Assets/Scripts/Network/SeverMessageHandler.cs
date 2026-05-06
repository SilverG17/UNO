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
            SendError(client, "Invalid JSON");
            return;
        }

        switch (wrapper.type)
        {
            case Protocol.CREATE_ROOM:
                HandleCreateRoom(client, wrapper.payload);
                break;

            case Protocol.JOIN_ROOM:
                HandleJoinRoom(client, wrapper.payload);
                break;

            case Protocol.LEAVE_ROOM:
                HandleLeaveRoom(client);
                break;

            case Protocol.START_GAME:
                HandleStartGame(client);
                break;

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

    private void HandleCreateRoom(TcpClient client, string payload)
    {
        var msg = ParseLobbyRequest(payload);
        if (string.IsNullOrWhiteSpace(msg.playerId))
        {
            SendError(client, "Player name is required");
            return;
        }

        var room = roomManager.CreateRoom(msg.playerId);
        clientPlayers.AddOrUpdate(client, msg.playerId, (_, _) => msg.playerId);
        playerToClient.AddOrUpdate(msg.playerId, client, (_, _) => client);
        clientRooms.AddOrUpdate(client, room.RoomId, (_, _) => room.RoomId);

        BroadcastLobbyState(room);
    }

    private void HandleJoinRoom(TcpClient client, string payload)
    {
        var msg = ParseLobbyRequest(payload);
        if (string.IsNullOrWhiteSpace(msg.playerId) || string.IsNullOrWhiteSpace(msg.roomId))
        {
            SendError(client, "Room code and player name are required");
            return;
        }

        string normalizedRoomId = msg.roomId.ToUpperInvariant();
        if (!roomManager.JoinRoom(normalizedRoomId, msg.playerId))
        {
            SendError(client, "Cannot join room");
            return;
        }

        clientPlayers.AddOrUpdate(client, msg.playerId, (_, _) => msg.playerId);
        playerToClient.AddOrUpdate(msg.playerId, client, (_, _) => client);
        clientRooms.AddOrUpdate(client, normalizedRoomId, (_, _) => normalizedRoomId);

        var room = roomManager.GetRoom(normalizedRoomId);
        BroadcastLobbyState(room);
    }

    private void HandleLeaveRoom(TcpClient client)
    {
        if (!clientPlayers.TryGetValue(client, out var playerId)) return;
        if (!clientRooms.TryRemove(client, out var roomId)) return;

        roomManager.LeaveRoom(roomId, playerId);
        var room = roomManager.GetRoom(roomId);

        if (room == null || room.State == RoomState.Finished)
        {
            sendToClient(client, Protocol.CreateMessage(Protocol.LOBBY_STATE, new LobbyStateMsg()));
            return;
        }

        BroadcastLobbyState(room);
    }

    private void HandleStartGame(TcpClient client)
    {
        if (!clientPlayers.TryGetValue(client, out var playerId)) return;
        if (!clientRooms.TryGetValue(client, out var roomId)) return;

        if (!roomManager.StartGame(roomId, playerId))
        {
            SendError(client, "Cannot start game");
            return;
        }

        var room = roomManager.GetRoom(roomId);
        sendToRoom(roomId, Protocol.CreateMessage(
            Protocol.GAME_STATE,
            room.GameManager.GetState()
        ));
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
            SendError(client, "Invalid payload");
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
            if (!room.GameManager.IsPlayerTurn(playerId))
            {
                SendError(client, "Not your turn");
                return;
            }

            bool success = room.GameManager.PlayCard(playerId, msg.card);

            if (!success)
            {
                SendError(client, "Invalid move");
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
            if (!room.GameManager.IsPlayerTurn(playerId))
            {
                SendError(client, "Not your turn");
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

    private LobbyRequestMsg ParseLobbyRequest(string payload)
    {
        try
        {
            return Protocol.FromJson<LobbyRequestMsg>(payload);
        }
        catch
        {
            return new LobbyRequestMsg();
        }
    }

    private void BroadcastLobbyState(Room room)
    {
        if (room == null) return;

        sendToRoom(room.RoomId, Protocol.CreateMessage(
            Protocol.LOBBY_STATE,
            new LobbyStateMsg
            {
                roomId = room.RoomId,
                hostId = room.HostId,
                players = room.PlayerIds.ToArray(),
                canStart = room.CanStart()
            }
        ));
    }

    private void SendError(TcpClient client, string message)
    {
        sendToClient(client, Protocol.CreateMessage(
            Protocol.ERROR,
            new ErrorMsg { message = message }
        ));
    }
}

[Serializable]
public class LobbyRequestMsg
{
    public string playerId;
    public string roomId;
}

[Serializable]
public class LobbyStateMsg
{
    public string roomId;
    public string hostId;
    public string[] players;
    public bool canStart;
}

[Serializable]
public class ErrorMsg
{
    public string message;
}
