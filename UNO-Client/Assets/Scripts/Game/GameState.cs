using System;

[Serializable]
public class GameState
{
    public string roomId;
    public string currentPlayerId;
    public int direction; // 1 = clockwise, -1 = counter-clockwise
    public int topCardId;
    public string activeColor; // effective color (matters after Wild)
    public int[] hand; // card IDs in your hand
    public PlayerState[] players;
    public bool isGameOver;
    public string winnerId;

    public CardColor ActiveColor
    {
        get
        {
            if (string.IsNullOrEmpty(activeColor)) return CardColor.None;
            if (Enum.TryParse(activeColor, true, out CardColor c)) return c;
            return CardColor.None;
        }
    }
}
