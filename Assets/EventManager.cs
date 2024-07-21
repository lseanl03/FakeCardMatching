using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public delegate void OnCardOpened(CardController cardMatching);
    public static event OnCardOpened onCardOpened;

    public static void CardOpened(CardController cardMatching)
    {
        onCardOpened?.Invoke(cardMatching);
    }

    public delegate void OnCardMatched(CardController card1, CardController card2);
    public static event OnCardMatched onCardMatched;

    public static void CardMatched(CardController card1, CardController card2)
    {
        onCardMatched?.Invoke(card1, card2);
    }

    public delegate void OnCardNotMatching(CardController card1, CardController card2);
    public static event OnCardNotMatching onCardNotMatching;

    public static void CardNotMatching(CardController card1, CardController card2)
    {
        onCardNotMatching?.Invoke(card1, card2);
    }
}
