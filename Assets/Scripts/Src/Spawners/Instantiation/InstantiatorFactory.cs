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
                return null;
            }
            else
            {
                return new LocalInstantiator();
            }
        }
    }
}
