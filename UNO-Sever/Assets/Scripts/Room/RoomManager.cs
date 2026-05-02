using System;
using System.Collections.Generic;

public class RoomManager
{
    private Dictionary<string, Room> rooms = new();

    private Random rng = new();

    // ================= CREATE =================

    public Room CreateRoom(string hostId)
    {
        string roomId = GenerateRoomId();

        var room = new Room(roomId, hostId);

        rooms.Add(roomId, room);

        return room;
    }

    // ================= JOIN =================

    public bool JoinRoom(string roomId, string playerId)
    {
        if (!rooms.ContainsKey(roomId))
            return false;

        return rooms[roomId].AddPlayer(playerId);
    }

    // ================= LEAVE =================

    public void LeaveRoom(string roomId, string playerId)
    {
        if (!rooms.ContainsKey(roomId))
            return;

        var room = rooms[roomId];

        room.RemovePlayer(playerId);

        // nếu room chết → remove luôn
        if (room.State == RoomState.Finished)
        {
            rooms.Remove(roomId);
        }
    }

    // ================= START =================

    public bool StartGame(string roomId, string playerId)
    {
        if (!rooms.ContainsKey(roomId))
            return false;

        var room = rooms[roomId];

        // chỉ host được start
        if (room.HostId != playerId)
            return false;

        return room.StartGame();
    }

    // ================= GET =================

    public Room GetRoom(string roomId)
    {
        rooms.TryGetValue(roomId, out var room);
        return room;
    }

    public List<Room> GetAllRooms()
    {
        return new List<Room>(rooms.Values);
    }

    // ================= HELPERS =================

    private string GenerateRoomId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        string id;
        do
        {
            id = "";
            for (int i = 0; i < 6; i++)
                id += chars[rng.Next(chars.Length)];

        } while (rooms.ContainsKey(id));

        return id;
    }
}