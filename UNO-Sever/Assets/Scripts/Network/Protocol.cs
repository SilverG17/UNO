using System;
using UnityEngine;

public static class Protocol
{
    // ================= MESSAGE TYPES =================

    public const string PLAY_CARD = "PlayCard";
    public const string DRAW_CARD = "DrawCard";
    public const string CALL_UNO = "CallUno";
    public const string CREATE_ROOM = "CreateRoom";
    public const string JOIN_ROOM = "JoinRoom";
    public const string LEAVE_ROOM = "LeaveRoom";
    public const string START_GAME = "StartGame";
    public const string LOBBY_STATE = "LobbyState";
    public const string GAME_STATE = "GameState";
    public const string ERROR = "Error";
    public const string PLAYER_LEFT = "PlayerLeft";

    // ================= SERIALIZATION =================

    public static string ToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    public static T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    // ================= MESSAGE FACTORY =================

    public static string CreateMessage(string type, object payload)
    {
        Wrapper wrapper = new Wrapper
        {
            type = type,
            payload = JsonUtility.ToJson(payload)
        };

        return JsonUtility.ToJson(wrapper);
    }

    public static Wrapper ParseWrapper(string json)
    {
        return JsonUtility.FromJson<Wrapper>(json);
    }
}

[Serializable]
public class Wrapper
{
    public string type;
    public string payload; 
}
