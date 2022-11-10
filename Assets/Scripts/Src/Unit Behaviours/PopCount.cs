using Fusion;
using game.assets.player;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ownership))]
public class PopCount : NetworkBehaviour
{
    private Ownership ownership;
    Text popCount;
    public override void Spawned()
    {
        ownership = GetComponent<Ownership>();
        popCount = GameObject.Find("Pop_Count").GetComponent<Text>();
    }

    public void Start()
    {
        if (Object.HasInputAuthority)
        {
            updateUI();
        }

        if (Object.HasStateAuthority)
        {
            ownership.owner.popCount++;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Object.HasStateAuthority && ownership?.owner != null)
        {
            ownership.owner.popCount--;
        }

        if (Object.HasInputAuthority)
        {
            updateUI();
        }
    }

    private void updateUI()
    {
        Debug.Log("Ownership is " + (ownership.owner == null).ToString());
        if (Object.HasInputAuthority && popCount != null)
        {
            popCount.text = ownership.owner.popCount.ToString();
        }
    }
}
