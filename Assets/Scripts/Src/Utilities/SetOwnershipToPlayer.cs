using game.assets;
using game.assets.player;
using UnityEngine;

[RequireComponent(typeof(Ownership))]
public class SetOwnershipToPlayer : MonoBehaviour
{
    void Start()
    {
        GetComponent<Ownership>().setOwner(LocalGameManager.Get().getLocalPlayer());
    }
}
