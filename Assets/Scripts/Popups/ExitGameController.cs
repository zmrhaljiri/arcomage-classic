using UnityEngine;
using UnityEngine.UI;

public class ExitGameController : MonoBehaviour
{
    [SerializeField] Button _buttonConfirm;
    [SerializeField] Button _buttonCancel;
    PopupManager _popupManager;
    GameManager _gameManager;

    void Start()
    {
        _popupManager = PopupManager.Instance;
        _gameManager = GameManager.Instance;

        _popupManager.RegisterPopup(PopupType.ExitGame, gameObject);
        gameObject.SetActive(false);

        _buttonConfirm.onClick.AddListener(OnButtonConfirmClick);
        _buttonCancel.onClick.AddListener(OnButtonCancelClick);
    }

    void OnButtonConfirmClick()
    {
        _popupManager.ClosePopup(PopupType.ExitGame);
        _gameManager.ExitGame();
    }

    void OnButtonCancelClick()
    {
        _popupManager.ClosePopup(PopupType.ExitGame);
    }
}
