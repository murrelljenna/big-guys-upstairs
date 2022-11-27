using Fusion;
using game.assets.player;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ownership))]
public class PopCount : NetworkBehaviour
{
    private Ownership ownership;
    public override void Spawned()
    {
        ownership = GetComponent<Ownership>();
    }

    public void Start()
    {
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
    }
}
