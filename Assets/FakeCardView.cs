using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeCardView : MonoBehaviour
{
    private Sprite cardSprite;

    [SerializeField] private Image cardImage;
    [SerializeField] private CardType cardType = CardType.None;


    public void GetDataInit()
    {
        var cardSprite = Resources.Load<Sprite>("Sprites/Card/" + GetCardType());
        SetCardSprite(cardSprite);
        SetCardImage(cardSprite);
    }
    public CardType GetCardType() => cardType;

    public void SetCardType(CardType type) => cardType = type;

    public void SetCardSprite(Sprite sprite) => cardSprite = sprite;

    public void SetCardImage(Sprite sprite) => cardImage.sprite = sprite;

}
