using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private bool isFake = false;

    [SerializeField] private Image cardImage;
    [SerializeField] private ParticleSystem matchingEffect;

    private Sprite cardSprite;
    private Sprite backCardSprite;

    [SerializeField] private CardType cardType = CardType.None;

    public void GetDataInit()
    {

        var backCardSprite = Resources.Load<Sprite>("Sprites/BackCard/BackCard1");
        SetBackCardSprite(backCardSprite);

        var cardSprite = Resources.Load<Sprite>("Sprites/Card/" + GetCardType());
        SetCardSprite(cardSprite);

        SetCardImage(backCardSprite);
    }

    #region Get Set
    public bool GetIsFake() => isFake;
    public void SetIsFake(bool state) => isFake = state;

    public CardType GetCardType() => cardType;

    public Image GetCardImage() => cardImage;

    public ParticleSystem GetEffect() => matchingEffect;

    public Sprite GetBackCardSprite() => backCardSprite;

    public Sprite GetCardSprite() => cardSprite;

    public void SetCardType(CardType cardType) => this.cardType = cardType;

    public void SetCardImage(Sprite sprite) => cardImage.sprite = sprite;
    public void SetBackCardSprite(Sprite sprite) => backCardSprite = sprite;

    public void SetCardSprite(Sprite sprite) => cardSprite = sprite;
    #endregion

}
