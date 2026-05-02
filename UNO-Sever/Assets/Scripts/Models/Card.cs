using System;

[Serializable]
public class Card
{
    public string Id;
    public CardColor Color;
    public CardType Type;
    public int Number; // chỉ dùng nếu là Number
}