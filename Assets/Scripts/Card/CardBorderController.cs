using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardBorderController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _topBorder, _rightBorder, _bottomBorder, _leftBorder;
    Image[] _borders;

    PlayerManager _playerManager;
    StatsManager _statsManager;

    void Awake()
    {
        _playerManager = PlayerManager.Instance;
        _statsManager = StatsManager.Instance;

        _borders = new Image[]
        {
            _topBorder.GetComponent<Image>(),
            _rightBorder.GetComponent<Image>(),
            _bottomBorder.GetComponent<Image>(),
            _leftBorder.GetComponent<Image>()
        };
    }

    void Start()
    {
        SetBlackColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHoverColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetBlackColor();
    }

    void SetBorderColor(Color color, float alpha)
    {
        color.a = alpha;

        foreach (var border in _borders)
        {
            border.color = color;
        }
    }

    void SetHoverColor()
    {
        CardInstanceController childCard = GetComponentInChildren<CardInstanceController>();

        if (childCard == null) return;

        CardDataController cardData = childCard.GetCardData();

        if (_playerManager.IsPlayerTurn())
        {
            if (_statsManager.CanPlayCard(cardData))
            {
                SetWhiteColor();
            }
            else
            {
                SetRedColor();
            }
        }
    }

    void SetBlackColor()
    {
        SetBorderColor(Color.black, 1);
    }

    void SetWhiteColor()
    {
        SetBorderColor(Color.white, 1);
    }

    void SetRedColor()
    {
        SetBorderColor(Color.red, 1);
    }    
}
