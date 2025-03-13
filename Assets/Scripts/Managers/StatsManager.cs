using UnityEngine;
using static CardDataController;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    public StatsPlayerController PlayerStatsController { get; private set; }
    public StatsPlayerController EnemyStatsController { get; private set; }

    [SerializeField] StatsUIController _statsUIController;

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
        _playerManager = PlayerManager.Instance;
        PlayerStatsController = new StatsPlayerController();
        EnemyStatsController = new StatsPlayerController();
    }

    public Stats PlayerStats => PlayerStatsController._stats;
    public Stats EnemyStats => EnemyStatsController._stats;

    public void InitializeStats()
    {
        PlayerStatsController.SetInitialStats();
        EnemyStatsController.SetInitialStats();
        _statsUIController.InitializeUI();
    }

    public void HandlePlayerSwitch()
    {
        GetActivePlayerStats().GenerateResources();
    }

    public void ApplyCardStats(GameObject card)
    {
        StatsPlayerController activePlayerStats = GetActivePlayerStats();
        StatsPlayerController activeEnemyStats = GetActiveEnemyStats();

        activePlayerStats.ApplyCardStats(card, activeEnemyStats);
        _statsUIController.UpdateStatsUI();
    }

    public bool CanPlayCard(CardDataController card)
    {
        return GetActivePlayerStats().CanPlayCard(card);
    }

    public void DeductCardCost(CardDataController cardData)
    {
        GetActivePlayerStats().DeductCardCost(cardData);
    }

    public void UpdateStatsUI()
    {
        _statsUIController.UpdateStatsUI();
    }

    public void InitializeUI()
    {
        _statsUIController.InitializeUI();
    }

    StatsPlayerController GetActivePlayerStats()
    {
        return _playerManager.IsPlayerTurn() ? PlayerStatsController : EnemyStatsController;
    }

    StatsPlayerController GetActiveEnemyStats()
    {
        return _playerManager.IsPlayerTurn() ? EnemyStatsController : PlayerStatsController;
    }
}
