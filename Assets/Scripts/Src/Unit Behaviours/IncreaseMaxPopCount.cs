using game.assets.player;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ownership))]
public class IncreaseMaxPopCount : MonoBehaviour
{
    [Tooltip("Increases max population by this once built")]
    public int increaseBy = 10;
    private Text maxText;
    void Start()
    {
        maxText = GameObject.Find("Pop_Max").GetComponent<Text>();
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
        if (maxText != null)
        {
            maxText.text = max.ToString();
        }
    }
}
