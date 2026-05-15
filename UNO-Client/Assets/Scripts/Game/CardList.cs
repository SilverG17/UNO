using UnityEngine;
using System.Collections.Generic;

public class CardList : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();
    public static bool IsLoaded => cardList.Count > 0;

    private static readonly CardColor[] colors = { CardColor.Red, CardColor.Yellow, CardColor.Green, CardColor.Blue };
    private static readonly int cardsPerColor = 13; // 0-9 + Skip + Reverse + DrawTwo

    void Awake()
    {
        if (IsLoaded) return;
        LoadDeck();
    }

    private void LoadDeck()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("deck");

        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("[CardList] No sprites found in Resources/deck");
            return;
        }

        cardList.Clear();
        int id = 0;
        int spriteIndex = 0;

        foreach (var color in colors)
        {
            for (int v = 0; v < cardsPerColor; v++)
            {
                if (spriteIndex >= sprites.Length)
                {
                    Debug.LogWarning($"[CardList] Sprite index {spriteIndex} out of range, deck has {sprites.Length} sprites");
                    break;
                }

                cardList.Add(new Card(id, color, (CardValue)v, sprites[spriteIndex]));
                id++;
                spriteIndex++;
            }
        }

        // Wild (sprite index 53)
        if (spriteIndex < sprites.Length)
            cardList.Add(new Card(id++, CardColor.None, CardValue.Wild, sprites[53]));

        // Wild Draw Four (sprite index 13 in original layout)
        if (sprites.Length > 13)
            cardList.Add(new Card(id++, CardColor.None, CardValue.WildDrawFour, sprites[13]));

        Debug.Log($"[CardList] Loaded {cardList.Count} cards");
    }

    public static Card GetById(int cardId)
    {
        if (cardId < 0 || cardId >= cardList.Count) return null;
        return cardList[cardId];
    }

    public static Sprite GetSpriteById(int cardId)
    {
        var card = GetById(cardId);
        return card?.sprite;
    }
}
