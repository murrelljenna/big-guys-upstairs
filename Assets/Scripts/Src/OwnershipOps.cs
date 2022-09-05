using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OwnershipOps
{
    public static bool IsMine(this MonoBehaviour behaviour)
    {
        return behaviour.gameObject.IsMine();
    }

    public static bool IsMine(this GameObject gameObject)
    {
        Ownership ownership = gameObject.GetComponent<Ownership>();
        if (ownership != null && ownership.owned && ownership.isOwnedBy(LocalPlayer.get()))
        {
            return true;
        }

        return false;
    }

    public static bool IsEnemy(this MonoBehaviour behaviour)
    {
        Ownership ownership = behaviour.GetComponent<Ownership>();
        if (ownership != null && ownership.owned && !ownership.isOwnedBy(LocalPlayer.get()))
        {
            return true;
        }

        return false;
    }

    public static void SetAsMine(this MonoBehaviour behaviour)
    {
        behaviour.gameObject.SetAsMine();
    }

    public static void SetAsMine(this GameObject gameObj)
    {
        gameObj.SetAsPlayer(LocalPlayer.get());
    }
    public static void SetAsPlayer(this GameObject gameObj, game.assets.player.Player player)
    {
        Ownership ownership = gameObj.GetComponent<Ownership>();
        if (ownership == null)
        {
            ownership = gameObj.AddComponent(typeof(Ownership)) as Ownership;
        }
        ownership.setOwner(player);
    }
}
