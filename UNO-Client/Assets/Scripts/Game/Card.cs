using System;
using UnityEngine;

public enum CardColor { None, Red, Yellow, Green, Blue }

public enum CardValue
{
    Zero = 0, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
    Skip = 10,
    Reverse = 11,
    DrawTwo = 12,
    Wild = 13,
    WildDrawFour = 14
}

[Serializable]
public class Card
{
    public int id;
    public CardColor color;
    public CardValue value;

    [NonSerialized] public Sprite sprite;

    public Card() { }

    public Card(int id, CardColor color, CardValue value, Sprite sprite = null)
    {
        this.id = id;
        this.color = color;
        this.value = value;
        this.sprite = sprite;
    }

    public bool IsWild => color == CardColor.None;
    public bool IsAction => value >= CardValue.Skip;
    public bool IsNumber => value <= CardValue.Nine;

    public bool CanPlayOn(Card topCard, CardColor activeColor)
    {
        if (IsWild) return true;
        if (color == activeColor) return true;
        if (value == topCard.value) return true;
        return false;
    }

    public string DisplayName
    {
        get
        {
            if (IsWild) return value.ToString();
            return $"{color} {value}";
        }
    }
}
