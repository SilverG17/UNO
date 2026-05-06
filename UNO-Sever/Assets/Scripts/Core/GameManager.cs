using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager
{
    private GameState state;
    private TurnManager turnManager;
    private WinChecker winChecker;

    private Random rng = new();

    public Action<GameState> OnStateUpdated;

    public GameManager(List<string> playerIds)
    {
        state = new GameState();

        foreach (var id in playerIds)
        {
            state.Players.Add(new PlayerState { PlayerId = id });
        }

        turnManager = new TurnManager(state);
        winChecker = new WinChecker(state);

        InitGame();
    }

    // ================= INIT =================
    private void InitGame()
    {
        var deck = GenerateDeck();
        Shuffle(deck);

        foreach (var card in deck)
            state.DrawPile.Push(card);

        // Deal 7 cards
        foreach (var player in state.Players)
        {
            for (int i = 0; i < 7; i++)
                player.Hand.Add(Draw());
        }

        // First card
        var first = Draw();
        state.DiscardPile.Push(first);
        state.CurrentColor = first.Color;

        Broadcast();
    }

    // ================= PUBLIC ACTIONS =================
    public bool PlayCard(string playerId, Card card)
    {
        if (GuardGameOver()) return false;
        var player = turnManager.GetCurrentPlayer();
        if (!turnManager.IsPlayerTurn(playerId))
            return false;

        if (!RuleEngine.IsValidPlay(state, card))
            return false;

        // Find card in hand
        var handCard = player.Hand.FirstOrDefault(c => RuleEngine.MatchCard(c, card));
        if (handCard == null)
            return false;

        // remove
        player.Hand.Remove(handCard);

        // push to discard
        state.DiscardPile.Push(handCard);

        // apply effect
        RuleEngine.ApplyCard(state, handCard, ChooseColor);

        // ===== UNO check =====
        winChecker.CheckUnoState(player);

        // ===== WIN check =====
        if (winChecker.CheckWin(player))
        {
            state.IsGameOver = true;
            state.WinnerId = player.PlayerId;

            Console.WriteLine($"Player {player.PlayerId} wins!");
            Broadcast();
            return true;
        }

        // ===== TURN =====
        turnManager.EndTurn();

        Broadcast();
        return true;
    }

    public bool IsPlayerTurn(string playerId)
    {
        return turnManager.IsPlayerTurn(playerId);
    }

    public void DrawCard(string playerId)
    {
        if (GuardGameOver()) return;
        var player = turnManager.GetCurrentPlayer();

        if (!turnManager.IsPlayerTurn(playerId))
            return;

        int drawCount = turnManager.GetDrawCount();
        for (int i = 0; i < drawCount; i++)
            player.Hand.Add(Draw());

        // UNO penalty 
        if (winChecker.ShouldApplyUnoPenalty(player))
        {
            winChecker.ApplyUnoPenalty(player, Draw);
        }

        if (turnManager.ShouldSkipAfterDraw())
        {
            state.PendingSkip = true;
        }

        // reset UNO
        winChecker.ResetUno(player.PlayerId);
        turnManager.ClearDrawPenalty();
        turnManager.EndTurn();

        Broadcast();
    }

    public void CallUno(string playerId)
    {
        if (GuardGameOver()) return;

        winChecker.CallUno(playerId);
    }

    // ================= CORE =================

    private Card Draw()
    {
        if (state.DrawPile.Count == 0)
            Reshuffle();

        return state.DrawPile.Pop();
    }

    private void Reshuffle()
    {
        var top = state.DiscardPile.Pop();
        var cards = state.DiscardPile.ToList();
        state.DiscardPile.Clear();

        Shuffle(cards);

        foreach (var c in cards)
            state.DrawPile.Push(c);

        state.DiscardPile.Push(top);
    }

    // ================= HELPERS =================

    private List<Card> GenerateDeck()
    {
        var deck = new List<Card>();

        foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
        {
            if (color == CardColor.Wild) continue;

            for (int i = 0; i <= 9; i++)
            {
                deck.Add(new Card { Color = color, Type = CardType.Number, Number = i });
            }

            deck.Add(new Card { Color = color, Type = CardType.Skip });
            deck.Add(new Card { Color = color, Type = CardType.Reverse });
            deck.Add(new Card { Color = color, Type = CardType.DrawTwo });
        }

        for (int i = 0; i < 4; i++)
        {
            deck.Add(new Card { Type = CardType.Wild, Color = CardColor.Wild });
            deck.Add(new Card { Type = CardType.WildDrawFour, Color = CardColor.Wild });
        }

        return deck;
    }

    private void Shuffle(List<Card> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private CardColor ChooseColor()
    {
        var colors = new[] { CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow };
        return colors[rng.Next(colors.Length)];
    }

    private void Broadcast()
    {
        OnStateUpdated?.Invoke(state);
    }

    private bool GuardGameOver()
    {
        if (state.IsGameOver)
        {
            Console.WriteLine("Game already ended.");
            return true;
        }
        return false;
    }

    public GameState GetState()
    {
        return state;
    }
}