using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CardDataController;

public class StatsUIController : MonoBehaviour
{
    public static StatsUIController Instance { get; private set; }

    [SerializeField] TextMeshProUGUI _textPlayerWall, _textPlayerTower, _textPlayerQuarries, _textPlayerBricks, _textPlayerMagic, _textPlayerGems, _textPlayerDungeons, _textPlayerRecruits;
    [SerializeField] TextMeshProUGUI _textEnemyWall, _textEnemyTower, _textEnemyQuarries, _textEnemyBricks, _textEnemyMagic, _textEnemyGems, _textEnemyDungeons, _textEnemyRecruits;
    [SerializeField] RectTransform _playerTower, _playerWall, _enemyTower, _enemyWall;
    float maxTowerHeight, maxWallHeight;

    StatsManager _statsManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        _statsManager = StatsManager.Instance;
    }

    public void InitializeUI()
    {
        maxTowerHeight = _playerTower.sizeDelta.y;
        maxWallHeight = _playerWall.sizeDelta.y;

        UpdateStatsUI();
    }    

    public void UpdateStatsUI()
    {
        Stats playerStats = _statsManager.PlayerStats;
        Stats enemyStats = _statsManager.EnemyStats;

        _textPlayerWall.text = playerStats.wall.ToString();
        _textPlayerTower.text = playerStats.tower.ToString();
        _textPlayerQuarries.text = playerStats.quarries.ToString();
        _textPlayerBricks.text = playerStats.bricks.ToString();
        _textPlayerMagic.text = playerStats.magic.ToString();
        _textPlayerGems.text = playerStats.gems.ToString();
        _textPlayerDungeons.text = playerStats.dungeons.ToString();
        _textPlayerRecruits.text = playerStats.recruits.ToString();

        _textEnemyWall.text = enemyStats.wall.ToString();
        _textEnemyTower.text = enemyStats.tower.ToString();
        _textEnemyQuarries.text = enemyStats.quarries.ToString();
        _textEnemyBricks.text = enemyStats.bricks.ToString();
        _textEnemyMagic.text = enemyStats.magic.ToString();
        _textEnemyGems.text = enemyStats.gems.ToString();
        _textEnemyDungeons.text = enemyStats.dungeons.ToString();
        _textEnemyRecruits.text = enemyStats.recruits.ToString();

        UpdateTowerPosition(int.Parse(_textPlayerTower.text), _playerTower);
        UpdateWallPosition(int.Parse(_textPlayerWall.text), _playerWall);

        UpdateTowerPosition(int.Parse(_textEnemyTower.text), _enemyTower);
        UpdateWallPosition(int.Parse(_textEnemyWall.text), _enemyWall);
    }

    public Dictionary<string, TextMeshProUGUI> GetUIElements(Player targetPlayer)
    {
        var uiElements = new Dictionary<string, TextMeshProUGUI>
        {
            {Constants.Generators.Quarries, targetPlayer == Player.Player ? _textPlayerQuarries : _textEnemyQuarries},
            {Constants.Resources.Bricks, targetPlayer == Player.Player ? _textPlayerBricks : _textEnemyBricks},
            {Constants.Generators.Magic, targetPlayer == Player.Player ? _textPlayerMagic : _textEnemyMagic},
            {Constants.Resources.Gems, targetPlayer == Player.Player ? _textPlayerGems : _textEnemyGems},
            {Constants.Generators.Dungeons, targetPlayer == Player.Player ? _textPlayerDungeons : _textEnemyDungeons},
            {Constants.Resources.Recruits, targetPlayer == Player.Player ? _textPlayerRecruits : _textEnemyRecruits},
            {Constants.Structures.Wall, targetPlayer == Player.Player ? _textPlayerWall : _textEnemyWall},
            {Constants.Structures.Tower, targetPlayer == Player.Player ? _textPlayerTower : _textEnemyTower},
        };

        return uiElements;
    }

    void UpdateTowerPosition(int towerValue, RectTransform towerTransform)
    {
        float towerPositionY = (towerValue / (float)Constants.GameplayLimits.MaxTowerRenderedUIValue) * maxTowerHeight;
        towerTransform.anchoredPosition = new Vector2(towerTransform.anchoredPosition.x, towerPositionY);
    }

    void UpdateWallPosition(int wallValue, RectTransform wallTransform)
    {
        float wallPositionY = (Mathf.Min(wallValue, 100) / (float)Constants.GameplayLimits.MaxWallRenderedUIValue) * maxWallHeight;
        wallTransform.anchoredPosition = new Vector2(wallTransform.anchoredPosition.x, wallPositionY);
    }
}
