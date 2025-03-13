using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Player ActivePlayer { get; private set; }
    public Player ActiveEnemy { get; private set; }
    public Player? Winner {  get; private set; }
    public Transform PlayerHand { get; private set; }
    public Transform EnemyHand { get; private set; }
    
    [SerializeField] Transform _playerHand;
    [SerializeField] Transform _enemyHand;
    [SerializeField] TextMeshProUGUI _playerName;
    [SerializeField] TextMeshProUGUI _enemyName;

    PlayAreaManager _playAreaManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }        
    }

    void Start()
    {
        _playAreaManager = PlayAreaManager.Instance;

        PlayerHand = _playerHand;
        EnemyHand = _enemyHand;

        ActivePlayer = Player.Player;
        ActiveEnemy = Player.Enemy;
        Winner = null;
    }

    public bool IsPlayerTurn()
    {
        return ActivePlayer == Player.Player;
    }

    public void SwitchPlayers()
    {
        SetActivePlayer(ActiveEnemy);
    }

    public void SetWinner(Player? player)
    {
        Winner = player;
    }

    public void SetActivePlayer(Player player)
    {
        ActivePlayer = player;
        ActiveEnemy = GetActiveEnemy();
        bool isPlayerTurn = IsPlayerTurn();

        StatsManager.Instance.HandlePlayerSwitch();
        _playAreaManager.SwitchHands(isPlayerTurn);
        UpdateActivePlayerName(isPlayerTurn);
    }

    public Player GetActiveEnemy()
    {
        return ActivePlayer == Player.Player ? Player.Enemy : Player.Player;
    }

    public Transform GetActivePlayerHand()
    {
        return ActivePlayer == Player.Player ? PlayerHand : EnemyHand;
    }    

    public Transform GetActiveEnemyHand()
    {
        return ActivePlayer == Player.Player ? EnemyHand : PlayerHand;
    }

    public void UpdateActivePlayerName(bool isPlayerTurn)
    {
        _playerName.text = Constants.Players.PlayerName + (isPlayerTurn ? Constants.Players.ActivePlayerMark : "");
        _enemyName.text = Constants.Players.EnemyName + (isPlayerTurn ? "" : Constants.Players.ActivePlayerMark);
    }
}

