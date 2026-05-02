using System;
using System.Collections.Generic;
using UnityEngine; // dùng JsonUtility

public class MessageManager
{
    private GameManager gameManager;

    // mapping type → handler
    private Dictionary<string, Action<string>> handlers;

    public MessageManager(GameManager gameManager)
    {
        this.gameManager = gameManager;

        handlers = new Dictionary<string, Action<string>>
        {
            { "PlayCard", HandlePlayCard },
            { "DrawCard", HandleDrawCard },
            { "CallUno", HandleCallUno }
        };
    }

    // ================= ENTRY POINT =================

    public void HandleMessage(string json)
    {
        BaseMessage baseMsg = JsonUtility.FromJson<BaseMessage>(json);

        if (baseMsg == null || string.IsNullOrEmpty(baseMsg.type))
        {
            Debug.LogWarning("Invalid message");
            return;
        }

        if (handlers.TryGetValue(baseMsg.type, out var handler))
        {
            handler(json);
        }
        else
        {
            Debug.LogWarning($"Unknown message type: {baseMsg.type}");
        }
    }

    // ================= HANDLERS =================

    private void HandlePlayCard(string json)
    {
        var msg = JsonUtility.FromJson<PlayCardMsg>(json);

        bool success = gameManager.PlayCard(msg.playerId, msg.card);

        if (!success)
        {
            Debug.LogWarning("PlayCard failed");
            // TODO: gửi error về client
        }
    }

    private void HandleDrawCard(string json)
    {
        var msg = JsonUtility.FromJson<DrawCardMsg>(json);

        gameManager.DrawCard(msg.playerId);
    }

    private void HandleCallUno(string json)
    {
        var msg = JsonUtility.FromJson<CallUnoMsg>(json);

        gameManager.CallUno(msg.playerId);
    }
}