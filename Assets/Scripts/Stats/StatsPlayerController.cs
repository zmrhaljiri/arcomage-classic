using UnityEngine;
using static CardDataController;

public class StatsPlayerController
{
    public Stats _stats { get; private set; }

    StatsEffectController _statsEffectController;
    PlayerManager _playerManager;

    public StatsPlayerController()
    {
        _statsEffectController = StatsEffectController.Instance;
        _playerManager = PlayerManager.Instance;
    }

    public void SetInitialStats()
    {
        _stats = new Stats(Constants.InitialPlayerStats);
    }

    public void ApplyCardStats(GameObject card, StatsPlayerController enemyStats)
    {
        if (card == null) return;

        CardDataController cardData = card.GetComponent<CardInstanceController>().GetCardData();

        if (cardData.stats != null)
        {
            Stats oldStats = new Stats(_stats);
            Stats oldEnemyStats = new Stats(enemyStats._stats);            

            if (cardData.stats.self != null)
            {
                ApplyStatsChanges(cardData.stats.self);
            }

            if (cardData.stats.enemy != null)
            {
                enemyStats.ApplyStatsChanges(cardData.stats.enemy);
            }            

            ApplySpecialStatsChanges(cardData, enemyStats);

            _statsEffectController.TriggerStatEffects(oldStats, _stats, GetActivePlayer());
            _statsEffectController.TriggerStatEffects(oldEnemyStats, enemyStats._stats, GetActiveEnemy());

            CardUIController.UpdateTransparencyInAllCards();
        }
    }

    public void ApplyDamage(int damage)
    {
        int damageToWall = Mathf.Min(_stats.wall, damage);
        _stats.wall -= damageToWall;

        int excessDamage = damage - damageToWall;
        if (excessDamage > 0)
        {
            _stats.tower = Mathf.Max(Constants.GameplayLimits.MinTowerValue, _stats.tower - excessDamage);
        }

        AudioManager.Instance.PlayGameplaySound("damage");
    }

    public void GenerateResources()
    {
        _stats.bricks += _stats.quarries;
        _stats.gems += _stats.magic;
        _stats.recruits += _stats.dungeons;
    }

    public bool CanPlayCard(CardDataController card)
    {
        if (card.type == Constants.Generators.Quarries)
        {
            return _stats.bricks >= card.cost;
        }
        if (card.type == Constants.Generators.Magic)
        {
            return _stats.gems >= card.cost;
        }
        if (card.type == Constants.Generators.Dungeons)
        {
            return _stats.recruits >= card.cost;
        }
        return false;
    }

    public void DeductCardCost(CardDataController cardData)
    {
        if (cardData.type == Constants.Generators.Quarries)
        {
            _stats.bricks = Mathf.Max(0, _stats.bricks - cardData.cost);
        }
        else if (cardData.type == Constants.Generators.Magic)
        {
            _stats.gems = Mathf.Max(0, _stats.gems - cardData.cost);
        }
        else if (cardData.type == Constants.Generators.Dungeons)
        {
            _stats.recruits = Mathf.Max(0, _stats.recruits - cardData.cost);
        }
    }

    void ApplyStatsChanges(Stats changes)
    {
        if (changes.damage > 0)
        {
            ApplyDamage(changes.damage);
        }

        _stats.wall = Mathf.Clamp(_stats.wall + changes.wall, Constants.GameplayLimits.MinWallValue, Constants.GameplayLimits.MaxWallValue);
        _stats.tower = Mathf.Clamp(_stats.tower + changes.tower, Constants.GameplayLimits.MinTowerValue, Constants.GameplayLimits.MaxTowerValue);
        _stats.quarries = Mathf.Clamp(_stats.quarries + changes.quarries, Constants.GameplayLimits.MinGeneratorValue, Constants.GameplayLimits.MaxGeneratorValue);
        _stats.bricks = Mathf.Clamp(_stats.bricks + changes.bricks, Constants.GameplayLimits.MinResourceValue, Constants.GameplayLimits.MaxResourceValue);
        _stats.magic = Mathf.Clamp(_stats.magic + changes.magic, Constants.GameplayLimits.MinGeneratorValue, Constants.GameplayLimits.MaxGeneratorValue);
        _stats.gems = Mathf.Clamp(_stats.gems + changes.gems, Constants.GameplayLimits.MinResourceValue, Constants.GameplayLimits.MaxResourceValue);
        _stats.dungeons = Mathf.Clamp(_stats.dungeons + changes.dungeons, Constants.GameplayLimits.MinGeneratorValue, Constants.GameplayLimits.MaxGeneratorValue);
        _stats.recruits = Mathf.Clamp(_stats.recruits + changes.recruits, Constants.GameplayLimits.MinResourceValue, Constants.GameplayLimits.MaxResourceValue);
    }

