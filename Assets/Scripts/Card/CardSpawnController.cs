using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using static Paths;

public class CardSpawnController : MonoBehaviour
{
    [SerializeField] GameObject _card, _cardBack, _cardSlot;

    StatsManager _statsManager;
    PlayAreaManager _playAreaManager;
    CardAnimationController _cardAnimationController;
    CardDatabaseController _cardDatabaseController;
    CardGameplayController _cardGameplayController;
    CardStateController _cardStateController;

    GameObject _handSlot;
    Transform _playerHand, _enemyHand;

    void Start()
    {
        _statsManager = StatsManager.Instance;

        _playAreaManager = PlayAreaManager.Instance;
        _cardAnimationController = GetComponent<CardAnimationController>();
        _cardDatabaseController = GetComponent<CardDatabaseController>();   
        _cardGameplayController = GetComponent<CardGameplayController>();
        _cardStateController = GetComponent<CardStateController>();

        _handSlot = _playAreaManager.HandSlot;
        _playerHand = _playAreaManager.PlayerHand;
        _enemyHand = _playAreaManager.EnemyHand;
    }    

    public GameObject InstantiateHandSlot(Transform position)
    {
        return Instantiate(_handSlot, position);
    }

    public GameObject InstantiateCardSlot(Transform position)
    {
        return Instantiate(_cardSlot, position);
    }

    public GameObject InstantiateCardBack(Vector3 position, Transform parent)
    {
        return Instantiate(_cardBack, position, Quaternion.identity, parent);
    }
    
    public void SpawnCard(CardDataController cardData, Transform hand, Vector2 randomOffset)
    {
        GameObject handSlot = InstantiateHandSlot(hand);
        Transform handSlotTransform = handSlot.GetComponent<Transform>();
        GameObject cardSlot = InstantiateCardSlot(handSlotTransform);
        GameObject cardObject = InstantiateCard(cardSlot.transform);
        CardUIController cardUI = cardObject.GetComponent<CardUIController>();
        CardInstanceController card = cardObject.GetComponent<CardInstanceController>();

        bool isCardInPlayerHand = _playAreaManager.IsCardInPlayerHand(hand);
        int lastPlayedCardIndex = _cardStateController.GetLastPlayedCardIndex(isCardInPlayerHand);

        cardObject.GetComponent<RectTransform>().SetAsFirstSibling();
        _cardAnimationController.ShiftCard(cardSlot.GetComponent<RectTransform>(), randomOffset);

        if (isCardInPlayerHand)
        {
            _cardStateController.RegisterCardInPlayerHand(card);
        }
        else
        {
            cardUI.ShowBackSide();
        }

        if (lastPlayedCardIndex != -1)
        {
            handSlotTransform.SetSiblingIndex(lastPlayedCardIndex);
        }

        card.Initialize(cardData);

        InitializeClickListener(card);
    }

    public IEnumerator DealCards(int[] cardIds = null)
    {
        _cardStateController.SetCanInteractWithCards(false);

        for (int i = 1; i < Constants.GameplayLimits.CardsInHand; i++)
        {
            // Uncomment to spawn specific cards
            
            //int[] playerCards = new int[] { 40, 63, 63, 63, 63, 63 };
            //int[] enemyCards = new int[] { 40, 70, 70, 70, 70, 70 };
            //DrawCard(_playerHand, false, playerCards?[i - 1]);
            //DrawCard(_enemyHand, false, enemyCards?[i - 1]);

            // And then comment this

            DrawCard(_playerHand, false);
            DrawCard(_enemyHand, false);
        }

        yield return new WaitForSeconds(Constants.Durations.RenderDelay);

        DrawCard(_playerHand, true, cardIds?[Constants.GameplayLimits.CardsInHand - 1]);
    }

    public void DrawCard(Transform hand, bool withAnimation = true, int? cardId = null)
    {
        int CountNonDiscardableCards(Transform hand)
        {
            int count = 0;

            foreach (Transform handSlot in hand)
            {
                CardInstanceController card = handSlot.GetChild(0).GetComponentInChildren<CardInstanceController>();

                if (card != null && !card.GetCardData().discardable)
                {
                    count++;
                }
            }

            return count;
        }

        string GetRandomRarity()
        {
            int rarityIndex = Random.Range(1, 101);
            string rarity = null;

            foreach (var range in Constants.RarityRanges.RarityRange)
            {
                if (rarityIndex <= range.Key)
                {
                    rarity = range.Value;
                    break;
                }
            }

            return rarity;
        }

        CardDataController ChooseCard(int? specificCardId)
        {
            if (specificCardId.HasValue)
            {
                return _cardDatabaseController.CardData.cards.FirstOrDefault(card => card.id == specificCardId.Value);
            }

            int nonDiscardableCardsInHand = CountNonDiscardableCards(hand);
            List<CardDataController> cards;

            if (nonDiscardableCardsInHand >= 2)
            {
                cards = _cardDatabaseController.CardData.cards
                     .Where(card => card.discardable).ToList();
            }
            else
            {
                cards = _cardDatabaseController.CardData.cards;
            }

            string cardRarity = GetRandomRarity();

            List<CardDataController> filteredCards = cards
                    .Where(card => card.rarity == cardRarity)
                    .ToList();

            return filteredCards[Random.Range(0, filteredCards.Count)];
        }

        CardDataController newCardData = ChooseCard(cardId);

        if (newCardData == null)
        {
            Debug.LogWarning("Card with specified ID not found. Falling back to random selection.");
            newCardData = ChooseCard(null);
        }

        if (withAnimation)
        {
            StartCoroutine(_cardGameplayController.MoveCardToHand(newCardData, hand));
        }
        else
        {
            Vector3 randomOffset = Utils.GetRandomOffset();
            SpawnCard(newCardData, hand, randomOffset);
        }
    }

    GameObject InstantiateCard(Transform position)
    {
        return Instantiate(_card, position);
    }

    void InitializeClickListener(CardInstanceController card)
    {
        card.OnCardClick.AddListener((clickedCard, eventData) =>
        {
            if (_cardStateController.IsCardInPlayerHand(clickedCard) && _cardStateController.CanInteractWithCards)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    if (_cardStateController.PlayerMustDiscardCard)
                    {
                        _cardGameplayController.DiscardCard(clickedCard.gameObject);
                    }
                    else if (_statsManager.CanPlayCard(clickedCard.GetCardData()))
                    {
                        StartCoroutine(_cardGameplayController.PlayCard(clickedCard.gameObject));
                    }
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    _cardGameplayController.DiscardCard(clickedCard.gameObject);
                }
            }
        });
    }
}
