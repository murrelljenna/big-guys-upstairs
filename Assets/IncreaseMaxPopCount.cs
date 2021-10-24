using game.assets.player;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ownership))]
public class IncreaseMaxPopCount : MonoBehaviour
{
    [Tooltip("Increases max population by this once built")]
    public int increaseBy = 10;
    void Start()
    {
        Ownership ownership = GetComponent<Ownership>();
        ownership.owner.maxCount += increaseBy;
        Debug.Log(ownership.owner.maxCount);
        updateUI(ownership.owner.maxCount);
    }

    void OnDestroy()
    {
        Ownership ownership = GetComponent<Ownership>();
        ownership.owner.maxCount -= increaseBy;
        updateUI(ownership.owner.maxCount);
    }

    private void updateUI(int max)
    {
        GameObject.Find("Pop_Max").GetComponent<Text>().text = max.ToString();
    }
}
