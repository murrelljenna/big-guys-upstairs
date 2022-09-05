using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils.MagicWords;

public static class UI
{
    public static GameObject BuildingMenu()
    {
        return GameObject.Find(GameObjectNames.BuildingMenu);
    }
}
