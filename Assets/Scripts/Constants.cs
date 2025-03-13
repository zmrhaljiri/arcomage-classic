using System.Collections.Generic;

public static class Constants
{
    public static class Durations
    {
        public const float RenderDelay = 0.01f; // = 0.01f
        public const float PlayCard = 0.15f; // = 0.15f
        public const float DrawCard = 0.5f; // = 0.5f
        public const float ShuffleCard = 0.25f; // = 0.25f        
        public const float AfterApplyStats = 0.6f; // = 0.6f
        public const float EnemyDecision = 1; // = 1
        public const float CardImageFadeOut = 0.3f; // 0.3f
        public const float CardTextFadeOut = 0.7f; // 0.7f        
        public const float IntroOverlayFadeDelay = 1f; // 1f
        public const float IntroOverlayFadeout = 2f; // 2f
        public const float MenuFadeInDelay = 5f; // 5f
        public const float MenuFadeIn = 1f; // 1f
    }

    public static class GameplayLimits
    {
        public const int CardsInHand = 6; // 6
        public const int MaxTowerValue = 75; // 75
        public const int MaxWallValue = 999; // 999
        public const int MaxGeneratorValue = 999; // 999
        public const int MaxResourceValue = 999; // 999
        public const int MaxTowerRenderedUIValue = 100; // 100
        public const int MaxWallRenderedUIValue = 100; // 100
        public const int MinTowerValue = 0; // 0
        public const int MinWallValue = 0; // 0
        public const int MinGeneratorValue = 1; // 1
        public const int MinResourceValue = 0; // 0
    }

    public static class VictoryConditions
    {
        public const int DefeatTowerValue = GameplayLimits.MinTowerValue; // GameplayLimits.MinTowerValue
        public const int VictoryTowerValue = GameplayLimits.MaxTowerValue; // GameplayLimits.MaxTowerValue
        public const int VictoryResourceValue = 200; // 200
    }

    public static readonly CardDataController.Stats InitialPlayerStats = new CardDataController.Stats
    {
        quarries = 3, // 3
        bricks = 5, // 5
        magic = 3, // 3
        gems = 5, // 5
        dungeons = 3, // 3
        recruits = 5, // 5
        wall = 8, // 8
        tower = 25 // 25
    };

    public static class RarityRanges
    {
        public static readonly Dictionary<int, string> RarityRange = new Dictionary<int, string>
        {
            { 60, "common" }, // { 60, "common" }
            { 80, "uncommon" }, // { 80, "uncommon" }
            { 95, "rare" }, // { 95, "rare" }
            { 100, "epic" } // { 100, "epic" }
        };
    }

    public static class Generators
    {
        public const string Quarries = "quarries";
        public const string Magic = "magic";
        public const string Dungeons = "dungeons";
    }

    public static class Resources
    {
        public const string Bricks = "bricks";
        public const string Gems = "gems";
        public const string Recruits = "recruits";
    }

    public static class Structures
    {
        public const string Wall = "wall";
        public const string Tower = "tower";
    } 

    public static class Images
    {
        public static class Background
        {
            public const string CardQuarries = "background-card-quarries";
            public const string CardMagic = "background-card-magic";
            public const string CardDungeons = "background-card-dungeons";
        }

        public static class Icon
        {
            public const string Quarries = "icon-quarries";
            public const string Magic = "icon-magic";
            public const string Dungeons = "icon-dungeons";
        }
    }

    public static class Sounds
    {
        public const string BookClose = "book-close";
        public const string BookOpen = "book-open";
        public const string Click = "click";
        public const string Damage = "damage";
        public const string GameOver = "game-over";
        public const string GameWin = "game-win";
        public const string GeneratorDown = "generator-down";
        public const string GeneratorUp = "generator-up";
        public const string MoveCard = "move-card";
        public const string ResourceDown = "resource-down";
        public const string ResourceUp = "resource-up";
        public const string TowerUp = "tower-up";
        public const string VictoryIsOurs = "victory-is-ours";
        public const string WallUp = "wall-up";
    }

    public static class Music
    {
        public const string Track01 = "track-01";
        public const string Track02 = "track-02";
        public const string Track03 = "track-03";
        public const string Track04 = "track-04";
        public const string Track05 = "track-05";
        public const string Track06 = "track-06";
        public const string Track07 = "track-07";
        public const string Track08 = "track-08";
        public const string Track09 = "track-09";
        public const string Track10 = "track-10";
        public const string Track11 = "track-11";
        public const string Track12 = "track-12";
        public const string Track13 = "track-13";
        public const string Track14 = "track-14";
    }

    public static class Players
    {
        public const string PlayerName = "Player";
        public const string EnemyName = "Enemy";
        public const string ActivePlayerMark = "***";
    }        

    public static class Messages
    {
        public const string HasWon = "has won!";
    }
}