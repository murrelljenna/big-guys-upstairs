using Fusion;
using game.assets.player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;

public class NetworkedGameManagerState : NetworkBehaviour
{

    private Player[] players { get; set; }
    [Networked]
    private int playerCount { get; set; } = 0;

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

    private ColourAvailability[] availableColours = new ColourAvailability[]{
            new ColourAvailability(PlayerColours.Blue),
            new ColourAvailability(PlayerColours.Red),
            new ColourAvailability(PlayerColours.Green),
            new ColourAvailability(PlayerColours.Pink),
            new ColourAvailability(PlayerColours.White),
            new ColourAvailability(PlayerColours.Yellow),
            new ColourAvailability(PlayerColours.Black)
        };

    public void Init(Vector3[] spawnPoints) 
    {
        players = new Player[spawnPoints.Length];

        for (int i = 0; i < players.Length; i++)
        {
            PlayerColour colour = pickFirstAvailableColour();
            players[i] = new Player();
            players[i].colour = pickFirstAvailableColour();
        }
    }

    private PlayerColour pickFirstAvailableColour()
    {
        for (int i = 0; i < availableColours.Length; i++)
        {
            if (availableColours[i].available)
            {
                return availableColours[i].colour;
            }
        }

        throw new ArgumentException("No available colours found", nameof(availableColours));
    }

    public Player reserveNewPlayer(int playerId)
    {
        Debug.Log("AA - " + playerCount);
        playerCount++;
        players[playerCount - 1].PlayerId = playerId;
        return players[playerCount - 1];
    }

    public PlayerColour getPlayerColour(int playerIndex)
    {
        return availableColours[playerIndex].colour;
    }
}
