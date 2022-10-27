using game.assets;
using game.assets.utilities.resources;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIController : MonoBehaviour
{
    [Tooltip("Player resources to represent")]
    public ResourceSet player;

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
        foodCount.text = player.food.ToString();
        woodCount.text = player.wood.ToString();
        goldCount.text = player.gold.ToString();
        stoneCount.text = player.stone.ToString();
        ironCount.text = player.iron.ToString();
    }
}
