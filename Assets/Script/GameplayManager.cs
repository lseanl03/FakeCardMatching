using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{

    [SerializeField] private TextMeshProUGUI startText = null;
    [SerializeField] private TextMeshProUGUI completeText = null;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI fakeCardText;


    [SerializeField] private Transform levelHolder = null;
    [SerializeField] private Transform infoPanel = null;
    [SerializeField] private Transform fakeCardViewHolder = null;

    private Coroutine startGameCoroutine;
    private Coroutine completeLevelCoroutine;

    private Coroutine enableAllCardCoroutine;
    private Coroutine setupStartLevelCoroutine;
    private Coroutine startLevelCoroutine;
    private GameManager gameManager => GameManager.Instance;

    public void Start()
    {
        levelText.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
    }

    public Transform GetLevelHolder()
    {
        return levelHolder;
    }

    private void SetStartText(string text)
    {
        startText.text = text;
    }
    public void StartGame()
    {
        if (startGameCoroutine != null) StopCoroutine(startGameCoroutine);
        startGameCoroutine = StartCoroutine(StartGameCoroutine());
    }
    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        SetStartText("Ready");
        startText.gameObject.SetActive(true);
        startText.transform.localScale = Vector3.zero;

        yield return startText.transform.DOScale(1, 0.5f).WaitForCompletion();
        yield return new WaitForSeconds(0.5f);
        yield return startText.transform.DOScale(0.5f, 0.2f).WaitForCompletion();

        SetStartText("Start");

        yield return startText.transform.DOScale(1, 0.1f).WaitForCompletion(); 
        yield return new WaitForSeconds(0.3f);
        yield return startText.transform.DOScale(0, 0.1f).WaitForCompletion(); 

        startText.gameObject.SetActive(false);
        gameManager.CanOpenCard = true;

    }

    public void CompleteLevel()
    {
        if (completeLevelCoroutine != null) StopCoroutine(completeLevelCoroutine);
        completeLevelCoroutine = StartCoroutine(CompleteLevelCoroutine());
    }
    private IEnumerator CompleteLevelCoroutine()
    {
        completeText.gameObject.SetActive(true);
        completeText.transform.localScale = Vector3.zero;

        yield return completeText.transform.DOScale(1, 0.5f).WaitForCompletion();
        yield return new WaitForSeconds(0.5f);
        yield return completeText.transform.DOScale(0, 0.2f).WaitForCompletion();

        completeText.gameObject.SetActive(false);
        fakeCardViewHolder.gameObject.SetActive(false);
        gameManager.GetCurrentLevelController().gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);


        DisableFakeCardViewList();
        gameManager.SetCurrentLevel(gameManager.GetCurrentLevel() + 1);
        gameManager.LoadLevelAsync($"Prefabs/Level/Level{gameManager.GetCurrentLevel()}");

    }







    public void SetupStartLevel()
    {
        if (setupStartLevelCoroutine != null) StopCoroutine(setupStartLevelCoroutine);
        setupStartLevelCoroutine = StartCoroutine(SetupStartLevelCoroutine());
    }

    private IEnumerator SetupStartLevelCoroutine()
    {
        gameManager.CanOpenCard = false;

        yield return new WaitForSeconds(0.5f);

        EnableCardList();
    }

    private void EnableCardList()
    {
        if (enableAllCardCoroutine != null) StopCoroutine(enableAllCardCoroutine);
        enableAllCardCoroutine = StartCoroutine(EnableAllCardCoroutine());
    }
    private IEnumerator EnableAllCardCoroutine()
    {
        var currentLevelController = gameManager.GetCurrentLevelController();
        float time = GameConfig.totalTimeEnaleCard / (float)currentLevelController.GetAllCard().Count;

        foreach (var card in currentLevelController.GetAllCard())
        {
            yield return new WaitForSeconds(time);

            card.gameObject.SetActive(true);

            var cardMatchingController = card.GetComponent<CardController>();
            if (cardMatchingController) cardMatchingController.EnableCard();
        }
        StartLevel();
    }

    private void StartLevel()
    {
        if (gameManager.GetCurrentLevel() == 1)
        {
            StartGame();
            return;
        }

        if (startLevelCoroutine != null) StopCoroutine(startLevelCoroutine);
        startLevelCoroutine = StartCoroutine(StartLevelCoroutine());
    }

    private IEnumerator StartLevelCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        infoPanel.gameObject.SetActive(true);
        levelText.gameObject.SetActive(false);
        fakeCardViewHolder.gameObject.SetActive(false);
        fakeCardText.gameObject.SetActive(false);
        fakeCardViewHolder.transform.localPosition = Vector3.zero;

        var rectTransform = infoPanel.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);

        yield return new WaitForSeconds(0.5f);
        yield return rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 400), 0.5f).WaitForCompletion();

        levelText.gameObject.SetActive(true);
        SetLevelText(gameManager.GetCurrentLevel());

        yield return new WaitForSeconds(0.5f);
        yield return levelText.transform.DOLocalMoveY(130, 0.5f).WaitForCompletion();
        yield return new WaitForSeconds(0.2f);

        fakeCardText.gameObject.SetActive(true);
        fakeCardText.transform.localScale = Vector3.zero;

        yield return fakeCardText.transform.DOScale(Vector3.one, 0.3f).WaitForCompletion();

        fakeCardViewHolder.gameObject.SetActive(true);
        fakeCardViewHolder.transform.localScale = Vector3.zero;

        yield return fakeCardViewHolder.transform.DOScale(Vector3.one, 0.5f).WaitForCompletion();
        yield return new WaitForSeconds(1f);

        infoPanel.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        yield return fakeCardViewHolder.transform.DOLocalMoveY(430, 0.5f).WaitForCompletion();

        StartGame();
    }

    private void SetLevelText(int level)
    {
        levelText.text = $"Level: <color=#ffff00>{level}</color>";
    }

    public void SetFakeCardViewList(List<CardController> list)
    {
        var fakeCardList = list;

        foreach (var fakeCard in fakeCardList)
        {
            Debug.Log(GetFakeCardInactive());
            FakeCardView fakeCardViewComponent;
            if (GetFakeCardInactive())
            {
                fakeCardViewComponent = GetFakeCardInactive();
                fakeCardViewComponent.gameObject.SetActive(true);
            }
            else
            {
                var prefab = Resources.Load<GameObject>("Prefabs/FakeCardView");
                var fakeCardView = Instantiate(prefab, fakeCardViewHolder.transform);


                fakeCardView.TryGetComponent<FakeCardView>(out fakeCardViewComponent);
            }

            fakeCardViewComponent.SetCardType(fakeCard.Card.GetCardType());
            fakeCardViewComponent.GetDataInit();
        }
    }


    private FakeCardView GetFakeCardInactive()
    {
        foreach (Transform card in fakeCardViewHolder)
        {
            if (!card.gameObject.activeSelf)
            {
                if(card.TryGetComponent<FakeCardView>(out var fakeCardView))
                {
                    return fakeCardView;
                }
            }
        }
        return null;
    }


    private void DisableFakeCardViewList()
    {
        foreach (Transform card in fakeCardViewHolder)
        {
            if (card.gameObject.activeSelf) card.gameObject.SetActive(false);
        }
    }

}
