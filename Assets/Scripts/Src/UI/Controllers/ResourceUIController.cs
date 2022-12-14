using game.assets;
using game.assets.player;
using game.assets.utilities.resources;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIController : MonoBehaviour
{
    [Tooltip("Player resources to represent")]
    public Player player;

    private Text foodCount;
    private Text woodCount;
    private Text goldCount;
    private Text stoneCount;
    private Text ironCount;

    void Awake()
    {
        foodCount = transform.Find("Food").Find("foodCount").GetComponent<Text>();
        woodCount = transform.Find("Wood").Find("woodCount").GetComponent<Text>();
        goldCount = transform.Find("Gold").Find("goldCount").GetComponent<Text>();
        stoneCount = transform.Find("Stone").Find("stoneCount").GetComponent<Text>();
        ironCount = transform.Find("Iron").Find("ironCount").GetComponent<Text>();
    }

    void Update()
    {
        if (player == null || player.Object == null)
        {
            return;
        }
        foodCount.text = player.resources.food.ToString();
        woodCount.text = player.resources.wood.ToString();
        goldCount.text = player.resources.gold.ToString();
        stoneCount.text = player.resources.stone.ToString();
        ironCount.text = player.resources.iron.ToString();
    }

    public static ResourceUIController Get()
    {
        return GameObject.Find("ResourcePanel").GetComponent<ResourceUIController>();
    }
}
