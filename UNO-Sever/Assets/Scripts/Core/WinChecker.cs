using System;
using System.Collections.Generic;
using System.Linq;

public class WinChecker
{
    private GameState state;

    // track player đã gọi UNO chưa
    private HashSet<string> unoCalledPlayers = new();

    public Action<string> OnPlayerWin;
    public Action<string> OnUnoRequired;
    public Action<string> OnUnoPenalty;

    public WinChecker(GameState gameState)
    {
        state = gameState;
    }

    // ================= WIN CHECK =================

    public bool CheckWin(PlayerState player)
    {
        if (player.Hand.Count == 0)
        {
            OnPlayerWin?.Invoke(player.PlayerId);
            return true;
        }

        return false;
    }

    // ================= UNO SYSTEM =================

    public void CheckUnoState(PlayerState player)
    {
        if (player.Hand.Count == 1)
        {
            // Player cần call UNO
            if (!unoCalledPlayers.Contains(player.PlayerId))
            {
                OnUnoRequired?.Invoke(player.PlayerId);
            }
        }
    }

    public void CallUno(string playerId)
    {
        unoCalledPlayers.Add(playerId);
    }

    public void ResetUno(string playerId)
    {
        if (unoCalledPlayers.Contains(playerId))
            unoCalledPlayers.Remove(playerId);
    }

    public bool HasCalledUno(string playerId)
    {
        return unoCalledPlayers.Contains(playerId);
    }

    // ================= PENALTY =================

    public bool ShouldApplyUnoPenalty(PlayerState player)
    {
        // còn 1 lá mà chưa call UNO
        return player.Hand.Count == 1 &&
               !unoCalledPlayers.Contains(player.PlayerId);
    }

    public void ApplyUnoPenalty(PlayerState player, Func<Card> drawCardFunc)
    {
        // UNO rule: draw 2 cards
        player.Hand.Add(drawCardFunc());
        player.Hand.Add(drawCardFunc());

        OnUnoPenalty?.Invoke(player.PlayerId);

        // reset trạng thái UNO
        ResetUno(player.PlayerId);
    }

    // ================= ROUND RESET =================

    public void ResetAllUno()
    {
        unoCalledPlayers.Clear();
    }
}