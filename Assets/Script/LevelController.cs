using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] private int level = 0;
    [SerializeField] private GameObject cardMatchingHolder = null;

    [SerializeField] private List<CardController> fakeCardList = new List<CardController>();
    private List<CardController> cardList = new List<CardController>();

    private GameManager gameManager => GameManager.Instance;
    private GameplayManager gameplayManager => GameplayManager.Instance;

    private void OnEnable()
    {
        SetLevel(gameManager.GetCurrentLevel());
        
        gameManager.IsEndingLevel = false;
        gameManager.SetCurrentLevelController(this);

    }
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        SetCardList();

        DisableCardList();

        ShuffleCardList(cardList);

        SetFakeCardList();

        SetupCardListData();

        gameplayManager.SetFakeCardViewList(fakeCardList);
        gameplayManager.SetupStartLevel();
    }

    public void SetLevel(int index)
    {
        level = index;
    }

    public void ShuffleCardList(List<CardController> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, list.Count);

            //đổi kiểu isFake
            bool tempIsFake = list[i].Card.GetIsFake();
            list[i].Card.SetIsFake(list[randomIndex].Card.GetIsFake());
            list[randomIndex].Card.SetIsFake(tempIsFake);


            //đổi cardType
            CardType cardTypeTemp = list[i].Card.GetCardType();
            list[i].Card.SetCardType(list[randomIndex].Card.GetCardType());
            list[randomIndex].Card.SetCardType(cardTypeTemp);
        }
    }

    private void SetupCardListData()
    {
        foreach (CardController card in cardList)
        {
            card.Card.GetDataInit();

            SpawnTextTest(card.Card.GetCardType(), card.gameObject, card.Card.GetIsFake()); //test
        }
    }

    private void DisableCardList()
    {

        foreach (var card in cardList)
        {
            card.gameObject.SetActive(false);
        }
    }

    private void StartingLevel()
    {
        GameplayManager.Instance.StartGame();
    }

    public bool AllCardOpened()
    {
        bool isEnd = true;
        foreach (var card in cardList)
        {
            if (!card.IsOpen() && !card.Card.GetIsFake())
            {
                isEnd = false;
                break;
            }
        }
        return isEnd;
    }

    public void OpenFakeCardList()
    {
        foreach(var fakeCard in fakeCardList)
        {
            if (fakeCard.IsOpen()) return;
            fakeCard.OpenCard();
        }
    }


    #region Get Set Card List
    private void SetCardList()
    {
        for (int i = 0; i < cardMatchingHolder.transform.childCount; i++)
        {
            var card = cardMatchingHolder.transform.GetChild(i);

            CardController cardController;
            if (card.TryGetComponent<CardController>(out cardController))
            {
                cardList.Add(cardController);
            }
        }

    }

    public List<CardController> GetAllCard()
    {
        List<CardController> cardList = new List<CardController>();
        for (int i = 0; i < cardMatchingHolder.transform.childCount; i++)
        {
            var card = cardMatchingHolder.transform.GetChild(i);

            CardController cardController;
            if (card.TryGetComponent<CardController>(out cardController))
            {
                cardList.Add(cardController);
            }
        }
        return cardList;
    }

    private void SetFakeCardList()
    {
        foreach (var card in cardList)
        {
            if (card.Card.GetIsFake()) fakeCardList.Add(card);
        }
    }

    private void SpawnTextTest(CardType cardType, GameObject obj, bool isFake)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/TextTest");
        var textTest = Instantiate(prefab, transform);
        textTest.transform.position = obj.transform.position;
        textTest.GetComponent<TextMeshProUGUI>().text = cardType.ToString();

        if(isFake) textTest.GetComponent<TextMeshProUGUI>().color = Color.red;
        else textTest.GetComponent<TextMeshProUGUI>().color = Color.green;
    }

    #endregion
}
