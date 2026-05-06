using System;

[Serializable]
public class PlayCardMsg : BaseMessage
{
    public string playerId;
    public Card card;
}