using static game.assets.utilities.GameUtils;

namespace game.assets.player
{
    [System.Serializable]
    public class Player
    {
        public PlayerColour colour { get; set; }
        public int popCount { get; set; }
        public int maxCount { get; set; }

        public Player()
        {
            colour = PlayerColours.Blue;
            popCount = 0;
            maxCount = 10;
        }

        public bool maxPop()
        {
            return popCount >= maxCount;
        }
    }
}
