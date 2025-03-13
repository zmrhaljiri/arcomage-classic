using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CardInstanceController : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent<CardInstanceController, PointerEventData> OnCardClick = new UnityEvent<CardInstanceController, PointerEventData>();

    CardUIController _cardUIController;
    CardDataController _cardDataController;

    static CardStateController _cardStateController;

    public void Initialize(CardDataController cardData)
    {
        _cardDataController = cardData;
        _cardStateController = CardStateController.Instance;
        _cardUIController = GetComponent<CardUIController>();

        _cardStateController.RegisterCardInGame(this);
        _cardUIController.InitializeUI(_cardDataController);
    }    

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCardClick.Invoke(this, eventData);
    }

    public CardDataController GetCardData()
    {
        return _cardDataController;
    }

    public bool IsInPlayerHand()
    {
        return _cardStateController.CardsInPlayerHand.Contains(this);
    }

    public bool IsInUsedZone()
    {
        return transform.parent.CompareTag("UsedZone");
    }

    public bool HasAdditionalTurn()
    {
        return _cardDataController.additionalTurn;
    }

    public bool HasDiscard()
    {
        return _cardDataController.discard;
    }

    public bool IsDiscardable()
    {
        return _cardDataController.discardable;
    }

    void OnDestroy()
    {
        _cardStateController.UnregisterCardInGame(this);
    }    
}
