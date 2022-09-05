using static game.assets.utilities.GameUtils;

namespace game.assets.player
{
    public class Player
    {
        public PlayerColour colour { get; set; }

        public Player()
        {
            colour = PlayerColours.Blue;
        }
    }
}
