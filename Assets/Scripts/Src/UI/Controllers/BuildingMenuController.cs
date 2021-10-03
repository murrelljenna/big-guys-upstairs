using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils.MagicWords;

public class BuildingMenuController : MonoBehaviour
{
    private GameObject buildingMenu;

    void Start()
    {
        buildingMenu = GameObject.Find(GameObjectNames.BuildingMenu);
    }

    void OnDisable()
    {
        buildingMenu.SetActive(false);
    }

    void OnEnable()
    {
        if (buildingMenu != null)
        { // Resulting in exception at beginning of game unless inside this statement
            buildingMenu.SetActive(true);
        }
    }

    public void triggerButtonPush(int num)
    {
        
    }
}
