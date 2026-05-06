using System;
using System.Collections.Generic;

public enum RoomState
{
    Waiting,
    Playing,
    Finished
}

public class Room
{
    public string RoomId { get; private set; }
    public List<string> PlayerIds { get; private set; } = new();

    public GameManager GameManager { get; private set; }

    public RoomState State { get; private set; } = RoomState.Waiting;

    public int MaxPlayers = 4;
    public int MinPlayers = 2;

    public string HostId { get; private set; }

    public Room(string roomId, string hostId)
    {
        RoomId = roomId;
        HostId = hostId;

        PlayerIds.Add(hostId);
    }

    // ================= PLAYER =================

    public bool AddPlayer(string playerId)
    {
        if (State != RoomState.Waiting)
            return false;

        if (PlayerIds.Count >= MaxPlayers)
            return false;

        if (PlayerIds.Contains(playerId))
            return false;

        PlayerIds.Add(playerId);
        return true;
    }

    public void RemovePlayer(string playerId)
    {
        PlayerIds.Remove(playerId);

        // nếu host rời → chuyển host
        if (playerId == HostId && PlayerIds.Count > 0)
        {
            HostId = PlayerIds[0];
        }

        // nếu hết người → room coi như chết
        if (PlayerIds.Count == 0)
        {
            State = RoomState.Finished;
        }
    }

    // ================= GAME =================

    public bool CanStart()
    {
        return PlayerIds.Count >= MinPlayers;
    }

    public bool StartGame()
    {
        if (State != RoomState.Waiting)
            return false;

        if (!CanStart())
            return false;

        GameManager = new GameManager(PlayerIds);

        State = RoomState.Playing;
        return true;
    }

    public void EndGame()
    {
        State = RoomState.Finished;
    }
}