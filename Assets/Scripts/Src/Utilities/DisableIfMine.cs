using Fusion;
using UnityEngine;

public class DisableIfMine : NetworkBehaviour
{
    public GameObject gameObjectToDisable;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            gameObjectToDisable.SetActive(false);
        }
    }
}
