using System;
using System.Net.Sockets;
using UnityEngine;

public class ClientMessageHandler
{
    public Action<GameState> OnGameState;
    public Action<string> OnError;
    public Action<string> OnPlayerLeft;

    public void HandleMessage(string json)
    {
        Protocol.Wrapper wrapper;

        try
        {
            wrapper = Protocol.ParseWrapper(json);
        }
        catch
        {
            Debug.LogWarning("Invalid JSON: " + json);
            return;
        }

        switch (wrapper.type)
        {
            case Protocol.GAME_STATE:
                HandleGameState(wrapper.payload);
                break;

            case Protocol.ERROR:
                HandleError(wrapper.payload);
                break;

            case Protocol.PLAYER_LEFT:
                HandlePlayerLeft(wrapper.payload);
                break;

            default:
                Debug.LogWarning("Unknown message type: " + wrapper.type);
                break;
        }
    }

    // ================= HANDLERS =================

    private void HandleGameState(string payload)
    {
        try
        {
            var state = Protocol.FromJson<GameState>(payload);
            OnGameState?.Invoke(state);
        }
        catch (Exception e)
        {
            Debug.LogError("Parse GameState failed: " + e.Message);
        }
    }

    private void HandleError(string payload)
    {
        try
        {
            var obj = JsonUtility.FromJson<ErrorMsg>(payload);
            OnError?.Invoke(obj.message);
        }
        catch
        {
            OnError?.Invoke("Unknown error");
        }
    }

    private void HandlePlayerLeft(string payload)
    {
        try
        {
            var obj = JsonUtility.FromJson<PlayerLeftMsg>(payload);
            OnPlayerLeft?.Invoke(obj.playerId);
        }
        catch
        {
            Debug.LogWarning("Parse PlayerLeft failed");
        }
    }
}

// ================= DTO =================

[Serializable]
public class ErrorMsg
{
    public string message;
}

[Serializable]
public class PlayerLeftMsg
{
    public string playerId;
}