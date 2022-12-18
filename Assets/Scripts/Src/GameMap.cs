using UnityEngine;
using System;

[Serializable]
public class GameMap
{
    public string sceneName;
    public int maxPlayers;
    public Texture icon;

    public GameMap(string sceneName, int maxPlayers, Texture icon)
    {
        this.sceneName = sceneName;
        this.maxPlayers = maxPlayers;
        this.icon = icon;
    }
}
