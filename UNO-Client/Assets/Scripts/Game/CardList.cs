using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CardList : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

    void Awake()
    {
        Sprite[] cardSprite = Resources.LoadAll<Sprite>("deck");

        //Red Cards
        cardList.Add(new Card(0, "Red", 0, cardSprite[0]));
        cardList.Add(new Card(1, "Red", 1, cardSprite[1]));
        cardList.Add(new Card(2, "Red", 2, cardSprite[2]));
        cardList.Add(new Card(3, "Red", 3, cardSprite[3]));
        cardList.Add(new Card(4, "Red", 4, cardSprite[4]));
        cardList.Add(new Card(5, "Red", 5, cardSprite[5]));
        cardList.Add(new Card(6, "Red", 6, cardSprite[6]));
        cardList.Add(new Card(7, "Red", 7, cardSprite[7]));
        cardList.Add(new Card(8, "Red", 8, cardSprite[8]));
        cardList.Add(new Card(9, "Red", 9, cardSprite[9]));
        cardList.Add(new Card(10, "Red", 10, cardSprite[10])); // Skip
        cardList.Add(new Card(11, "Red", 11, cardSprite[11])); // Reverse
        cardList.Add(new Card(12, "Red", 12, cardSprite[12])); // Draw Two

        // Yellow Cards
        cardList.Add(new Card(13, "Yellow", 0, cardSprite[14]));
        cardList.Add(new Card(14, "Yellow", 1, cardSprite[15]));
        cardList.Add(new Card(15, "Yellow", 2, cardSprite[16]));
        cardList.Add(new Card(16, "Yellow", 3, cardSprite[17]));
        cardList.Add(new Card(17, "Yellow", 4, cardSprite[18]));
        cardList.Add(new Card(18, "Yellow", 5, cardSprite[19]));
        cardList.Add(new Card(19, "Yellow", 6, cardSprite[20]));
        cardList.Add(new Card(20, "Yellow", 7, cardSprite[21]));
        cardList.Add(new Card(21, "Yellow", 8, cardSprite[22]));
        cardList.Add(new Card(22, "Yellow", 9, cardSprite[23]));
        cardList.Add(new Card(23, "Yellow", 10, cardSprite[24])); // Skip
        cardList.Add(new Card(24, "Yellow", 11, cardSprite[25])); // Reverse
        cardList.Add(new Card(25, "Yellow", 12, cardSprite[26])); // Draw Two

        // Green Cards
        cardList.Add(new Card(26, "Green", 0, cardSprite[27]));
        cardList.Add(new Card(27, "Green", 1, cardSprite[28]));
        cardList.Add(new Card(28, "Green", 2, cardSprite[29]));
        cardList.Add(new Card(29, "Green", 3, cardSprite[30]));
        cardList.Add(new Card(30, "Green", 4, cardSprite[31]));
        cardList.Add(new Card(31, "Green", 5, cardSprite[32]));
        cardList.Add(new Card(32, "Green", 6, cardSprite[33]));
        cardList.Add(new Card(33, "Green", 7, cardSprite[34]));
        cardList.Add(new Card(34, "Green", 8, cardSprite[35]));
        cardList.Add(new Card(35, "Green", 9, cardSprite[36]));
        cardList.Add(new Card(36, "Green", 10, cardSprite[37])); // Skip
        cardList.Add(new Card(37, "Green", 11, cardSprite[38])); // Reverse
        cardList.Add(new Card(38, "Green", 12, cardSprite[39])); // Draw Two

        // Blue Cards
        cardList.Add(new Card(39, "Blue", 0, cardSprite[40]));
        cardList.Add(new Card(40, "Blue", 1, cardSprite[41]));
        cardList.Add(new Card(41, "Blue", 2, cardSprite[42]));
        cardList.Add(new Card(42, "Blue", 3, cardSprite[43]));
        cardList.Add(new Card(43, "Blue", 4, cardSprite[44]));
        cardList.Add(new Card(44, "Blue", 5, cardSprite[45]));
        cardList.Add(new Card(45, "Blue", 6, cardSprite[46]));
        cardList.Add(new Card(46, "Blue", 7, cardSprite[47]));
        cardList.Add(new Card(47, "Blue", 8, cardSprite[48]));
        cardList.Add(new Card(48, "Blue", 9, cardSprite[49]));
        cardList.Add(new Card(49, "Blue", 10, cardSprite[50])); // Skip
        cardList.Add(new Card(50, "Blue", 11, cardSprite[51])); // Reverse
        cardList.Add(new Card(51, "Blue", 12, cardSprite[52])); // Draw Two

        // Wild Cards
        cardList.Add(new Card(52, "None", 13, cardSprite[53])); // Wild
        cardList.Add(new Card(53, "None", 14, cardSprite[13])); // Wild Draw Four
    }
}
