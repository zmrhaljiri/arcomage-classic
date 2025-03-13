// The central interface for external systems that delegates card-related tasks to other controllers

using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [SerializeField] CardAnimationController _cardAnimationController;
    [SerializeField] CardGameplayController _cardGameplayController;
    [SerializeField] CardSpawnController _cardSpawnController;
    [SerializeField] CardStateController _cardStateController;    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayCard(GameObject cardObject)
    {
        StartCoroutine(_cardGameplayController.PlayCard(cardObject));
    }

    public void DiscardCard(GameObject cardObject)
    {        
        _cardGameplayController.DiscardCard(cardObject);        
    }    

    public void DealCards()
    {
        StartCoroutine(_cardSpawnController.DealCards());
    }

    public void StopAllCardAnimations()
    {
        _cardAnimationController.StopAllCardCoroutines();
    }

    public void CheckCardsInActiveHand(Transform activeHand)
    {
        _cardGameplayController.RefillHandIfNeeded(activeHand);
    }

    public void UpdateTransparencyInAllCards()
    {
        CardUIController.UpdateTransparencyInAllCards();
    }

    public void ClearCards()
    {
        _cardStateController.ClearCards();
    }

    public void SetPlayingFirstCardInRound(bool playingFirstCardInRound)
    {
        _cardStateController.SetPlayingFirstCardInRound(playingFirstCardInRound);
    }

    public void SetInitialState()
    {
        _cardStateController.SetInitialState();
    }

    public bool PlayerMustDiscardCard => _cardStateController.PlayerMustDiscardCard;
}
