using game.assets;
using game.assets.player;
using UnityEngine;

[RequireComponent(typeof(Ownership))]
public class BarbarianOwnership : MonoBehaviour
{
    [Tooltip("Replaces this GameObject once strength threshold reached")]
    public GameObject fortifiedVariant;
    public bool fortified = false;
    private BarbarianPlayer player;

    void Awake()
    {
        player = LocalGameManager.Get().barbarianPlayer;
        gameObject.SetAsPlayer(player);
    }

    public void fortify()
    {
        if (fortified)
        {
            return;
        }

        if (fortifiedVariant != null) {
            Vector3 spawnPosition = transform.parent.gameObject.transform.position;
            Quaternion spawnRotation = transform.parent.gameObject.transform.rotation;
            Destroy(transform?.parent?.gameObject);
            InstantiatorFactory.getInstantiator(false).InstantiateAsPlayer(fortifiedVariant, spawnPosition, spawnRotation, player);
            fortified = true;
        } else {
            Debug.LogError("BarbarianOwnership being asked to fortify() without an fortifiedGameObject");
        }
    }
}
