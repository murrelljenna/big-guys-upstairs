using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets
{
    public static class InstantiatorFactory
    {
        public static IInstantiator getInstantiator(bool networked = false) {
            if (networked)
            {
                return new NetworkedInstantiator();
            }
            else
            {
                return new LocalInstantiator();
            }
        }
    }
}
