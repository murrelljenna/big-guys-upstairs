using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IsAUtils
{
    public static bool IsATown(GameObject go)
    {
        return go.GetComponent<Depositor>() != null && go.GetComponent<IncreaseMaxPopCount>() != null;
    }

    public static GameObject IfIsA(this GameObject go, Func<GameObject, bool> f, Func<GameObject, GameObject> f2) {
        if (f(go))
        {
            return f2(go);
        }

        return null;
    }
}
