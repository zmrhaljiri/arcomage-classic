// This is an unrefactored code

// TODO
// - Make AI work correctly with condition cards
// - Split into multiple files
// - Polish logic
// - Polish conventions (casing, naming, order, ...)

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CardDataController;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }

    CardManager _cardManager;
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
        _cardManager = CardManager.Instance;
        _playAreaManager = PlayAreaManager.Instance;
    }

    public IEnumerator HandleEnemyTurn()
    {
        //print("----------");
        //print("NEW ROUND");
        //print("----------");        

        yield return new WaitForSeconds(Constants.Durations.EnemyDecision);

        List<CardInstanceController> enemyCards = _playAreaManager.GetEnemyCards();
        List<CardInstanceController> playableCards = enemyCards.Where(card => StatsManager.Instance.CanPlayCard(card.GetCardData())).ToList();
        List<CardInstanceController> discardableCards = enemyCards.Where(card => card.IsDiscardable()).ToList();

        CardInstanceController bestPlayableCard = null;
        int bestPlayableScore = int.MinValue;

        CardInstanceController lowestScoreCard = null;
        int lowestScore = int.MaxValue;

        var gameStats = new GameStats
        {
            PlayerTower = StatsManager.Instance.PlayerStats.tower,
            PlayerWall = StatsManager.Instance.PlayerStats.wall,
            PlayerQuarries = StatsManager.Instance.PlayerStats.quarries,
            PlayerMagic = StatsManager.Instance.PlayerStats.magic,
            PlayerDungeons = StatsManager.Instance.PlayerStats.dungeons,
            PlayerBricks = StatsManager.Instance.PlayerStats.bricks,
            PlayerGems = StatsManager.Instance.PlayerStats.gems,
            PlayerRecruits = StatsManager.Instance.PlayerStats.recruits,
            EnemyTower = StatsManager.Instance.EnemyStats.tower,
            EnemyWall = StatsManager.Instance.EnemyStats.wall,
            EnemyQuarries = StatsManager.Instance.EnemyStats.quarries,
            EnemyMagic = StatsManager.Instance.EnemyStats.magic,
            EnemyDungeons = StatsManager.Instance.EnemyStats.dungeons,
            EnemyBricks = StatsManager.Instance.EnemyStats.bricks,
            EnemyGems = StatsManager.Instance.EnemyStats.gems,
            EnemyRecruits = StatsManager.Instance.EnemyStats.recruits
        };

        var gameConditions = new GameConditions
        {
            ShouldFocusTowerWin = ShouldFocusTowerWin(gameStats.EnemyTower),
            ShouldFocusBricksWin = ShouldFocusBricksWin(gameStats.EnemyBricks, gameStats.EnemyQuarries),
            ShouldFocusGemsWin = ShouldFocusGemsWin(gameStats.EnemyGems, gameStats.EnemyMagic),
            ShouldFocusRecruitsWin = ShouldFocusRecruitsWin(gameStats.EnemyRecruits, gameStats.EnemyDungeons),
            ShouldAttack = ShouldAttack(gameStats.PlayerTower, gameStats.PlayerWall),
            ShouldAttackTowerOnly = ShouldAttackTowerOnly(gameStats.PlayerTower, gameStats.PlayerWall),
            ShouldPreventTowerLoss = ShouldPreventTowerLoss(gameStats.EnemyTower),
            ShouldSwitchWall = ShouldSwitchWall(gameStats.PlayerWall, gameStats.EnemyWall, gameStats.EnemyTower),
            ShouldGetWall = ShouldGetWall(gameStats.EnemyWall),
            ShouldGetTower = ShouldGetTower(gameStats.EnemyTower)
        };

        Stats EnemyMultipliers = new Stats();
        Stats PlayerMultipliers = new Stats();

        EnemyMultipliers.quarries = 100;
        EnemyMultipliers.magic = 100;
        EnemyMultipliers.dungeons = 100;
        EnemyMultipliers.bricks = 1;
        EnemyMultipliers.gems = 1;
        EnemyMultipliers.recruits = 1;
        EnemyMultipliers.wall = 2;
        EnemyMultipliers.tower = 3;
        EnemyMultipliers.damage = 3;

        PlayerMultipliers.quarries = 100;
        PlayerMultipliers.magic = 100;
        PlayerMultipliers.dungeons = 100;
        PlayerMultipliers.bricks = 1;
        PlayerMultipliers.gems = 1;
        PlayerMultipliers.recruits = 1;
        PlayerMultipliers.wall = 2;
        PlayerMultipliers.tower = 3;
        PlayerMultipliers.damage = 3;

        int bricksCostMultiplier = 0;
        int gemsCostMultiplier = 0;
        int recruitsCostMultiplier = 0;

        if (ShouldFocusTowerWin(gameStats.EnemyTower))
        {
            EnemyMultipliers.tower += 50;
            //print("ShouldFocusTowerWin");
        }

        if (ShouldFocusBricksWin(gameStats.EnemyBricks, gameStats.EnemyQuarries))
        {
            bricksCostMultiplier += 50;
            EnemyMultipliers.bricks += 50;
            //print("ShouldFocusBricksWin");
        }

        if (ShouldFocusGemsWin(gameStats.EnemyGems, gameStats.EnemyMagic))
        {
            gemsCostMultiplier += 50;
            EnemyMultipliers.gems += 50;
            //print("ShouldFocusGemsWin");
        }

        if (ShouldFocusRecruitsWin(gameStats.EnemyRecruits, gameStats.EnemyDungeons))
        {
            recruitsCostMultiplier += 50;
            EnemyMultipliers.recruits += 50;
            //print("ShouldFocusRecruitsWin");
        }

        if (ShouldAttack(gameStats.PlayerTower, gameStats.PlayerWall))
        {
            EnemyMultipliers.damage += 25;
            PlayerMultipliers.tower += 25;
            PlayerMultipliers.wall += 25;
            //print("ShouldAttack");
        }

        if (ShouldAttackTowerOnly(gameStats.PlayerTower, gameStats.PlayerWall))
        {
            PlayerMultipliers.tower += 50;
            //print("ShouldAttackTowerOnly");
        }

        if (ShouldPreventTowerLoss(gameStats.EnemyTower))
        {
            EnemyMultipliers.tower += 50;
            EnemyMultipliers.wall += 25;
            //print("ShouldPreventTowerLoss");
        }

        if (ShouldPreventPlayerTowerWin(gameStats.PlayerTower))
        {
            PlayerMultipliers.tower += 50;
            //print("ShouldPreventPlayerTowerWin");
        }

        if (ShouldGetWall(gameStats.EnemyWall))
        {
            EnemyMultipliers.wall += 25;
            //print("ShouldGetWall");
        }

        if (ShouldGetTower(gameStats.EnemyTower))
        {
            EnemyMultipliers.tower += 25;
            //print("ShouldGetTower");
        }

        //print("-----");
        //print("MULTIPLIERS");

        //print("EnemyMultipliers.quarries = " + EnemyMultipliers.quarries);
        //print("EnemyMultipliers.magic = " + EnemyMultipliers.magic);
        //print("EnemyMultipliers.dungeons = " + EnemyMultipliers.dungeons);
        //print("EnemyMultipliers.bricks = " + EnemyMultipliers.bricks);
        //print("EnemyMultipliers.gems = " + EnemyMultipliers.gems);
        //print("EnemyMultipliers.recruits = " + EnemyMultipliers.recruits);
        //print("EnemyMultipliers.wall = " + EnemyMultipliers.wall);
        //print("EnemyMultipliers.tower = " + EnemyMultipliers.tower);
        //print("EnemyMultipliers.damage = " + EnemyMultipliers.damage);
        //print("PlayerMultipliers.quarries = " + PlayerMultipliers.quarries);
        //print("PlayerMultipliers.magic = " + PlayerMultipliers.magic);
        //print("PlayerMultipliers.dungeons = " + PlayerMultipliers.dungeons);
        //print("PlayerMultipliers.bricks = " + PlayerMultipliers.bricks);
        //print("PlayerMultipliers.gems = " + PlayerMultipliers.gems);
        //print("PlayerMultipliers.recruits = " + PlayerMultipliers.recruits);
        //print("PlayerMultipliers.wall = " + PlayerMultipliers.wall);
        //print("PlayerMultipliers.tower = " + PlayerMultipliers.tower);
        //print("PlayerMultipliers.damage = " + PlayerMultipliers.damage);
        //print("bricksCostMultiplier = " + bricksCostMultiplier);
        //print("gemsCostMultiplier = " + gemsCostMultiplier);
        //print("recruitsCostMultiplier = " + recruitsCostMultiplier);


        //print("-----");
        //print("LISTING CARDS...");

        // Evaluate all cards
        foreach (var card in enemyCards)
        {
            CardDataController cardData = card.GetCardData();
            CardDataController cloneTest = JsonUtility.FromJson<CardDataController>(JsonUtility.ToJson(cardData));
            // Evaluate score for the card
            int cardScore = EvaluateCard(cloneTest, gameStats, gameConditions, EnemyMultipliers, PlayerMultipliers, bricksCostMultiplier, gemsCostMultiplier, recruitsCostMultiplier);

            // Track the best playable card            
            if (StatsManager.Instance.CanPlayCard(cardData) && cardScore > bestPlayableScore)
            {
                bestPlayableCard = card;
                bestPlayableScore = cardScore;
            }

            // Track the lowest-scoring card for discard logic

            if (cardScore < lowestScore)
            {
                lowestScore = cardScore;
                lowestScoreCard = card;
            }
        }

        if (_cardManager.PlayerMustDiscardCard)
        {
            CardManager.Instance.DiscardCard(lowestScoreCard.gameObject);
        }
        else
        {
            // Play the best playable card
            if (bestPlayableCard != null && bestPlayableScore > 0)
            {
                StartCoroutine(CardGameplayController.Instance.PlayCard(bestPlayableCard.gameObject));
            }
            else
            {
                // No playable cards, discard the lowest-scoring card
                if (lowestScoreCard != null)
                {
                    CardManager.Instance.DiscardCard(lowestScoreCard.gameObject);
                }
                else
                {
                    Debug.LogWarning("No discardable cards available.");
                }
            }
        }
    }

    int EvaluateCard(CardDataController cardData, GameStats gameStats, GameConditions gameConditions, Stats EnemyMultipliers, Stats PlayerMultipliers, int bricksCostMultiplier, int gemsCostMultiplier, int recruitsCostMultiplier)
    {
        int score = 0;

        ApplySpecialStatsChanges(cardData, gameStats);

        if (IsInstantWin(cardData, gameStats.PlayerTower, gameStats.PlayerWall, gameStats.EnemyTower, gameStats.EnemyBricks, gameStats.EnemyGems, gameStats.EnemyRecruits))
        {
            score += 9999;
            //print("IsInstantWin - Adding 9999");
        }

        if (IsInstantLoss(cardData, gameStats.EnemyTower, gameStats.EnemyWall, gameStats.PlayerTower, gameStats.PlayerBricks, gameStats.PlayerGems, gameStats.PlayerRecruits))
        {
            score -= 9999;
            //print("IsInstantLoss - Removing 9999");
        }

        if (HasAdditionalTurn(cardData))
        {
            score += 2000;
            //print("HasAdditionalTurn - Adding 2000");
        }

        if (ShouldSwitchWall(gameStats.PlayerWall, gameStats.EnemyWall, gameStats.EnemyTower))
        {
            if (cardData.id == 34)
            {
                score += 500;
                //print("ShouldSwitchWall - Adding 200");
            }
        }

        //print("EnemyMultipliers.quarries * " + (card.stats?.self?.quarries ?? 0));
        score += EnemyMultipliers.quarries * (cardData.stats?.self?.quarries ?? 0);
        //print("EnemyMultipliers.magic * " + (card.stats?.self?.magic ?? 0));
        score += EnemyMultipliers.magic * (cardData.stats?.self?.magic ?? 0);
        //print("EnemyMultipliers.dungeons * " + (card.stats?.self?.dungeons ?? 0));
        score += EnemyMultipliers.dungeons * (cardData.stats?.self?.dungeons ?? 0);
        //print("EnemyMultipliers.bricks * " + (card.stats?.self?.bricks ?? 0));
        score += EnemyMultipliers.bricks * (cardData.stats?.self?.bricks ?? 0);
        //print("EnemyMultipliers.gems * " + (card.stats?.self?.gems ?? 0));
        score += EnemyMultipliers.gems * (cardData.stats?.self?.gems ?? 0);
        //print("EnemyMultipliers.recruits * " + (card.stats?.self?.recruits ?? 0));
        score += EnemyMultipliers.recruits * (cardData.stats?.self?.recruits ?? 0);
        //print("EnemyMultipliers.wall * " + (card.stats?.self?.wall ?? 0));
        score += EnemyMultipliers.wall * (cardData.stats?.self?.wall ?? 0);
        //print("EnemyMultipliers.tower * " + (card.stats?.self?.tower ?? 0));
        score += EnemyMultipliers.tower * (cardData.stats?.self?.tower ?? 0);
        //print("EnemyMultipliers.damage * " + (card.stats?.self?.damage ?? 0));
        score -= EnemyMultipliers.damage * (cardData.stats?.self?.damage ?? 0);
        //print("PlayerMultipliers.quarries * " + (card.stats?.enemy?.quarries ?? 0));
        score -= PlayerMultipliers.quarries * (cardData.stats?.enemy?.quarries ?? 0);
        //print("PlayerMultipliers.magic * " + (card.stats?.enemy?.magic ?? 0));
        score -= PlayerMultipliers.magic * (cardData.stats?.enemy?.magic ?? 0);
        //print("PlayerMultipliers.dungeons * " + (card.stats?.enemy?.dungeons ?? 0));
        score -= PlayerMultipliers.dungeons * (cardData.stats?.enemy?.dungeons ?? 0);
        //print("PlayerMultipliers.bricks * " + (card.stats?.enemy?.bricks ?? 0));
        score -= PlayerMultipliers.bricks * (cardData.stats?.enemy?.bricks ?? 0);
        //print("PlayerMultipliers.gems * " + (card.stats?.enemy?.gems ?? 0));
        score -= PlayerMultipliers.gems * (cardData.stats?.enemy?.gems ?? 0);
        //print("PlayerMultipliers.recruits * " + (card.stats?.enemy?.recruits ?? 0));
        score -= PlayerMultipliers.recruits * (cardData.stats?.enemy?.recruits ?? 0);
        //print("PlayerMultipliers.wall * " + (card.stats?.enemy?.wall ?? 0));
        score -= PlayerMultipliers.wall * (cardData.stats?.enemy?.wall ?? 0);
        //print("PlayerMultipliers.tower * " + (card.stats?.enemy?.tower ?? 0));
        score -= PlayerMultipliers.tower * (cardData.stats?.enemy?.tower ?? 0);
        //print("PlayerMultipliers.damage * " + (card.stats?.enemy?.damage ?? 0));
        score += PlayerMultipliers.damage * (cardData.stats?.enemy?.damage ?? 0);

        score -= bricksCostMultiplier * cardData.cost;
        score -= gemsCostMultiplier * cardData.cost;
        score -= recruitsCostMultiplier * cardData.cost;

        return score;
    }

    bool IsInstantWin(CardDataController cardData, int playerTower, int playerWall, int enemyTower, int enemyBricks, int enemyGems, int enemyRecruits)
    {
        return
            (cardData.stats?.enemy?.damage ?? 0) >= (playerTower + Constants.VictoryConditions.DefeatTowerValue) + playerWall ||
            (-cardData.stats?.enemy?.tower ?? 0) >= (playerTower + Constants.VictoryConditions.DefeatTowerValue) ||
            (cardData.stats?.self?.tower ?? 0) >= Constants.VictoryConditions.VictoryTowerValue - enemyTower ||
            (cardData.stats?.self?.bricks ?? 0) >= Constants.VictoryConditions.VictoryResourceValue - enemyBricks ||
            (cardData.stats?.self?.gems ?? 0) >= Constants.VictoryConditions.VictoryResourceValue - enemyGems ||
            (cardData.stats?.self?.recruits ?? 0) >= Constants.VictoryConditions.VictoryResourceValue - enemyRecruits;
    }
    bool IsInstantLoss(CardDataController cardData, int enemyTower, int enemyWall, int playerTower, int playerBricks, int playerGems, int playerRecruits)
    {
        return
            (cardData.stats?.self?.damage ?? 0) >= (enemyTower + Constants.VictoryConditions.DefeatTowerValue) + enemyWall ||
            (-cardData.stats?.self?.tower ?? 0) >= (enemyTower + Constants.VictoryConditions.DefeatTowerValue) ||
            (cardData.stats?.enemy?.tower ?? 0) >= Constants.VictoryConditions.VictoryTowerValue - playerTower ||
            (cardData.stats?.enemy?.bricks ?? 0) >= Constants.VictoryConditions.VictoryResourceValue - playerBricks ||
            (cardData.stats?.enemy?.gems ?? 0) >= Constants.VictoryConditions.VictoryResourceValue - playerGems ||
            (cardData.stats?.enemy?.recruits ?? 0) >= Constants.VictoryConditions.VictoryResourceValue - playerRecruits;
    }

    bool HasAdditionalTurn(CardDataController cardData)
    {
        return cardData.additionalTurn;
    }

    bool ShouldFocusTowerWin(int enemyTower)
    {
        return enemyTower >= Constants.VictoryConditions.VictoryTowerValue - 20;
    }

    bool ShouldFocusResourceWin(int resource, int generator)
    {
        // Can win in six rounds or less?
        return (Constants.VictoryConditions.VictoryResourceValue - resource) / generator <= 6;
    }

    bool ShouldFocusBricksWin(int enemyBricks, int enemyQuarries)
    {
        return ShouldFocusResourceWin(enemyBricks, enemyQuarries);
    }

    bool ShouldFocusGemsWin(int enemyGems, int enemyMagic)
    {
        return ShouldFocusResourceWin(enemyGems, enemyMagic);
    }

    bool ShouldFocusRecruitsWin(int enemyRecruits, int enemyDungeons)
    {
        return ShouldFocusResourceWin(enemyRecruits, enemyDungeons);
    }
    bool ShouldAttack(int playerTower, int playerWall)
    {
        return playerTower + playerWall <= 20;
    }

    bool ShouldAttackTowerOnly(int playerTower, int playerWall)
    {
        return playerTower <= 12 && playerWall != 0;
    }

    bool ShouldPreventTowerLoss(int enemyTower)
    {
        return enemyTower <= Constants.VictoryConditions.DefeatTowerValue + 12;
    }

    bool ShouldPreventPlayerTowerWin(int playerTower)
    {
        return playerTower >= Constants.VictoryConditions.VictoryTowerValue - 20;
    }

    bool ShouldSwitchWall(int playerWall, int enemyWall, int enemyTower)
    {
        return
            playerWall - enemyWall >= 20 ||
            (enemyWall <= 5 && playerWall >= 20) ||
            (enemyWall <= 5 && enemyTower <= 10 && playerWall >= 10);
    }

    bool ShouldGetWall(int enemyWall)
    {
        return enemyWall <= 10;
    }

    bool ShouldGetTower(int enemyTower)
    {
        return enemyTower <= 20;
    }

    void ApplySpecialStatsChanges(CardDataController cardData, GameStats gameStats)
    {
        cardData.stats ??= new CardDataController.CardStats();
        cardData.stats.self ??= new CardDataController.Stats();
        cardData.stats.enemy ??= new CardDataController.Stats();

        switch (cardData.stringId)
        {
            case "mother_lode":
                cardData.stats.self.quarries += gameStats.EnemyQuarries < gameStats.PlayerQuarries ? 2 : 1;
                //print("cardData.stats.self.quarries = " + cardData.stats.self.quarries);
                break;
            case "copping_the_tech":
                if (gameStats.EnemyQuarries < gameStats.PlayerQuarries)
                {
                    cardData.stats.self.quarries += (gameStats.PlayerQuarries - gameStats.EnemyQuarries);
                }
                //print("gameStats.EnemyQuarries = " + gameStats.EnemyQuarries);
                //print("gameStats.PlayerQuarries = " + gameStats.PlayerQuarries);
                //print("cardData.stats.self.quarries = " + cardData.stats.self.quarries);
                break;
            case "foundations":
                cardData.stats.self.wall += gameStats.EnemyWall == 0 ? 6 : 3;
                //print("cardData.stats.self.wall = " + cardData.stats.self.wall);
                break;
            case "flood_water":
                // TODO - check if reference is working and changes are applying really to cardData, not just targetStats
                Stats targetStats = gameStats.EnemyWall >= gameStats.PlayerWall ? cardData.stats.enemy : cardData.stats.self;
                targetStats.dungeons -= 1;
                targetStats.tower -= 2;
                //print("targetStats.dungeons = " + targetStats.dungeons);
                //print("targetStats.tower = " + targetStats.tower);
                break;
            case "barracks":
                if (gameStats.EnemyDungeons < gameStats.PlayerDungeons)
                {
                    cardData.stats.self.dungeons += 1;
                }
                //print("cardData.stats.self.dungeons = " + cardData.stats.self.dungeons);
                break;
            case "shift":
                // TODO - decide if it makes sense here or keep special condition of "if id == 34 then..."
                break;
            case "parity":
                // TODO - this is wrong probably (enemy had I think 3 and me 2)
                int highestMagic = Mathf.Max(gameStats.EnemyMagic, gameStats.PlayerMagic);
                // 1 3
                cardData.stats.self.magic += (highestMagic - gameStats.EnemyMagic);
                cardData.stats.enemy.magic += (highestMagic - gameStats.PlayerMagic);
                //print("cardData.stats.self.magic = " + cardData.stats.self.magic);
                //print("cardData.stats.enemy.magic = " + cardData.stats.enemy.magic);
                break;
            case "bag_of_baubles":
                cardData.stats.self.tower += (gameStats.EnemyTower < gameStats.PlayerTower ? 2 : 1);
                //print("cardData.stats.self.tower = " + cardData.stats.self.tower);
                break;
            case "lightning_shard":
                if (gameStats.EnemyTower > gameStats.PlayerWall)
                {
                    cardData.stats.enemy.tower -= 8;
                }
                else
                {
                    cardData.stats.enemy.damage += 8;
                }
                //print("cardData.stats.enemy.tower = " + cardData.stats.enemy.tower);
                //print("cardData.stats.enemy.damage = " + cardData.stats.enemy.damage);
                break;
            case "spizzer":
                cardData.stats.enemy.damage += gameStats.PlayerWall == 0 ? 10 : 6;
                //print("cardData.stats.enemy.damage = " + cardData.stats.enemy.damage);
                break;
            case "corrosion_cloud":
                cardData.stats.enemy.damage += gameStats.PlayerWall > 0 ? 10 : 7;
                //print("cardData.stats.enemy.damage = " + cardData.stats.enemy.damage);
                break;
            case "unicorn":
                cardData.stats.enemy.damage += gameStats.EnemyMagic > gameStats.PlayerMagic ? 12 : 7;
                //print("cardData.stats.enemy.damage = " + cardData.stats.enemy.damage);
                break;
            case "elven_archers":
                if (gameStats.EnemyWall > gameStats.PlayerWall)
                {
                    cardData.stats.enemy.tower -= 6;
                }
                else
                {
                    cardData.stats.enemy.damage += 6;
                }
                //print("cardData.stats.enemy.tower = " + cardData.stats.enemy.tower);
                //print("cardData.stats.enemy.damage = " + cardData.stats.enemy.damage);
                break;
            case "spearman":
                cardData.stats.enemy.damage += gameStats.EnemyWall > gameStats.PlayerWall ? 3 : 2;
                //print("cardData.stats.enemy.damage = " + cardData.stats.enemy.damage);
                break;
            default:
                break;
        }
    }
}

class GameStats
{
    public int PlayerTower { get; set; }
    public int PlayerWall { get; set; }
    public int PlayerQuarries { get; set; }
    public int PlayerMagic { get; set; }
    public int PlayerDungeons { get; set; }
    public int PlayerBricks { get; set; }
    public int PlayerGems { get; set; }
    public int PlayerRecruits { get; set; }
    public int EnemyTower { get; set; }
    public int EnemyWall { get; set; }
    public int EnemyQuarries { get; set; }
    public int EnemyMagic { get; set; }
    public int EnemyDungeons { get; set; }
    public int EnemyBricks { get; set; }
    public int EnemyGems { get; set; }
    public int EnemyRecruits { get; set; }
}

class GameConditions
{
    public bool ShouldFocusTowerWin { get; set; }
    public bool ShouldFocusBricksWin { get; set; }
    public bool ShouldFocusGemsWin { get; set; }
    public bool ShouldFocusRecruitsWin { get; set; }
    public bool ShouldAttack { get; set; }
    public bool ShouldAttackTowerOnly { get; set; }
    public bool ShouldPreventTowerLoss { get; set; }
    public bool ShouldSwitchWall { get; set; }
    public bool ShouldGetWall { get; set; }
    public bool ShouldGetTower { get; set; }
}