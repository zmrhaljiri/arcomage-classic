using UnityEngine;
using static CardDataController;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    
    
    public bool GameStarted;                
    public bool IsGameOver;    

    [SerializeField] Canvas _introCanvas;
    [SerializeField] Canvas _playAreaCanvas;
    [SerializeField] Canvas _popupsCanvas;    

    AIManager _aiManager;
    AudioManager _audioManager;
    CardManager _cardManager;    
    StatsManager _statsManager;
    PlayAreaManager _playAreaManager;
    PopupManager _popupManager;
    ResourceManager _resourceManager;
    PlayerManager _playerManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        InitializeDependencies();
        SetInitialUI();                
        PreloadImageResources();
    }

    public void StartNewGame()
    {        
        ClearCoroutines();
        
        _cardManager.ClearCards();
        _playAreaManager.ClearUI();
        _statsManager.InitializeStats();

        SetNewGameState();
        SetNewGameUI();

        _cardManager.DealCards();
    }

    public void NewTurn()
    {
        _cardManager.SetPlayingFirstCardInRound(true);        
        _playerManager.SwitchPlayers();

        Transform activeHand = _playerManager.GetActivePlayerHand();
        _cardManager.CheckCardsInActiveHand(activeHand);
        _cardManager.UpdateTransparencyInAllCards();
        _statsManager.UpdateStatsUI();

        if (_playerManager.ActivePlayer == Player.Enemy)
        {
            StartCoroutine(_aiManager.HandleEnemyTurn());
        }
    }
    public void CheckForGameEnd()
    {
        Stats playerStats = _statsManager.PlayerStats;
        Stats enemyStats = _statsManager.EnemyStats;

        bool playerLostWithTower = playerStats.tower <= Constants.VictoryConditions.DefeatTowerValue;
        bool playerWonWithTower = playerStats.tower >= Constants.VictoryConditions.VictoryTowerValue;
        bool playerWonWithResources = playerStats.bricks >= Constants.VictoryConditions.VictoryResourceValue || playerStats.gems >= Constants.VictoryConditions.VictoryResourceValue || playerStats.recruits >= Constants.VictoryConditions.VictoryResourceValue;

        bool enemyLostWithTower = enemyStats.tower <= Constants.VictoryConditions.DefeatTowerValue;
        bool enemyWonWithTower = enemyStats.tower >= Constants.VictoryConditions.VictoryTowerValue;
        bool enemyWonWithResources = enemyStats.bricks >= Constants.VictoryConditions.VictoryResourceValue || enemyStats.gems >= Constants.VictoryConditions.VictoryResourceValue || enemyStats.recruits >= Constants.VictoryConditions.VictoryResourceValue;

        if (playerWonWithTower || playerWonWithResources || enemyLostWithTower)
        {
            EndGame(Player.Player);
        }
        else if (enemyWonWithTower || enemyWonWithResources || playerLostWithTower)
        {
            EndGame(Player.Enemy);
        }
    }    

    public void ExitGame()
    {
        Application.Quit();
    }    

    public void SetLowFPS(bool isOn)
    {
        if (isOn)
        {
            Application.targetFrameRate = 25;
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 1;
        }
    }

    void SetIsGameOver(bool isGameOver)
    {
        IsGameOver = isGameOver;
    }

    void SetGameStarted(bool gameStarted)
    {
        GameStarted = gameStarted;
    }

    void EndGame(Player winner)
    {
        SetIsGameOver(true);
        _playerManager.SetWinner(winner);
        _popupManager.OpenPopup(PopupType.GameOver);        
        PlayEndGameSound(winner);        
    }

    void PlayEndGameSound(Player winner)
    {
        if (winner == Player.Player)
        {
            _audioManager.PlayGameplaySound(Constants.Sounds.GameWin);
            _audioManager.PlayGameplaySound(Constants.Sounds.VictoryIsOurs);
        }
        else
        {
            _audioManager.PlayGameplaySound(Constants.Sounds.GameOver);
        }
    }    

    void SetCanvasVisible(Canvas canvas, bool isVisible)
    {        
        canvas.gameObject.SetActive(isVisible);
    }    

    void InitializeDependencies()
    {
        _aiManager = AIManager.Instance;
        _audioManager = AudioManager.Instance;
        _cardManager = CardManager.Instance;        
        _statsManager = StatsManager.Instance;
        _playAreaManager = PlayAreaManager.Instance;
        _popupManager = PopupManager.Instance;
        _resourceManager = ResourceManager.Instance;
        _playerManager = PlayerManager.Instance;
    }

    void SetInitialUI()
    {
        SetCanvasVisible(_introCanvas, true);
        SetCanvasVisible(_popupsCanvas, true);
        SetCanvasVisible(_playAreaCanvas, false);        
    }

    void SetNewGameState()
    {
        SetGameStarted(true);
        SetIsGameOver(false);        
        _playerManager.SetWinner(null);
        _cardManager.SetInitialState();
        _playAreaManager.SetDiscardTextVisible(false);
        _playerManager.SetActivePlayer(Player.Player);
    }

    void ClearCoroutines()
    {
        StopAllCoroutines();
        _cardManager.StopAllCardAnimations();
    }

    void SetNewGameUI()
    {
        SetCanvasVisible(_introCanvas, false);
        SetCanvasVisible(_playAreaCanvas, true);
        _statsManager.InitializeUI();
    }

    void PreloadImageResources()
    {        
        _resourceManager.PreloadResources(Paths.Cards.Base);        
    }
}