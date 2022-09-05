using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace game.assets.utilities
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager current;

        void Awake()
        {
            current = this;
        }

        public event Action onExampleAction;
        public void ExampleAction()
        {
            if (onExampleAction != null)
            {
                onExampleAction();
            }
        }

    }
}
