using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    private const string DisconnectedMarker = "__LOBBY_DISCONNECTED__";

    [Header("Network")]
    [SerializeField] private ClientNetworkManager client;
    [SerializeField] private string host = "127.0.0.1";
    [SerializeField] private int port = 7777;

    [Header("Lobby Inputs")]
    [SerializeField] private InputField playerNameInput;
    [SerializeField] private InputField roomCodeInput;

    [Header("Lobby Text")]
    [SerializeField] private Text statusText;
    [SerializeField] private Text roomCodeText;
    [SerializeField] private Text[] playerSlotTexts = new Text[4];

    [Header("Lobby Buttons")]
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button leaveButton;

    [Header("Images In Scene")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image logoImage;
    [SerializeField] private Image tableImage;
    [SerializeField] private Image cardBackImage;
    [SerializeField] private Image[] avatarImages = new Image[4];

    [Header("Your Sprites")]
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Sprite logoSprite;
    [SerializeField] private Sprite tableSprite;
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private Sprite[] avatarSprites = new Sprite[4];

    private readonly Queue<string> pendingMessages = new Queue<string>();
    private ClientMessageHandler messageHandler;
    private string currentPlayerId;

    private void Awake()
    {
        if (client == null)
            client = FindAnyObjectByType<ClientNetworkManager>();

        if (client == null)
            client = gameObject.AddComponent<ClientNetworkManager>();

        EnsureEventSystem();
        ApplySpriteAssignments();
        BindButtons();

        messageHandler = new ClientMessageHandler();
        messageHandler.OnError += SetStatus;
        ClearLobby();
    }

    private void OnEnable()
    {
        if (client == null) return;

        client.OnConnected += HandleConnected;
        client.OnDisconnected += HandleDisconnected;
        client.OnMessageReceived += QueueMessage;
    }

    private void OnDisable()
    {
        if (client == null) return;

        client.OnConnected -= HandleConnected;
        client.OnDisconnected -= HandleDisconnected;
        client.OnMessageReceived -= QueueMessage;
    }

    private void Update()
    {
        lock (pendingMessages)
        {
            while (pendingMessages.Count > 0)
                HandleMessage(pendingMessages.Dequeue());
        }
    }

    public void Connect()
    {
        SetStatus("Dang ket noi...");
        client.Connect(host, port);
    }

    public void CreateRoom()
    {
        EnsureConnected();
        currentPlayerId = GetPlayerName();

        client.Send(Protocol.CreateMessage(
            Protocol.CREATE_ROOM,
            new LobbyRequestMsg { playerId = currentPlayerId }
        ));
    }

    public void JoinRoom()
    {
        EnsureConnected();
        currentPlayerId = GetPlayerName();

        client.Send(Protocol.CreateMessage(
            Protocol.JOIN_ROOM,
            new LobbyRequestMsg
            {
                playerId = currentPlayerId,
                roomId = GetRoomCode()
            }
        ));
    }

    public void StartGame()
    {
        client.Send(Protocol.CreateMessage(Protocol.START_GAME, new LobbyRequestMsg()));
    }

    public void LeaveRoom()
    {
        client.Send(Protocol.CreateMessage(Protocol.LEAVE_ROOM, new LobbyRequestMsg()));
        ClearLobby();
        SetStatus("Da roi phong.");
    }

    private void EnsureConnected()
    {
        if (string.IsNullOrWhiteSpace(currentPlayerId))
            currentPlayerId = GetPlayerName();

        if (client == null || client.IsConnected)
            return;

        Connect();
    }

    private void QueueMessage(string message)
    {
        lock (pendingMessages)
        {
            pendingMessages.Enqueue(message);
        }
    }

    private void HandleMessage(string json)
    {
        if (json == DisconnectedMarker)
        {
            SetStatus("Mat ket noi server.");
            return;
        }

        Protocol.Wrapper wrapper;

        try
        {
            wrapper = Protocol.ParseWrapper(json);
        }
        catch
        {
            SetStatus("Server gui message khong hop le.");
            return;
        }

        if (wrapper.type == Protocol.LOBBY_STATE)
        {
            var state = Protocol.FromJson<LobbyStateMsg>(wrapper.payload);
            ApplyLobbyState(state);
            return;
        }

        if (wrapper.type == Protocol.GAME_STATE)
        {
            SetStatus("Game da bat dau. Hay chuyen sang Game scene/UI o buoc tiep theo.");
            return;
        }

        messageHandler.HandleMessage(json);
    }

    private void ApplyLobbyState(LobbyStateMsg state)
    {
        if (roomCodeText != null)
            roomCodeText.text = string.IsNullOrEmpty(state.roomId) ? "Room: -" : "Room: " + state.roomId;

        for (int i = 0; i < playerSlotTexts.Length; i++)
        {
            bool hasPlayer = state.players != null && i < state.players.Length;

            if (playerSlotTexts[i] != null)
                playerSlotTexts[i].text = hasPlayer ? state.players[i] : "Waiting...";

            if (avatarImages != null && i < avatarImages.Length && avatarImages[i] != null)
                avatarImages[i].color = hasPlayer ? Color.white : new Color(1f, 1f, 1f, 0.35f);
        }

        if (startButton != null)
            startButton.interactable = state.canStart && state.hostId == currentPlayerId;

        if (leaveButton != null)
            leaveButton.interactable = !string.IsNullOrEmpty(state.roomId);

        SetStatus(state.canStart ? "Phong san sang bat dau." : "Dang cho them nguoi choi...");
    }

    private void ClearLobby()
    {
        if (roomCodeText != null)
            roomCodeText.text = "Room: -";

        if (startButton != null)
            startButton.interactable = false;

        if (leaveButton != null)
            leaveButton.interactable = false;

        for (int i = 0; i < playerSlotTexts.Length; i++)
        {
            if (playerSlotTexts[i] != null)
                playerSlotTexts[i].text = "Waiting...";

            if (avatarImages != null && i < avatarImages.Length && avatarImages[i] != null)
                avatarImages[i].color = new Color(1f, 1f, 1f, 0.35f);
        }
    }

    private string GetPlayerName()
    {
        string value = playerNameInput == null ? "" : playerNameInput.text.Trim();
        return string.IsNullOrEmpty(value) ? "Player" + UnityEngine.Random.Range(1000, 9999) : value;
    }

    private string GetRoomCode()
    {
        return roomCodeInput == null ? "" : roomCodeInput.text.Trim().ToUpperInvariant();
    }

    private void HandleConnected()
    {
        SetStatus("Da ket noi server.");
    }

    private void HandleDisconnected()
    {
        QueueMessage(DisconnectedMarker);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    private void BindButtons()
    {
        if (createButton != null)
        {
            createButton.onClick.RemoveListener(CreateRoom);
            createButton.onClick.AddListener(CreateRoom);
        }

        if (joinButton != null)
        {
            joinButton.onClick.RemoveListener(JoinRoom);
            joinButton.onClick.AddListener(JoinRoom);
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveListener(StartGame);
            startButton.onClick.AddListener(StartGame);
        }

        if (leaveButton != null)
        {
            leaveButton.onClick.RemoveListener(LeaveRoom);
            leaveButton.onClick.AddListener(LeaveRoom);
        }
    }

    private void ApplySpriteAssignments()
    {
        SetImageSprite(backgroundImage, backgroundSprite);
        SetImageSprite(logoImage, logoSprite);
        SetImageSprite(tableImage, tableSprite);
        SetImageSprite(cardBackImage, cardBackSprite);

        if (avatarImages == null || avatarSprites == null) return;

        int count = Mathf.Min(avatarImages.Length, avatarSprites.Length);
        for (int i = 0; i < count; i++)
            SetImageSprite(avatarImages[i], avatarSprites[i]);
    }

    private void SetImageSprite(Image image, Sprite sprite)
    {
        if (image == null || sprite == null) return;

        image.sprite = sprite;
        image.preserveAspect = true;
        image.color = Color.white;
    }

    private void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null) return;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
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
