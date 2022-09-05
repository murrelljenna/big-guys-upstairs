using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanel : MonoBehaviour
{

	game.assets.Player player;
	Text foodCount;
    Text woodCount;
    // Start is called before the first frame update
    void Awake()
    {
        foodCount = this.transform.Find("Food").Find("foodCount").GetComponent<Text>();
        woodCount = this.transform.Find("Wood").Find("woodCount").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        foodCount.text = player.food.ToString() + " (+" + player.foodIt.ToString() + ")";
        woodCount.text = player.wood.ToString() + " (+" + player.woodIt.ToString() + ")";
    }

    public void assignPlayer(game.assets.Player player) {
    	this.player = player;
    }
}
