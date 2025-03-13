using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameplayController : MonoBehaviour
{
    public static CardGameplayController Instance { get; private set; }
    
    GameManager _gameManager;
    AIManager _aiManager;
    StatsManager _statsManager;
    PlayAreaManager _playAreaManager;
    PlayerManager _playerManager;
    CardStateController _cardStateController;
    AudioManager _audioManager;
    CardAnimationController _cardAnimationController;        
    CardSpawnController _cardSpawnController;

    GameObject _handSlot;
    Transform _playerHand, _playArea, _actionZone;
    RectTransform _deck, _usedZone;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        InitializeDependencies();
        InitializePlayAreaElements();
    }    

    public IEnumerator PlayCard(GameObject cardObject, bool isDiscardTurn = false)
    {
        Transform activeHand = _playerManager.GetActivePlayerHand();
        CardUIController cardUI = cardObject.GetComponent<CardUIController>();
        CardInstanceController card = cardObject.GetComponent<CardInstanceController>();

        yield return HandleImmediateCardActions(isDiscardTurn, card, cardObject, activeHand, _usedZone, cardUI);

        RemoveCardFromPlayArea(cardObject);

        if (!isDiscardTurn)
        {
            yield return MoveCardToActionZone(cardObject, card, activeHand);
        }

        yield return MoveCardToUsedZone(isDiscardTurn, cardObject, _usedZone, cardUI);

        HandlePostCardActions(isDiscardTurn, card, activeHand);
    }

    public void DiscardCard(GameObject cardObject)
    {
        if (cardObject.GetComponent<CardInstanceController>().IsDiscardable())
        {
            StartCoroutine(PlayCard(cardObject, true));
        }
    }

    public void RefillHandIfNeeded(Transform activeHand)
    {
        if (activeHand.childCount < Constants.GameplayLimits.CardsInHand)
        {
            _cardSpawnController.DrawCard(activeHand);
        }
        else
        {
            _cardStateController.SetCanInteractWithCards(true);
        }
    }

    public IEnumerator MoveCardToHand(CardDataController newCardData, Transform hand)
    {
        GameObject cardBack = _cardSpawnController.InstantiateCardBack(_deck.position, _playArea);
        GameObject handSlot = _cardSpawnController.InstantiateHandSlot(hand);

        Vector2 randomOffset = Utils.GetRandomOffset();

        int siblingIndex = _cardStateController.GetLastPlayedCardIndex(_playAreaManager.IsCardInPlayerHand(hand));
        handSlot.transform.SetSiblingIndex(siblingIndex);

        GameObject cardSlot = _cardSpawnController.InstantiateCardSlot(handSlot.GetComponent<Transform>());
        _cardAnimationController.ShiftCard(cardSlot.GetComponent<RectTransform>(), randomOffset);

        yield return new WaitForSeconds(Constants.Durations.RenderDelay);

        Vector3 finalPosition = cardSlot.transform.position;

        yield return StartCoroutine(_cardAnimationController.AnimateCardToHand(cardBack, finalPosition, Constants.Durations.DrawCard));

        Destroy(handSlot);
        _cardSpawnController.SpawnCard(newCardData, hand, randomOffset);
        Destroy(cardBack);

        if (!_cardStateController.PlayingDiscardCard)
        {
            _cardStateController.SetCanInteractWithCards(true);
        }
    }

    IEnumerator HandleImmediateCardActions(bool isDiscardTurn, CardInstanceController card, GameObject cardObject, Transform activeHand, RectTransform usedZone, CardUIController cardUI)
    {
        _cardStateController.SetCanInteractWithCards(false);
        _audioManager.PlayGameplaySound(Constants.Sounds.MoveCard);

        CardDataController cardData = card.GetCardData();

        if (activeHand == _playerHand)
        {
            _cardStateController.UnregisterCardInPlayerHand(card);
        }
        else
        {
            cardUI.ShowFrontSide();
        }

        if (isDiscardTurn && _cardStateController.PlayerMustDiscardCard)
        {
            _playAreaManager.SetDiscardTextVisible(false);
        }
        else if (!isDiscardTurn)
        {
            _statsManager.DeductCardCost(cardData);
            _statsManager.UpdateStatsUI();
        }

        bool shouldShuffleCards = _cardStateController.PlayingFirstCardInRound && usedZone.childCount > 1;

        if (shouldShuffleCards)
        {
            yield return ShuffleCards(usedZone);
            // Without the following pause, discarding a card after enemy turn 
            // with many cards in usedZone causes the card to go to a wrong position
            yield return new WaitForSeconds(0.01f);
        }
    }    

    IEnumerator MoveCardToActionZone(GameObject cardObject, CardInstanceController card, Transform activeHand)
    {        
        yield return StartCoroutine(_cardAnimationController.AnimateCardToActionZone(cardObject, _actionZone.position, Constants.Durations.PlayCard));        

        _statsManager.ApplyCardStats(cardObject);

        if (card.HasDiscard())
        {
            if (_playerManager.IsPlayerTurn())
            {
                _playAreaManager.SetDiscardTextVisible(true);
            }

            _cardStateController.SetPlayingDiscardCard(true);
            _cardSpawnController.DrawCard(activeHand);
        }

        yield return new WaitForSeconds(Constants.Durations.AfterApplyStats);
    }

    IEnumerator MoveCardToUsedZone(bool isDiscardTurn, GameObject cardObject, RectTransform usedZone, CardUIController cardUI)
    {
        GameObject usedZoneSlot = Instantiate(_handSlot, usedZone);
        CardBorderController borderController = usedZoneSlot.GetComponent<CardBorderController>();
        if (borderController != null)
        {
            borderController.enabled = false;
        }

        yield return new WaitForSeconds(Constants.Durations.RenderDelay);
        Vector3 finalPosition = usedZoneSlot.GetComponent<RectTransform>().position;

        if (cardUI == null) yield break;

        cardUI.UpdateTransparency(false);

        yield return StartCoroutine(_cardAnimationController.AnimateCardToUsedZone(cardObject, finalPosition, isDiscardTurn ? Constants.Durations.DrawCard : Constants.Durations.ShuffleCard));

        Destroy(usedZoneSlot);

        cardObject.transform.SetParent(usedZone, false);

        if (isDiscardTurn)
        {
            cardUI.ShowDiscardedText();
        }
        else
        {
            _statsManager.UpdateStatsUI();
        }

        yield return null;
    }         

    IEnumerator ShuffleCards(RectTransform usedZone)
    {
        List<GameObject> usedCards = new List<GameObject>();
        List<Vector3> cardPositions = new List<Vector3>();

        for (int i = 1; i < usedZone.childCount; i++) 
        {
            GameObject usedCard = usedZone.GetChild(i).gameObject;
            RectTransform usedCardRectTransform = usedCard.GetComponent<RectTransform>();
            Vector3 usedZoneWorldPosition = usedCardRectTransform.position;

            usedCards.Add(usedCard);
            cardPositions.Add(usedZoneWorldPosition);
        }

        // Send the animation task to the CardAnimationController
        yield return StartCoroutine(_cardAnimationController.AnimateShuffle(usedCards, cardPositions, _deck, _playArea));
    }

    void RemoveCardFromPlayArea(GameObject cardObject)
    {
        Transform handSlot = cardObject.transform.parent.parent;
        RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
        Vector3 cardWorldPosition = cardRectTransform.position;

        _cardStateController.SetLastPlayedCardIndex(cardRectTransform.parent.parent.GetSiblingIndex(), _playerManager.IsPlayerTurn());
        cardObject.transform.SetParent(_playArea, false);
        Destroy(handSlot.gameObject);

        cardRectTransform.position = cardWorldPosition;
    }

    void HandlePostCardActions(bool isDiscardTurn, CardInstanceController card, Transform activeHand)
    {
        _gameManager.CheckForGameEnd();
        _cardStateController.SetPlayingFirstCardInRound(false);

        if (_gameManager.IsGameOver)
        {
            return;
        }

        if (_cardStateController.PlayingDiscardCard)
        {
            _cardStateController.SetCanInteractWithCards(true);
        }

        if (isDiscardTurn)
        {
            if (_cardStateController.PlayerMustDiscardCard)
            {
                _cardSpawnController.DrawCard(activeHand);
                _cardStateController.SetPlayerMustDiscardCard(false);
            }
            else
            {
                _gameManager.NewTurn();
                return;
            }
        }
        else if (card.HasDiscard())
        {
            _cardStateController.SetPlayerMustDiscardCard(true);
        }
        else if (card.HasAdditionalTurn())
        {
            _cardSpawnController.DrawCard(activeHand);
        }
        else
        {
            _gameManager.NewTurn();
            return;
        }

        if (!_playerManager.IsPlayerTurn())
        {
            StartCoroutine(_aiManager.HandleEnemyTurn());
        }

        _cardStateController.PlayingDiscardCard = false;
    }

    void InitializePlayAreaElements()
    {
        _handSlot = _playAreaManager.HandSlot;
        _deck = _playAreaManager.Deck;
        _usedZone = _playAreaManager.UsedZone;
        _playerHand = _playAreaManager.PlayerHand;
        _playArea = _playAreaManager.PlayArea;
        _actionZone = _playAreaManager.ActionZone;
    }

    void InitializeDependencies()
    {
        _gameManager = GameManager.Instance;
        _aiManager = AIManager.Instance;
        _statsManager = StatsManager.Instance;
        _playerManager = PlayerManager.Instance;
        _playAreaManager = PlayAreaManager.Instance;
        _audioManager = AudioManager.Instance;
        _cardAnimationController = GetComponent<CardAnimationController>();
        _cardStateController = GetComponent<CardStateController>();
        _cardSpawnController = GetComponent<CardSpawnController>();
    }
}
