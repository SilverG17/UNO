using System;

[Serializable]
public class PlayerState
{
    public string playerId;
    public int cardCount;
    public bool isCurrentTurn;
    public bool calledUno;
}
