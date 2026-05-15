using UnityEngine;
using UnityEngine.UI;

public class DisplayCard : MonoBehaviour
{
    [SerializeField] private int displayCardID;
    [SerializeField] private Image cardImage;

    private Card card;

    public Card Card => card;

    void Start()
    {
        SetCard(displayCardID);
    }

    public void SetCard(int cardId)
    {
        card = CardList.GetById(cardId);

        if (card == null)
        {
            Debug.LogWarning($"[DisplayCard] Card ID {cardId} not found");
            return;
        }

        displayCardID = cardId;

        if (cardImage != null && card.sprite != null)
        {
            cardImage.sprite = card.sprite;
            cardImage.preserveAspect = true;
        }
    }
}
