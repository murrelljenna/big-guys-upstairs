using game.assets;
using game.assets.player;
using UnityEngine;

[RequireComponent(typeof(Ownership))]
public class BarbarianOwnership : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetAsPlayer(LocalGameManager.Get().barbarianPlayer);
    }
}
