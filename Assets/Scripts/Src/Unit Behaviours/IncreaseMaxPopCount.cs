using game.assets.player;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ownership))]
public class IncreaseMaxPopCount : MonoBehaviour
{
    [Tooltip("Increases max population by this once built")]
    public int increaseBy = 10;
    private Text maxText;
    void Start()
    {
        Ownership ownership = GetComponent<Ownership>();
        if (ownership.Object == null)
        {
            return;
        }
        ownership.owner.maxCount += increaseBy;
    }

    void OnDestroy()
    {
        Ownership ownership = GetComponent<Ownership>();
        if (ownership.Object == null)
        {
            return;
        }
        ownership.owner.maxCount -= increaseBy;
    }


}
