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
        if (Object.HasStateAuthority)
        {
            ownership.owner.popCount++;
        }

        if (Object.HasInputAuthority)
        {
            updateUI();
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
        if (popCount != null && ownership?.owner != null)
        {
            popCount.text = ownership.owner.popCount.ToString();
        }
    }
}
