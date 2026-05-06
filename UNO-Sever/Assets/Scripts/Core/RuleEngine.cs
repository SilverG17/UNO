using System;
using System.Linq;

public static class RuleEngine
{
    // ================= VALIDATION =================

    public static bool IsValidPlay(GameState state, Card card)
    {
        var top = GetTopCard(state);
        if (state.PendingDraw > 0)
        {
            return card.Type == CardType.DrawTwo ||
                   card.Type == CardType.WildDrawFour;
        }

        return card.Color == state.CurrentColor ||
               card.Type == top.Type ||
               (card.Type == CardType.Number && top.Type == CardType.Number && card.Number == top.Number) ||
               card.Type == CardType.Wild ||
               card.Type == CardType.WildDrawFour;
    }

    // ================= APPLY EFFECT =================

    public static void ApplyCard(GameState state, Card card, Func<CardColor> chooseColorCallback)
    {
        switch (card.Type)
        {
            case CardType.Number:
                state.CurrentColor = card.Color;
                break;

            case CardType.Skip:
                state.CurrentColor = card.Color;
                state.PendingSkip = true;
                break;

            case CardType.Reverse:
                state.CurrentColor = card.Color;
                ApplyReverse(state);
                break;

            case CardType.DrawTwo:
                state.CurrentColor = card.Color;
                state.PendingDraw += 2;
                state.PendingSkip = true;
                break;

            case CardType.Wild:
                state.CurrentColor = chooseColorCallback();
                break;

            case CardType.WildDrawFour:
                state.PendingDraw += 4;
                state.CurrentColor = chooseColorCallback();
                state.PendingSkip = true;
                break;
        }
    }

    // ================= TURN LOGIC =================

    public static void NextTurn(GameState state)
    {
        state.CurrentPlayerIndex =
            (state.CurrentPlayerIndex + state.Direction + state.Players.Count) % state.Players.Count;
    }

    public static void SkipNext(GameState state)
    {
        NextTurn(state);
    }

    public static void ApplyReverse(GameState state)
    {
        // Nếu chỉ có 2 người → reverse = skip
        if (state.Players.Count == 2)
        {
            SkipNext(state);
            return;
        }

        state.Direction *= -1;
    }

    // ================= HELPERS =================

    public static Card GetTopCard(GameState state)
    {
        return state.DiscardPile.Peek();
    }

    public static bool MatchCard(Card a, Card b)
    {
        return a.Color == b.Color &&
               a.Type == b.Type &&
               a.Number == b.Number;
    }

    public static bool HasPlayableCard(GameState state, PlayerState player)
    {
        return player.Hand.Any(card => IsValidPlay(state, card));
    }
}