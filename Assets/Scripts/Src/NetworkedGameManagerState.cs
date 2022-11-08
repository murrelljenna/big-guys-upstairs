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
    public struct PlayerSlot
    {
        public Vector3 spawnPoint;
        public PlayerColour colour;
        public PlayerRef? player;
        public bool taken;

        public PlayerSlot(Vector3 spawnpoint, PlayerColour colour)
        {
            this.taken = false;
            this.spawnPoint = spawnpoint;
            this.colour = colour;
            player = null;
        }

        public void Clear()
        {
            taken = false;
        }

        public PlayerSlot Take(PlayerRef player)
        {
            this.taken = true;
            this.player = player;
            return this;
        }
    }

    public void Init() 
    {
        var spawnPoints = PlayerSpawner.GetAll();
        playerSlots = new PlayerSlot[spawnPoints.Length];

        for (int i = 0; i < playerSlots.Length; i++)
        {
            PlayerColour colour = PlayerColourManager.PickFirstAvailableColour();
            Debug.Log("AC - Creating player slots. Colour is " + colour.name);
            playerSlots[i] = new PlayerSlot(spawnPoints[i].location(), colour);
        }
    }

    public PlayerSlot ReserveNewPlayer(PlayerRef networkPlayer)
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            Debug.Log("AC - Checking slot : " + i);
            if (!playerSlots[i].taken)
            {
                Debug.Log("AC - Taking slot : " + i);
                return playerSlots[i].Take(networkPlayer);
            }
        }

        throw new Exception("All player slots taken yet we're still trying to request a player");
    }

    public void freePlayerSlot(PlayerRef networkPlayer)
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i].player == networkPlayer)
            {
                playerSlots[i].Clear();
            }
        }
    }
}
