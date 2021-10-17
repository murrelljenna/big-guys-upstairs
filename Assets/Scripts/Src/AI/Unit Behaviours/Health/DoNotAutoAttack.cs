using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.ai.units
{
    [RequireComponent(typeof(Health))]
    public class DoNotAutoAttack : MonoBehaviour
    {
        // Empty for now - attach to Health gameObjects and
        // Attack.cs will ignore attach Health script when
        // auto detecting units around it.
    }
}
