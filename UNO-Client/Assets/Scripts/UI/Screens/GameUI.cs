using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] private ClientNetworkManager client;

    [Header("Card Display")]
    [SerializeField] private Transform handContainer;
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Image topCardImage;

    [Header("Player Views")]
    [SerializeField] private PlayerView[] playerViews = new PlayerView[3];

    [Header("Buttons")]
    [SerializeField] private Button drawButton;
    [SerializeField] private Button unoButton;

    [Header("Status")]
    [SerializeField] private Text statusText;
    [SerializeField] private Text directionText;
    [SerializeField] private Text activeColorText;

    [Header("Color Picker (Wild)")]
    [SerializeField] private GameObject colorPickerPanel;
    [SerializeField] private Button redButton;
    [SerializeField] private Button yellowButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private Button blueButton;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text winnerText;
    [SerializeField] private Button backToLobbyButton;

    private readonly Queue<string> pendingMessages = new Queue<string>();
    private readonly List<CardView> handCards = new List<CardView>();
    private ClientMessageHandler messageHandler;
    private GameState currentState;
    private string myPlayerId;
    private int pendingWildCardId = -1;

    public Action OnBackToLobby;

    private void Awake()
    {
        if (client == null)
            client = FindAnyObjectByType<ClientNetworkManager>();

        messageHandler = new ClientMessageHandler();
        messageHandler.OnGameState += OnGameStateReceived;
        messageHandler.OnError += OnErrorReceived;
        messageHandler.OnPlayerLeft += OnPlayerLeftReceived;

        BindButtons();
        HideColorPicker();
        HideGameOver();
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
                ProcessMessage(pendingMessages.Dequeue());
        }
    }

    public void Initialize(ClientNetworkManager networkClient, string playerId)
    {
        client = networkClient;
        myPlayerId = playerId;

        if (client != null)
            client.OnMessageReceived += QueueMessage;
    }

    // ================= MESSAGE HANDLING =================

    private void QueueMessage(string message)
    {
        lock (pendingMessages)
        {
            pendingMessages.Enqueue(message);
        }
    }

    private void ProcessMessage(string json)
    {
        Protocol.Wrapper wrapper;
        try { wrapper = Protocol.ParseWrapper(json); }
        catch { return; }

        if (wrapper.type == Protocol.GAME_STATE)
        {
            var state = Protocol.FromJson<GameState>(wrapper.payload);
            OnGameStateReceived(state);
            return;
        }

        messageHandler.HandleMessage(json);
    }

    // ================= STATE UPDATE =================

    private void OnGameStateReceived(GameState state)
    {
        currentState = state;

        UpdateTopCard(state.topCardId, state.ActiveColor);
        UpdateHand(state.hand, state.topCardId, state.ActiveColor);
        UpdatePlayerViews(state.players);
        UpdateStatus(state);

        bool isMyTurn = state.currentPlayerId == myPlayerId;
        SetInteractable(isMyTurn);

        if (state.isGameOver)
            ShowGameOver(state.winnerId);
    }

    private void OnErrorReceived(string error)
    {
        SetStatus("Loi: " + error);
    }

    private void OnPlayerLeftReceived(string playerId)
    {
        SetStatus(playerId + " da roi game.");
    }

    // ================= UI UPDATES =================

    private void UpdateTopCard(int topCardId, CardColor activeColor)
    {
        Sprite sprite = CardList.GetSpriteById(topCardId);
        if (topCardImage != null && sprite != null)
        {
            topCardImage.sprite = sprite;
            topCardImage.preserveAspect = true;
        }

        if (activeColorText != null)
            activeColorText.text = activeColor.ToString();

        if (activeColorText != null)
            activeColorText.color = GetColorForCard(activeColor);
    }

    private void UpdateHand(int[] handIds, int topCardId, CardColor activeColor)
    {
        ClearHand();

        if (handIds == null || cardPrefab == null || handContainer == null) return;

        Card topCard = CardList.GetById(topCardId);

        foreach (int id in handIds)
        {
            CardView view = Instantiate(cardPrefab, handContainer);
            Card card = CardList.GetById(id);
            bool canPlay = topCard != null && card != null && card.CanPlayOn(topCard, activeColor);

            view.Setup(id, canPlay);
            view.OnCardClicked += OnHandCardClicked;
            handCards.Add(view);
        }
    }

    private void ClearHand()
    {
        foreach (var card in handCards)
        {
            if (card != null)
            {
                card.OnCardClicked -= OnHandCardClicked;
                Destroy(card.gameObject);
            }
        }
        handCards.Clear();
    }

    private void UpdatePlayerViews(PlayerState[] players)
    {
        if (players == null) return;

        int viewIdx = 0;
        foreach (var p in players)
        {
            if (p.playerId == myPlayerId) continue;
            if (viewIdx >= playerViews.Length) break;

            if (playerViews[viewIdx] != null)
            {
                playerViews[viewIdx].Setup(p);
                playerViews[viewIdx].SetActive(true);
            }
            viewIdx++;
        }

        for (int i = viewIdx; i < playerViews.Length; i++)
        {
            if (playerViews[i] != null)
                playerViews[i].SetActive(false);
        }
    }

    private void UpdateStatus(GameState state)
    {
        bool isMyTurn = state.currentPlayerId == myPlayerId;

        SetStatus(isMyTurn ? "Luot cua ban!" : $"Dang cho {state.currentPlayerId}...");

        if (directionText != null)
            directionText.text = state.direction == 1 ? ">>>" : "<<<";
    }

    private void SetInteractable(bool isMyTurn)
    {
        if (drawButton != null)
            drawButton.interactable = isMyTurn;

        if (unoButton != null)
            unoButton.interactable = isMyTurn && currentState?.hand != null && currentState.hand.Length <= 2;
    }

    // ================= PLAYER ACTIONS =================

    private void OnHandCardClicked(CardView cardView)
    {
        if (currentState == null || currentState.currentPlayerId != myPlayerId) return;

        Card card = cardView.Card;
        if (card == null) return;

        if (card.IsWild)
        {
            pendingWildCardId = cardView.CardId;
            ShowColorPicker();
            return;
        }

        SendPlayCard(cardView.CardId, card.color.ToString());
    }

    private void OnDrawCard()
    {
        if (client == null) return;
        client.Send(Protocol.CreateMessage(Protocol.DRAW_CARD, new DrawCardMsg()));
    }

    private void OnCallUno()
    {
        if (client == null) return;
        client.Send(Protocol.CreateMessage(Protocol.CALL_UNO, new CallUnoMsg()));
    }

    private void OnColorPicked(CardColor color)
    {
        HideColorPicker();
        if (pendingWildCardId < 0) return;

        SendPlayCard(pendingWildCardId, color.ToString());
        pendingWildCardId = -1;
    }

    private void SendPlayCard(int cardId, string chosenColor)
    {
        if (client == null) return;

        client.Send(Protocol.CreateMessage(Protocol.PLAY_CARD, new PlayCardMsg
        {
            cardId = cardId,
            chosenColor = chosenColor
        }));
    }

    // ================= COLOR PICKER =================

    private void ShowColorPicker()
    {
        if (colorPickerPanel != null) colorPickerPanel.SetActive(true);
    }

    private void HideColorPicker()
    {
        if (colorPickerPanel != null) colorPickerPanel.SetActive(false);
    }

    // ================= GAME OVER =================

    private void ShowGameOver(string winnerId)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (winnerText != null)
            winnerText.text = winnerId == myPlayerId ? "Ban da thang!" : $"{winnerId} da thang!";
    }

    private void HideGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    // ================= HELPERS =================

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    private Color GetColorForCard(CardColor c)
    {
        return c switch
        {
            CardColor.Red => Color.red,
            CardColor.Yellow => Color.yellow,
            CardColor.Green => Color.green,
            CardColor.Blue => Color.cyan,
            _ => Color.white
        };
    }

    private void BindButtons()
    {
        if (drawButton != null)
        {
            drawButton.onClick.RemoveListener(OnDrawCard);
            drawButton.onClick.AddListener(OnDrawCard);
        }

        if (unoButton != null)
        {
            unoButton.onClick.RemoveListener(OnCallUno);
            unoButton.onClick.AddListener(OnCallUno);
        }

        if (redButton != null) redButton.onClick.AddListener(() => OnColorPicked(CardColor.Red));
        if (yellowButton != null) yellowButton.onClick.AddListener(() => OnColorPicked(CardColor.Yellow));
        if (greenButton != null) greenButton.onClick.AddListener(() => OnColorPicked(CardColor.Green));
        if (blueButton != null) blueButton.onClick.AddListener(() => OnColorPicked(CardColor.Blue));

        if (backToLobbyButton != null) backToLobbyButton.onClick.AddListener(() => OnBackToLobby?.Invoke());
    }
}

// ================= MESSAGE DTOs =================

[Serializable]
public class PlayCardMsg
{
    public int cardId;
    public string chosenColor;
}

[Serializable]
public class DrawCardMsg { }

[Serializable]
public class CallUnoMsg { }
