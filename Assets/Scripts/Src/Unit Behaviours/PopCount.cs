using game.assets.player;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ownership))]
public class PopCount : MonoBehaviour
{
    private Ownership ownership;
    Text popCount;
    void Start()
    {
        ownership = GetComponent<Ownership>();
        ownership.owner.popCount++;
        popCount = GameObject.Find("Pop_Count").GetComponent<Text>();

        updateUI();
    }

    private void OnDestroy()
    {
        ownership.owner.popCount--;

        updateUI();
    }

    private void updateUI()
    {
        if (this.IsMine())
        {
            popCount.text = ownership.owner.popCount.ToString();
        }
    }
}
