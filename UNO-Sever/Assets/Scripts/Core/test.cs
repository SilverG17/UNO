using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestGameCore : MonoBehaviour
{
    void Start()
    {
        var players = new List<string> { "P1", "P2" };

        var game = new GameManager(players);

        game.OnStateUpdated += (state) =>
        {
            Debug.Log("===== STATE UPDATE =====");
            Debug.Log($"Current Player: {state.Players[state.CurrentPlayerIndex].PlayerId}");
            Debug.Log($"Top Card: {state.DiscardPile.Peek().Type} - {state.DiscardPile.Peek().Color}");

            foreach (var p in state.Players)
            {
                Debug.Log($"{p.PlayerId} has {p.Hand.Count} cards");
            }
        };

        // test flow
        SimulateGame(game);
    }

    void SimulateGame(GameManager game)
    {
        var state = GetState(game);

        for (int i = 0; i < 20; i++)
        {
            if (state.IsGameOver)
            {
                Debug.Log($"GAME OVER - Winner: {state.WinnerId}");
                break;
            }
            var player = state.Players[state.CurrentPlayerIndex];

            Debug.Log($"\n--- TURN {i} ({player.PlayerId}) ---");

            bool played = false;

            var playableCard = player.Hand.FirstOrDefault(card => RuleEngine.IsValidPlay(state, card));
            if (playableCard != null)
            {
                game.PlayCard(player.PlayerId, playableCard);
                played = true;
            }

            if (!played)
            {
                game.DrawCard(player.PlayerId);
            }
        }
    }

    GameState GetState(GameManager gm)
    {
        var field = typeof(GameManager).GetField("state", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);

        return (GameState)field.GetValue(gm);
    }
}