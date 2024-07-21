using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool IsEndingLevel = false;
    public bool CanOpenCard = true;
    public bool IsMatching = false;

    [SerializeField] private int currentLevel = 0;
    [SerializeField] private LevelController currentLevelController = null;

    [SerializeField] private CardController cardMatching1 = null;
    [SerializeField] private CardController cardMatching2 = null;


    private Coroutine handleCompleteLevelCoroutine;
    private GameplayManager gameplayManager => GameplayManager.Instance;

    private void Start()
    {
        LoadLevelAsync($"Prefabs/Level/Level{currentLevel}");
    }

    private void OnEnable()
    {
        EventManager.onCardOpened += SetCardMatchingOpened;
    }
    private void OnDisable()
    {
        EventManager.onCardOpened -= SetCardMatchingOpened;
    }

    #region Load Asset
    public void LoadLevelAsync(string path)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>(path);
        request.completed += OnLoadLevelDone;
    }

    public void OnLoadLevelDone(AsyncOperation operation)
    {
        ResourceRequest request = (ResourceRequest)operation;
        if (request.asset != null)
        {
            GameObject loadedObject = request.asset as GameObject;
            
            var holder = gameplayManager.GetLevelHolder();
            var level = Instantiate(loadedObject, holder);

            Resources.UnloadUnusedAssets();

        }
        else
        {
            Debug.LogError("Failed to load asset at path: " + request.asset);
        }
    }
    #endregion

    public void SetCardMatchingOpened(CardController cardMatching)
    {
        if (cardMatching1 == null) cardMatching1 = cardMatching;
        else if (cardMatching2 == null) cardMatching2 = cardMatching;
        else Debug.Log("error");

        CheckMatchingCard();
    }

    private void CheckMatchingCard()
    {
        if (cardMatching1 == null || cardMatching2 == null) return;

        CanOpenCard = false;


        var cardType1 = cardMatching1.Card.GetCardType();
        var cardType2 = cardMatching2.Card.GetCardType();

        if (cardType1 == cardType2)
        {
            if (HaveFakeCard()) HandleNotMatching();
            else HandleMatching();
        }
        else
        {
            HandleNotMatching();
        }

        cardMatching1 = null;
        cardMatching2 = null;

    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }

    public void SetCurrentLevelController(LevelController levelController)
    {
        currentLevelController = levelController;
    }

    public LevelController GetCurrentLevelController()
    {
        return currentLevelController;
    }


    private bool HaveFakeCard()
    {
        if (cardMatching1.Card.GetIsFake() || cardMatching2.Card.GetIsFake()) 
            return true;
        else 
            return false;
    }

    private void HandleMatching()
    {
        Debug.Log("Matching");

        cardMatching1.MatchedCard();
        cardMatching2.MatchedCard();

        CheckEndLevel();
    }

    private void CheckEndLevel()
    {
        if (currentLevelController.AllCardOpened())
        {
            if (handleCompleteLevelCoroutine != null) StopCoroutine(handleCompleteLevelCoroutine);
            handleCompleteLevelCoroutine = StartCoroutine(HandleCompleteLevelCoroutine());
        }
    }

    private void HandleNotMatching()
    {
        Debug.Log("Not Matching");

        cardMatching1.CloseCard();
        cardMatching2.CloseCard();
    }

    private IEnumerator HandleCompleteLevelCoroutine()
    {
        IsEndingLevel = true;
        CanOpenCard = false;
        cardMatching1 = null;
        cardMatching2 = null;

        yield return new WaitForSeconds(1);
        currentLevelController.OpenFakeCardList();
        yield return new WaitForSeconds(1);
        gameplayManager.CompleteLevel();

    }
}
