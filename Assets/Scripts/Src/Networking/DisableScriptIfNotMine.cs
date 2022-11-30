using Fusion;
using UnityEngine;

public class DisableScriptIfNotMine : NetworkBehaviour
{
    public MonoBehaviour scriptToDisable;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
        {
            scriptToDisable.enabled = false;
        }
    }
}
