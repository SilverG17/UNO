using System;

public class TurnManager
{
    private GameState state;

    public TurnManager(GameState gameState)
    {
        state = gameState;
    }

    // ================= TURN FLOW =================

    public string GetCurrentPlayerId()
    {
        return state.Players[state.CurrentPlayerIndex].PlayerId;
    }

    public PlayerState GetCurrentPlayer()
    {
        return state.Players[state.CurrentPlayerIndex];
    }

    public bool IsPlayerTurn(string playerId)
    {
        return GetCurrentPlayerId() == playerId;
    }

    // ================= TURN PROGRESSION =================

    public void EndTurn()
    {
        int step = 1;

        if (state.PendingSkip)
        {
            step = 2;
            state.PendingSkip = false;
        }

        state.CurrentPlayerIndex =
            (state.CurrentPlayerIndex + step * state.Direction + state.Players.Count)
            % state.Players.Count;
    }

    public void SkipTurn()
    {
        RuleEngine.SkipNext(state);
    }

    public void Reverse()
    {
        RuleEngine.ApplyReverse(state);
    }

    // ================= DRAW FLOW =================

    public int GetDrawCount()
    {
        return state.PendingDraw > 0 ? state.PendingDraw : 1;
    }

    public void ClearDrawPenalty()
    {
        state.PendingDraw = 0;
    }

    public bool HasPendingDraw()
    {
        return state.PendingDraw > 0;
    }

    // ================= AUTO LOGIC =================

    public bool ShouldSkipAfterDraw()
    {
        return HasPendingDraw();
    }

    // ================= DEBUG =================

    public void PrintTurnInfo()
    {
        Console.WriteLine($"Current Player: {GetCurrentPlayerId()}");
        Console.WriteLine($"Direction: {(state.Direction == 1 ? "Clockwise" : "Reverse")}");
        Console.WriteLine($"Pending Draw: {state.PendingDraw}");
    }
}