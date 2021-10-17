using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets.ai
{
    public interface IHealth
    {
        void lowerHP(int amt);
        void raiseHP(int amt);
        bool maxed();
        bool zero();
    }

    public class Health : MonoBehaviour, IHealth
    {
        [Tooltip("Starting health")]
        public int HP;

        [Tooltip("Maximum health reachable")]
        public int maxHP;

        [Tooltip("Invoked when HP reaches zero")]
        public UnityEvent<Health> onZeroHP;

        [Tooltip("Invoked when HP reaches max")]
        public UnityEvent onMaxHP;

        [Tooltip("Invoked when HP damaged")]
        public UnityEvent<float, float> onLowerHP;

        [Tooltip("Invoked when HP raised")]
        public UnityEvent<float, float> onRaiseHP;

        public void lowerHP(int amt)
        {
            HP = HP - amt;
            onLowerHP.Invoke(HP, maxHP);

            if (zero())
            {
                HP = 0;
                onZeroHP.Invoke(this);
            }
        }

        public void raiseHP(int amt)
        {
            HP = HP + amt;

            onRaiseHP.Invoke(HP, maxHP);

            if (maxed())
            {
                HP = maxHP;
                onMaxHP.Invoke();
            }
        }

        public bool maxed()
        {
            return (HP >= maxHP);
        }

        public bool zero()
        {
            return (HP <= 0);
        }
    }
}
