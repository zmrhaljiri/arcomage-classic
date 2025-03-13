// Handles card-related state management

using System.Collections.Generic;
using UnityEngine;

public class CardStateController : MonoBehaviour
{
    public static CardStateController Instance { get; private set; }

    public List<CardInstanceController> CardsInGame = new List<CardInstanceController>();
    public List<CardInstanceController> CardsInPlayerHand = new List<CardInstanceController>();

    public bool CanInteractWithCards;
    public bool PlayerMustDiscardCard;
    public bool PlayingDiscardCard;
    public bool PlayingFirstCardInRound;
    
    int _playerLastPlayedCardIndex = -1;
    int _enemyLastPlayedCardIndex = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        SetInitialState();
    }

    public void SetInitialState()
    {
        CanInteractWithCards = false;
        PlayerMustDiscardCard = false;
        PlayingDiscardCard = false;
        PlayingFirstCardInRound = false;
    }

    public void RegisterCardInGame(CardInstanceController card) => CardsInGame.Add(card);
    public void UnregisterCardInGame(CardInstanceController card) => CardsInGame.Remove(card);
    public void RegisterCardInPlayerHand(CardInstanceController card) => CardsInPlayerHand.Add(card);

    public void UnregisterCardInPlayerHand(CardInstanceController card)
    {
        CardsInPlayerHand.Remove(card);
        card.GetComponent<CardUIController>().HideNotEnoughResourcesText();
    }
    public bool IsCardInPlayerHand(CardInstanceController card)
    {
        return CardsInPlayerHand.Contains(card);
    }
    public void ClearCards()
    {
        CardsInGame.Clear();
        CardsInPlayerHand.Clear();
    }
    public void SetCanInteractWithCards(bool canInteract) => CanInteractWithCards = canInteract;    
    public void SetPlayerMustDiscardCard(bool mustDiscard) => PlayerMustDiscardCard = mustDiscard;
    public void SetPlayingDiscardCard(bool isPlayingDiscard) => PlayingDiscardCard = isPlayingDiscard;
    public void SetPlayingFirstCardInRound(bool playingFirstCardInRound) => PlayingFirstCardInRound = playingFirstCardInRound;
    public int GetLastPlayedCardIndex(bool isPlayerHand) => isPlayerHand ? _playerLastPlayedCardIndex : _enemyLastPlayedCardIndex;    

    public void SetLastPlayedCardIndex(int index, bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            _playerLastPlayedCardIndex = index;
        }
        else
        {
            _enemyLastPlayedCardIndex = index;
        }
    }
}
