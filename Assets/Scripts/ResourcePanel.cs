using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanel : MonoBehaviour
{

	game.assets.Player player;
	Text foodCount;
    Text woodCount;
    Text goldCount;
    Text stoneCount;
    Text ironCount;
    // Start is called before the first frame update
    void Awake()
    {
        foodCount = this.transform.Find("Food").Find("foodCount").GetComponent<Text>();
        woodCount = this.transform.Find("Wood").Find("woodCount").GetComponent<Text>();
        goldCount = this.transform.Find("Gold").Find("goldCount").GetComponent<Text>();
        stoneCount = this.transform.Find("Stone").Find("stoneCount").GetComponent<Text>();
        ironCount = this.transform.Find("Iron").Find("ironCount").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            foodCount.text = player.resources.food.ToString();
            woodCount.text = player.resources.wood.ToString();
            goldCount.text = player.resources.gold.ToString();
            stoneCount.text = player.resources.stone.ToString();
            ironCount.text = player.resources.iron.ToString();
        }
    }

    public void assignPlayer(game.assets.Player player) {
    	this.player = player;
    }
}
