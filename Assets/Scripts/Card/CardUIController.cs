using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _cardName;
    [SerializeField] Image _image;
    [SerializeField] GameObject _textDiscarded;
    [SerializeField] TextMeshProUGUI _textDescription;
    [SerializeField] TextMeshProUGUI _textCost;
    [SerializeField] GameObject _originalImage;
    [SerializeField] GameObject _textNotEnoughResources;
    [SerializeField] GameObject _cardBack;
    [SerializeField] GameObject _iconBackground;
    [SerializeField] Image _icon;

    CardInstanceController _cardInstanceController;

    static CardStateController _cardStateController;
    static StatsManager _statsManager;
    static PlayerManager _playerManager;
    static OptionsManager _optionsManager;
    static ResourceManager _resourceManager;
    
    public void InitializeUI(CardDataController cardData)
    {
        InitializeDependencies();

        ToggleOriginalImage(_optionsManager.options.originalImages);

        if (!isCardBackShown())
        {
            ToggleIconBackground(_optionsManager.options.showGeneratorIcons);
        }

        _cardName.text = cardData.cardName;
        _textDescription.text = cardData.description;
        _textCost.text = cardData.cost.ToString();

        _image.sprite = _resourceManager.GetSprite(cardData.stringId);
        _originalImage.GetComponent<Image>().sprite = _resourceManager.GetSprite($"{cardData.stringId}-original");
        _icon.sprite = _resourceManager.GetSprite(GetIconSprite(cardData.type));
        GetComponent<Image>().sprite = _resourceManager.GetSprite(GetCardSprite(cardData.type));

        UpdateTransparency(_statsManager.CanPlayCard(cardData));
    }    

    public static void ToggleOriginalImages(bool isOn)
    {
        if (!_cardStateController) return;

        foreach (CardInstanceController card in _cardStateController.CardsInGame)
        {
            CardUIController cardUI = card.GetComponent<CardUIController>();
            cardUI.ToggleOriginalImage(isOn);
            cardUI.UpdateTransparency(_statsManager.CanPlayCard(card.GetCardData()));
        }
    }
    public static void ToggleIcons(bool isOn)
    {
        if (!_cardStateController) return;

        foreach (CardInstanceController card in _cardStateController.CardsInGame)
        {
            CardUIController cardUI = card.GetComponent<CardUIController>();
            cardUI.ToggleIcon(isOn);
            cardUI.UpdateTransparency(_statsManager.CanPlayCard(card.GetCardData()));
        }
    }

    public static void ToggleNotEnoughResourcesInPlayerCards()
    {
        if (!_cardStateController) return;

        foreach (CardInstanceController card in _cardStateController.CardsInPlayerHand)
        {
            CardUIController cardUI = card.GetComponent<CardUIController>();
            cardUI.ToggleNotEnoughResources();
        }
    }

    public static void UpdateTransparencyInAllCards()
    {
        Transform activeHand = _playerManager.GetActivePlayerHand();        

        foreach (Transform handSlot in activeHand)
        {
            Transform cardSlot = handSlot.GetChild(0);
            CardUIController cardUI = cardSlot.GetComponentInChildren<CardUIController>();
            CardInstanceController card = cardSlot.GetComponentInChildren<CardInstanceController>();

            if (cardUI != null)
            {
                CardDataController cardData = card.GetCardData();
                bool canPlay = StatsManager.Instance.CanPlayCard(cardData);
                cardUI.UpdateTransparency(canPlay);
            }
        }
    }

    public void UpdateTransparency(bool isFullyVisible)
    {        
        if (_cardInstanceController.IsInPlayerHand())
        {            
            ToggleNotEnoughResources();
        }

        if (_cardInstanceController.IsInUsedZone())
        {
            isFullyVisible = false;            
        }        

        // Setting transparency of underlying elements to 0 for cases when the original image is faded out
        bool originalCardsUsed = _optionsManager.options.originalImages;

        // Updating transparency of images and text separately, because text would appear too gray if we'd use canvas group and update its alpha.
        float iconTransparency = isFullyVisible ? 1 : Constants.Durations.CardImageFadeOut;
        float imageTransparency = isFullyVisible ? 1 : originalCardsUsed ? 0 : Constants.Durations.CardImageFadeOut;
        float textTransparency = isFullyVisible ? 1 : originalCardsUsed ? 0 : Constants.Durations.CardTextFadeOut;
        float originalImageTransparency = isFullyVisible ? 1 : Constants.Durations.CardImageFadeOut;

        SetAlpha(GetComponent<Image>(), imageTransparency);
        SetAlpha(_originalImage.GetComponent<Image>(), originalImageTransparency);
        SetAlpha(_image, imageTransparency);
        SetAlpha(_icon, iconTransparency);
        SetAlpha(_cardName, textTransparency);
        SetAlpha(_textDescription, textTransparency);
        SetAlpha(_textCost, textTransparency);

        void SetAlpha(Graphic graphic, float alpha)
        {
            Utils.SetGraphicAlpha(graphic, alpha);
        }
    }

    public void ShowDiscardedText()
    {
        if (_textDiscarded != null)
        {
            _textDiscarded.SetActive(true);
        }
    }

    public void ShowBackSide()
    {
        if (_cardBack != null)
        {
            _cardBack.SetActive(true);

            if (_iconBackground != null)
            {
                ToggleIconBackground(false);
            }
        }
    }

    public void ShowFrontSide()
    {
        if (_cardBack != null)
        {
            _cardBack.SetActive(false);

            if (_iconBackground != null && _optionsManager.options.showGeneratorIcons)
            {
                ToggleIconBackground(true);
            }
        }
    }        
    
    public void HideNotEnoughResourcesText()
    {
        _textNotEnoughResources.SetActive(false);
    }

    static string GetCardSprite(string cardType)
    {
        switch (cardType)
        {
            case Constants.Generators.Quarries:
                return Constants.Images.Background.CardQuarries;
            case Constants.Generators.Magic:
                return Constants.Images.Background.CardMagic;
            case Constants.Generators.Dungeons:
                return Constants.Images.Background.CardDungeons;
            default:
                return null;
        }
    }

    static string GetIconSprite(string cardType)
    {
        switch (cardType)
        {
            case Constants.Generators.Quarries:
                return Constants.Images.Icon.Quarries;
            case Constants.Generators.Magic:
                return Constants.Images.Icon.Magic;
            case Constants.Generators.Dungeons:
                return Constants.Images.Icon.Dungeons;
            default:
                return null;
        }
    }

    void ToggleOriginalImage(bool show)
    {
        if (_originalImage != null)
        {
            _originalImage.SetActive(show);
        }
    }

    void ToggleIcon(bool showIcon)
    {
        if (_cardBack != null && isCardBackShown())
        {
            showIcon = false;
        }

        ToggleIconBackground(showIcon);
    }

    void ToggleNotEnoughResources()
    {
        bool showText = _optionsManager.options.showNotEnoughResources && !_statsManager.CanPlayCard(_cardInstanceController.GetCardData());

        _textNotEnoughResources.SetActive(showText);
    }

    void ToggleIconBackground(bool show)
    {
        if (_iconBackground != null)
        {
            _iconBackground.SetActive(show);
        }
    }

    bool isCardBackShown()
    {
        return _cardBack.activeSelf;
    }

    void InitializeDependencies()
    {
        _cardInstanceController = GetComponent<CardInstanceController>();
        _cardStateController = CardStateController.Instance;
        _statsManager = StatsManager.Instance;
        _optionsManager = OptionsManager.Instance;
        _resourceManager = ResourceManager.Instance;
        _playerManager = PlayerManager.Instance;
    }
}
