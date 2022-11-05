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
    [Networked]
    public ResourceSet resources { get; set; }
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

    private ResourceSet startingResources = new ResourceSet(200, 200);

    public void Init() 
    {
        var spawnPoints = PlayerSpawner.GetAll();
        playerSlots = new PlayerSlot[spawnPoints.Length];

        for (int i = 0; i < playerSlots.Length; i++)
        {
            PlayerColour colour = PlayerColourManager.PickFirstAvailableColour();
            playerSlots[i] = new PlayerSlot(spawnPoints[i].location(), colour);
        }
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
}
