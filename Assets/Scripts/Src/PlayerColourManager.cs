using System;
using UnityEngine;
using static game.assets.utilities.GameUtils;

public class PlayerColourManager
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

    private static ColourAvailability[] Colours = new ColourAvailability[]{
            new ColourAvailability(PlayerColours.Blue),
            new ColourAvailability(PlayerColours.Red),
            new ColourAvailability(PlayerColours.Green),
            new ColourAvailability(PlayerColours.Pink),
            new ColourAvailability(PlayerColours.White),
            new ColourAvailability(PlayerColours.Yellow),
            new ColourAvailability(PlayerColours.Black)
        };

    private ColourAvailability[] AvailableColours;

    public PlayerColourManager()
    {
        AvailableColours = new ColourAvailability[Colours.Length];
        Array.Copy(Colours, AvailableColours, Colours.Length);
    }

    public static PlayerColour ColourAtIndex(int index)
    {
        return Colours[index].colour;
    } 

    public static int IndexOfColour(PlayerColour colour)
    {
        for (int i = 0; i < Colours.Length; i++)
        {
            if (Colours[i].colour.name == colour.name)
            {
                Debug.Log(" AC - Colour name: " + colour.name);
                Debug.Log(" AC - Available Colour name: " + Colours[i].colour.name);
                return i;
            }
        }

        return -1;
    }

    public PlayerColour PickFirstAvailableColour()
    {
        for (int i = 0; i < AvailableColours.Length; i++)
        {
            if (AvailableColours[i].available)
            {
                AvailableColours[i].available = false;
                return AvailableColours[i].colour;
            }
        }

        throw new ArgumentException("No available colours found", nameof(AvailableColours));
    }
}
