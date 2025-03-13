using UnityEngine;
using UnityEngine.UI;

public class NewGameController : MonoBehaviour
{
    [SerializeField] Button _buttonConfirm;
    [SerializeField] Button _buttonCancel;
    PopupManager _popupManager;
    MenuManager _menuManager;

    void Start()
    {
        _popupManager = PopupManager.Instance;
        _menuManager = MenuManager.Instance;

        _popupManager.RegisterPopup(PopupType.NewGame, gameObject);   
        gameObject.SetActive(false);

        _buttonConfirm.onClick.AddListener(OnButtonConfirmClick);
        _buttonCancel.onClick.AddListener(OnButtonCancelClick);
    }

    void OnButtonConfirmClick()
    {
        _popupManager.ClosePopup(PopupType.NewGame);
        _menuManager.CloseMenu();
        _menuManager.LaunchNewGame();
    }

    void OnButtonCancelClick()
    {
        _popupManager.ClosePopup(PopupType.NewGame);
    }
}
