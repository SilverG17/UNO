using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private ClientNetworkManager client;
    [SerializeField] private GameUI gameUI;

    private readonly Queue<string> pendingMessages = new Queue<string>();
    private ClientMessageHandler messageHandler;
    private GameState lastState;

    private void Awake()
    {
        if (client == null)
            client = FindAnyObjectByType<ClientNetworkManager>();

        messageHandler = new ClientMessageHandler();
        messageHandler.OnGameState += HandleGameState;
        messageHandler.OnError += msg => Debug.LogWarning("[GameController] Error: " + msg);
        messageHandler.OnPlayerLeft += id => Debug.Log("[GameController] Player left: " + id);

        if (gameUI != null)
            gameUI.OnBackToLobby += ReturnToLobby;
    }

    private void OnEnable()
    {
        if (client != null)
            client.OnMessageReceived += QueueMessage;
    }

    private void OnDisable()
    {
        if (client != null)
            client.OnMessageReceived -= QueueMessage;
    }

    private void Update()
    {
        lock (pendingMessages)
        {
            while (pendingMessages.Count > 0)
                messageHandler.HandleMessage(pendingMessages.Dequeue());
        }
    }

    private void QueueMessage(string msg)
    {
        lock (pendingMessages)
        {
            pendingMessages.Enqueue(msg);
        }
    }

    private void HandleGameState(GameState state)
    {
        lastState = state;
        Debug.Log($"[GameController] Turn: {state.currentPlayerId}, TopCard: {state.topCardId}, Hand: {state.hand?.Length ?? 0}");
    }

    public void ReturnToLobby()
    {
        if (client != null)
            client.Send(Protocol.CreateMessage(Protocol.LEAVE_ROOM, new LobbyRequestMsg()));

        SceneManager.LoadScene("Lobby");
    }
}
