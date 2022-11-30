using game.assets;
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

    public static bool IsFriendOf(this MonoBehaviour behaviour, MonoBehaviour otherBehaviour)
    {
        Player player = behaviour.GetComponent<Ownership>().owner;
        return otherBehaviour.BelongsTo(player);
    }

    public static bool IsFriendOf(this GameObject gameObject, GameObject otherGameObject)
    {
        Player player = gameObject.GetComponent<Ownership>().owner;
        return otherGameObject.BelongsTo(player);
    }

    public static bool BelongsTo(this MonoBehaviour behaviour, Player player)
    {
        return behaviour.gameObject.BelongsTo(player);
    }

    public static bool BelongsTo(this GameObject gameObject, Player player)
    {
        Ownership ownership = gameObject.GetComponent<Ownership>();
        if (ownership != null && ownership.owned && ownership.isOwnedBy(player))
        {
            return true;
        }

        return false;
    }

    public static bool IsEnemyOf(this MonoBehaviour behaviour, MonoBehaviour otherBehaviour)
    {
        return behaviour.gameObject.IsEnemyOf(otherBehaviour.gameObject);
    }

    public static bool IsEnemyOf(this GameObject gameObject, GameObject otherGameObject)
    {
        Ownership ownership = gameObject.GetComponent<Ownership>();
        Ownership otherOwnership = otherGameObject.GetComponent<Ownership>();
        if (
            ownership.Object != null &&
            (ownership != null && ownership.owned) &&
            (otherOwnership != null && otherOwnership.owned)
            && (ownership.owner != otherOwnership.owner)
            )
        {
            return true;
        }

        return false;
    }

    public static bool IsEnemyOf(this MonoBehaviour behaviour, Player player)
    {
        Ownership ownership = behaviour.GetComponent<Ownership>();
        if (
            (ownership != null && ownership.owned)
            && (ownership.owner != player)
            )
        {
            return true;
        }

        return false;
    }


    // Only useful for monobehaviours specifically belonging to local player.
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

    public static void SetAsPlayer(this MonoBehaviour behaviour, game.assets.player.Player player)
    {
        behaviour.gameObject.SetAsPlayer(player);
    }

    public static bool IsBarbarian(this MonoBehaviour behaviour)
    {
        return behaviour.BelongsTo(LocalGameManager.Get().barbarianPlayer);
    }

    public static GameObject SetAsPlayer(this GameObject gameObj, game.assets.player.Player player)
    {
        Ownership ownership = gameObj.GetComponent<Ownership>();
        if (ownership == null)
        {
            ownership = gameObj.AddComponent(typeof(Ownership)) as Ownership;
        }
        ownership.setOwnerRecursively(player);

        return gameObj;
    }
}
