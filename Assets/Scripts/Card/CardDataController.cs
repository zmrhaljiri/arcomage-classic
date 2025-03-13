// Not following PascalCase naming of public stuff here due to the names mapping to card database
// Might refactor later

[System.Serializable]
public class CardDataController
{
    public int id;
    public string stringId;
    public string cardName;
    public string description;
    public string type;
    public int cost;
    public string rarity;
    public bool discardable;
    public bool extraAction;
    public bool additionalTurn;
    public bool discard;
    public CardStats stats;

    [System.Serializable]
    public class CardStats
    {
        public Stats self;
        public Stats enemy;
    }

    [System.Serializable]
    public class Stats
    {
        public int quarries;
        public int bricks;
        public int magic;
        public int gems;
        public int dungeons;
        public int recruits;
        public int wall;
        public int tower;
        public int damage;
        public Stats() { }

        public Stats(Stats statsToCopy)
        {
            quarries = statsToCopy.quarries;
            bricks = statsToCopy.bricks;
            magic = statsToCopy.magic;
            gems = statsToCopy.gems;
            dungeons = statsToCopy.dungeons;
            recruits = statsToCopy.recruits;
            wall = statsToCopy.wall;
            tower = statsToCopy.tower;
            damage = statsToCopy.damage;
        }
    }    
}