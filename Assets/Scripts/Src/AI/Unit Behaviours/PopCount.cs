using game.assets.player;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ownership))]
public class PopCount : MonoBehaviour
{
    private Ownership ownership;
    void Start()
    {
        ownership = GetComponent<Ownership>();
        ownership.owner.popCount++;

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
            GameObject.Find("Pop_Count").GetComponent<Text>().text = ownership.owner.popCount.ToString();
        }
    }
}
