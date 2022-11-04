using Fusion;
using game.assets.player;
using game.assets.utilities.resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;

public class NetworkedGameManagerState : NetworkBehaviour
{

    private PlayerSlot[] playerSlots { get; set; }

    private struct PlayerSlot
    {
        public Player player;
        public Vector3 spawnPoint;
        public PlayerColour colour;
        public bool taken; // player == null

        public PlayerSlot(Vector3 spawnpoint, PlayerColour colour)
        {
            this.taken = false;
            this.player = null;
            this.spawnPoint = spawnpoint;
            this.colour = colour;
        }

        public void Clear()
        {
            player = null; // Reset Player state for next person to join
            taken = false;
        }

        public Player Take(PlayerRef networkPlayer, ResourceSet resources)
        {
            player = new Player(colour); // Reset Player state for next person to join
            this.player.networkPlayer = networkPlayer;
            this.player.resources = resources;
            this.player.spawnPoint = this.spawnPoint;
            taken = true;
            return this.player;
        }
    }

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

    private ResourceSet startingResources = new ResourceSet(200, 200);

    public void Init() 
    {
        var spawnPoints = PlayerSpawner.GetAll();
        playerSlots = new PlayerSlot[spawnPoints.Length];

        for (int i = 0; i < playerSlots.Length; i++)
        {
            PlayerColour colour = pickFirstAvailableColour();
            playerSlots[i] = new PlayerSlot(spawnPoints[i].location(), colour);
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

    public Player reserveNewPlayer(PlayerRef networkPlayer)
    {
        Player player = null;
        for (int i = 0; i < playerSlots.Length; i++)
        {
            var playerSlot = playerSlots[i];
            if (!playerSlot.taken)
            {
                player = playerSlot.Take(networkPlayer, startingResources);
            }
        }
        return player;
    }

    public void freePlayerSlot(PlayerRef networkPlayer)
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            var playerSlot = playerSlots[i];
            if (playerSlot.player.networkPlayer == networkPlayer)
            {
                playerSlot.Clear();
            }
        }
    }

    public PlayerColour getPlayerColour(int playerIndex)
    {
        return availableColours[playerIndex].colour;
    }
}
