using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerClickHandler
{
    private bool isOpen = false;
    public Card Card { get; private set; }

    private Coroutine closeCardCoroutine;
    private Coroutine matchedCardCoroutine;
    private GameManager gameManager => GameManager.Instance;

    private void Awake()
    {
        Card = GetComponent<Card>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOpen || !gameManager.CanOpenCard) return;
        OpenCard();
    }

    public void OpenCard()
    {
        var duration = GameConfig.openCardDuration / 2;
        Vector3 targetRotation = new Vector3(0, -90, 0);

        Card.GetCardImage().transform.DORotate(targetRotation, duration)
            .OnStart(() =>
            {
                isOpen = true;
            })
            .OnComplete(() =>
            {
                HandleAfterOpenCard();
            });
    }

    private void HandleAfterOpenCard()
    {
        var duration = GameConfig.openCardDuration / 2;

        Card.SetCardImage(Card.GetCardSprite());
        Card.GetCardImage().transform.DORotate(Vector3.zero, duration);

        if (gameManager.IsEndingLevel) return;
        EventManager.CardOpened(this);
    }


    public void CloseCard()
    {
        if (closeCardCoroutine != null) StopCoroutine(closeCardCoroutine);
        closeCardCoroutine = StartCoroutine(CloseCardCoroutine());
    }
    private IEnumerator CloseCardCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return Card.GetCardImage().transform.DOLocalMoveX(-20, 0.2f).WaitForCompletion();
       
        Card.SetCardImage(Card.GetBackCardSprite());

        yield return Card.GetCardImage().transform.DOLocalMoveX(0, 0.2f).WaitForCompletion();
       
        isOpen = false;
        gameManager.CanOpenCard = true;
    }



    public void MatchedCard()
    {
        if (matchedCardCoroutine != null) StopCoroutine(matchedCardCoroutine);
        matchedCardCoroutine = StartCoroutine(MatchedCardCoroutine());
    }
    private IEnumerator MatchedCardCoroutine()
    {
        yield return new WaitForSeconds(0.3f);

        Card.GetEffect().Play();

        yield return new WaitForSeconds(0.5f);

        gameManager.CanOpenCard = true;

        yield return transform.DOScale(0, 0.2f).WaitForCompletion();

        gameObject.SetActive(false);

    }

    public void EnableCard()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.5f);
    }
    public bool IsOpen() => isOpen;
}
