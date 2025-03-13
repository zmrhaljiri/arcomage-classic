using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] Button _buttonPlayAgain;
    [SerializeField] Button _buttonExitGame;
    PopupManager _popupManager;
    GameManager _gameManager;
    PlayerManager _playerManager;

    void Start()
    {        
        _popupManager = PopupManager.Instance;
        _gameManager = GameManager.Instance;
        _playerManager = PlayerManager.Instance;

        _popupManager.RegisterPopup(PopupType.GameOver, gameObject);
        gameObject.SetActive(false);

        _buttonPlayAgain.onClick.AddListener(OnButtonPlayAgainClick);
        _buttonExitGame.onClick.AddListener(OnButtonExitGameClick);
    }

    public void SetWinnerMessage(string playerName)
    {
        _title.text = $"{playerName} {Constants.Messages.HasWon}";
    }

    void OnButtonPlayAgainClick()
    {
        _popupManager.ClosePopup(PopupType.GameOver);
        _gameManager.StartNewGame();
    }

    void OnButtonExitGameClick()
    {
        _gameManager.ExitGame();
    }

    void OnEnable()
    {
        if (_gameManager && _gameManager.GameStarted)
        {
            string winnerName = _playerManager.Winner == Player.Player
                ? Constants.Players.PlayerName
                : Constants.Players.EnemyName;

            SetWinnerMessage(winnerName);
        }
    }
}
