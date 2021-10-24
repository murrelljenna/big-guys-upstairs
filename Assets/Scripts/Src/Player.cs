using static game.assets.utilities.GameUtils;

namespace game.assets.player
{
    public class Player
    {
        public PlayerColour colour { get; set; }
        public int popCount { get; set; }

        public Player()
        {
            colour = PlayerColours.Blue;
            popCount = 0;
        }
    }
}
