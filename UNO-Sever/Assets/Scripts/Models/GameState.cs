using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public bool IsGameOver = false;
    public string WinnerId;
    public List<PlayerState> Players = new();

    public Stack<Card> DrawPile = new();
    public Stack<Card> DiscardPile = new();

    public int CurrentPlayerIndex = 0;
    public int Direction = 1; // 1 = clockwise, -1 = reverse

    public int PendingDraw = 0; // stacking +2/+4
    public bool PendingSkip = false;
    public CardColor CurrentColor;
}