using game.assets;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIController : MonoBehaviour
{
    [Tooltip("Player resources to represent")]
    public PlayerDepositor player;

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
        foodCount.text = player.store.food.ToString();
        woodCount.text = player.store.wood.ToString();
        goldCount.text = player.store.gold.ToString();
        stoneCount.text = player.store.stone.ToString();
        ironCount.text = player.store.iron.ToString();
    }
}
