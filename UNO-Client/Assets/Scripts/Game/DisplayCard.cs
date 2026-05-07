using UnityEngine;
using UnityEngine.UI;

public class DisplayCard : MonoBehaviour
{
    public int displayCardID;

    public int id;
    public string color;
    public int value;
    public Sprite cardSprite;

    public Image cardImage;

    void Start()
    {
        Card card = CardList.cardList[displayCardID];

        id = card.id;
        color = card.color;
        value = card.value;
        cardSprite = card.cardSprite;

        cardImage.sprite = cardSprite;
    }
}