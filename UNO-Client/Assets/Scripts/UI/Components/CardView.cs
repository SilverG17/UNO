using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardView : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Button button;
    [SerializeField] private GameObject highlightBorder;

    private Card card;
    private int cardId;
    private bool interactable = true;

    public Card Card => card;
    public int CardId => cardId;
    public Action<CardView> OnCardClicked;

    private void Awake()
    {
        if (cardImage == null) cardImage = GetComponent<Image>();
        if (button == null) button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(HandleClick);
    }

    public void Setup(int cardId, bool canPlay = true)
    {
        this.cardId = cardId;
        card = CardList.GetById(cardId);

        if (card == null)
        {
            Debug.LogWarning($"[CardView] Card {cardId} not found");
            return;
        }

        if (cardImage != null && card.sprite != null)
        {
            cardImage.sprite = card.sprite;
            cardImage.preserveAspect = true;
        }

        SetPlayable(canPlay);
    }

    public void SetPlayable(bool canPlay)
    {
        interactable = canPlay;

        if (button != null)
            button.interactable = canPlay;

        if (cardImage != null)
            cardImage.color = canPlay ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
    }

    public void SetHighlight(bool on)
    {
        if (highlightBorder != null)
            highlightBorder.SetActive(on);
    }

    private void HandleClick()
    {
        if (!interactable) return;
        OnCardClicked?.Invoke(this);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleClick);
    }
}
