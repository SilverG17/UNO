using UnityEngine;

public class Card
{
    public int id;
    public string color; // None => Wild, Red, Blue, Green, Yellow
    public int value; // 0 -> 9 ,10 => Skip, 11 => Reverse, 12 => Draw Two, 13 => Wild, 14 => Wild Draw Four
    public Sprite cardSprite;

    public Card() { }

    public Card (int id, string color, int value, Sprite cardSprite = null)
    {
        this.id = id;
        this.color = color;
        this.value = value;
        this.cardSprite = cardSprite;
    }
}
