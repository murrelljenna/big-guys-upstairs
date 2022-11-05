using System;
using static game.assets.utilities.GameUtils;

public static class PlayerColourManager
{
    private struct ColourAvailability
    {
        public PlayerColour colour;
        public bool available;

        public ColourAvailability(PlayerColour colour, bool available = false)
        {
            this.colour = colour;
            this.available = true;
        }
    }

    private static ColourAvailability[] availableColours = new ColourAvailability[]{
            new ColourAvailability(PlayerColours.Blue),
            new ColourAvailability(PlayerColours.Red),
            new ColourAvailability(PlayerColours.Green),
            new ColourAvailability(PlayerColours.Pink),
            new ColourAvailability(PlayerColours.White),
            new ColourAvailability(PlayerColours.Yellow),
            new ColourAvailability(PlayerColours.Black)
        };

    public static PlayerColour ColourAtIndex(int index)
    {
        return availableColours[index].colour;
    } 

    public static int IndexOfColour(PlayerColour colour)
    {
        for (int i = 0; i < availableColours.Length; i++)
        {
            if (availableColours[i].colour.name == colour.name)
            {
                return i;
            }
        }

        return -1;
    }

    public static PlayerColour PickFirstAvailableColour()
    {
        for (int i = 0; i < availableColours.Length; i++)
        {
            if (availableColours[i].available)
            {
                availableColours[i].available = false;
                return availableColours[i].colour;
            }
        }

        throw new ArgumentException("No available colours found", nameof(availableColours));
    }
}
