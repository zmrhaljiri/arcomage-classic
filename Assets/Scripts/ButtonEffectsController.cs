using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonEffectsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TextMeshProUGUI _buttonText;
    [SerializeField] Color _hoverColor;
    [SerializeField] Color _hoverGlowColor;
    [SerializeField] Color _pressedColor;
    Vector3 _originalPosition;
    Color _originalColor;
    bool _pointerHovering = false;
    bool _pointerPressed = false;

    void Start()
    {
        _originalPosition = _buttonText.rectTransform.localPosition;
        _originalColor = _buttonText.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetPointerHovering(true);

        if (!_pointerPressed)
        {
            SetGlowOn();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetPointerHovering(false);
        SetGlowOff();

        if (!_pointerPressed)
        {
            SetOriginalColor();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SetPointerPressed(true);
            SetPressedColor();
            SetShiftedPosition();
            SetGlowOff();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SetPointerPressed(false);
            SetOriginalPosition();

            if (_pointerHovering)
            {
                SetGlowOn();
            }
            else
            {
                SetOriginalColor();
            }
        }
    }

    void SetGlowOn()
    {
        _buttonText.fontMaterial.EnableKeyword("GLOW_ON");
        _buttonText.fontMaterial.SetColor("_GlowColor", _hoverGlowColor);
        _buttonText.fontMaterial.SetFloat("_GlowOuter", 1f);
        _buttonText.fontMaterial.SetFloat("_GlowPower", 0.45f);

        SetHoverColor();
    }

    void SetGlowOff()
    {
        _buttonText.fontMaterial.DisableKeyword("GLOW_ON");
    }

    void SetOriginalColor()
    {
        _buttonText.color = _originalColor;
    }

    void SetHoverColor()
    {
        _buttonText.color = _hoverColor;
    }

    void SetPressedColor()
    {
        _buttonText.color = _pressedColor;
    }

    void SetOriginalPosition()
    {
        _buttonText.rectTransform.localPosition = _originalPosition;
    }

    void SetShiftedPosition()
    {
        _buttonText.rectTransform.localPosition = _originalPosition + new Vector3(-2, -2, 0);
    }

    void SetPointerHovering(bool isHovering)
    {
        _pointerHovering = isHovering;
    }

    void SetPointerPressed(bool isPressed)
    {
        _pointerPressed = isPressed;
    }
}
