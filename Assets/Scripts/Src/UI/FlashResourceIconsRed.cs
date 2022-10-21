using game.assets.utilities.resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashResourceIconsRed : MonoBehaviour
{
    public Image foodIcon;
    public Image woodIcon;

    private void flashFood()
    {
        
        foodIcon.color = Color.red;
        Invoke("resetFood", 0.6f);
    }

    private void flashWood()
    {
        woodIcon.color = Color.red;
        Invoke("resetWood", 0.6f);
    }

    public void flashRelevant(ResourceSet first, ResourceSet second)
    {
        if (woodIcon == null || foodIcon == null)
        {
            return;
        }

        if (first.wood < second.wood)
        {
            flashWood();
        }

        if (first.food < second.food)
        {
            flashFood();
        }
    }

    private void resetFood()
    {
        foodIcon.color = Color.white;
    }

    private void resetWood()
    {
        woodIcon.color = Color.white;
    }
}
