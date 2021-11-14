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

        [Tooltip("Invoked when HP is less than half")]
        public UnityEvent onUnderHalfHP;

        [Tooltip("Invoked when HP is over half")]
        public UnityEvent onOverHalfHP;

        void Start()
        {
            onRaiseHP.Invoke(HP, maxHP);
        }

        public void lowerHP(int amt)
        {
            HP = HP - amt;
            onLowerHP.Invoke(HP, maxHP);

            if (zero())
            {
                HP = 0;
                onZeroHP.Invoke(this);
            }

            if (underHalf())
            {
                onUnderHalfHP.Invoke();
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

            if (overHalf())
            {
                onOverHalfHP.Invoke();
            }
        }

        public bool maxed()
        {
            return (HP >= maxHP);
        }

        public bool underHalf()
        {
            return (HP <= (maxHP / 2));
        }

        public bool overHalf()
        {
            return (HP > (maxHP / 2));
        }

        public bool zero()
        {
            return (HP <= 0);
        }

        void OnDisable()
        {
            onZeroHP.Invoke(this);
        }
    }
}