    void ApplySpecialStatsChanges(CardDataController cardData, StatsPlayerController enemyStats)
    {
        Stats oldPlayerStats = new Stats(_stats);
        Stats oldEnemyStats = new Stats(enemyStats._stats);

        switch (cardData.stringId)
        {
            case "mother_lode":
                _stats.quarries = Mathf.Min(_stats.quarries + (_stats.quarries < enemyStats._stats.quarries ? 2 : 1), Constants.GameplayLimits.MaxGeneratorValue);
                break;
            case "copping_the_tech":
                if (_stats.quarries < enemyStats._stats.quarries)
                {
                    _stats.quarries = enemyStats._stats.quarries;
                }
                break;
            case "foundations":
                _stats.wall = Mathf.Min(_stats.wall + (_stats.wall == 0 ? 6 : 3), Constants.GameplayLimits.MaxWallValue);
                break;
            case "flood_water":
                Stats targetStats = _stats.wall >= enemyStats._stats.wall ? enemyStats._stats : _stats;
                targetStats.dungeons = Mathf.Max(targetStats.dungeons - 1, Constants.GameplayLimits.MinGeneratorValue);
                targetStats.tower = Mathf.Max(targetStats.tower - 2, Constants.GameplayLimits.MinTowerValue);
                break;
            case "barracks":
                if (_stats.dungeons < enemyStats._stats.dungeons)
                {
                    _stats.dungeons = Mathf.Min(Constants.GameplayLimits.MaxGeneratorValue, _stats.dungeons + 1);
                }
                break;
            case "shift":
                int tempWall = _stats.wall;
                _stats.wall = enemyStats._stats.wall;
                enemyStats._stats.wall = tempWall;
                break;
            case "parity":
                int highestMagic = Mathf.Max(_stats.magic, enemyStats._stats.magic);
                _stats.magic = highestMagic;
                enemyStats._stats.magic = highestMagic;
                break;
            case "bag_of_baubles":
                _stats.tower = Mathf.Min(Constants.GameplayLimits.MaxTowerValue, _stats.tower + (_stats.tower < enemyStats._stats.tower ? 2 : 1));
                break;
            case "lightning_shard":
                if (_stats.tower > enemyStats._stats.wall)
                {
                    enemyStats._stats.tower = Mathf.Max(enemyStats._stats.tower - 8, Constants.GameplayLimits.MinTowerValue);
                }
                else
                {
                    enemyStats.ApplyDamage(8);
                }
                break;
            case "spizzer":
                enemyStats.ApplyDamage(enemyStats._stats.wall == 0 ? 10 : 6);
                break;
            case "corrosion_cloud":
                enemyStats.ApplyDamage(enemyStats._stats.wall > 0 ? 10 : 7);
                break;
            case "unicorn":
                enemyStats.ApplyDamage(_stats.magic > enemyStats._stats.magic ? 12 : 7);
                break;
            case "elven_archers":
                if (_stats.wall > enemyStats._stats.wall)
                {
                    enemyStats._stats.tower = Mathf.Max(enemyStats._stats.tower - 6, Constants.GameplayLimits.MinTowerValue);
                }
                else
                {
                    enemyStats.ApplyDamage(6);
                }
                break;
            case "spearman":
                enemyStats.ApplyDamage(_stats.wall > enemyStats._stats.wall ? 3 : 2);
                break;
            default:
                break;
        }

        _statsEffectController.TriggerStatEffects(oldPlayerStats, _stats, GetActivePlayer());
        _statsEffectController.TriggerStatEffects(oldEnemyStats, enemyStats._stats, GetActiveEnemy());
    }

    Player GetActivePlayer()
    {
        return _playerManager.IsPlayerTurn() ? Player.Player: Player.Enemy;
    }

    Player GetActiveEnemy()
    {
        return _playerManager.IsPlayerTurn() ? Player.Enemy : Player.Player;
    }
}
