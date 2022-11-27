using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets.ai
{
    public class Health : NetworkBehaviour
    {
        [Networked]
        public int HP { get; set; }

        [Tooltip("Maximum health reachable")]
        public int maxHP;

        [Tooltip("Invoked when HP reaches zero")]
        public UnityEvent<Health> onZeroHP;

        [Tooltip("Invoked when HP reaches max")]
        public UnityEvent onMaxHP;

        [Tooltip("Invoked when HP damaged")]
        public UnityEvent<float, float> onLowerHP;

        public UnityEvent<Attack> onAttacked = new UnityEvent<Attack>();

        [Tooltip("Invoked when HP raised")]
        public UnityEvent<float, float> onRaiseHP;

        [Tooltip("Invoked when HP is less than half")]
        public UnityEvent onUnderHalfHP;

        [Tooltip("Invoked when HP is over half")]
        public UnityEvent onOverHalfHP;

        public override void Spawned()
        {
            onRaiseHP.Invoke(HP, maxHP);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_FireLowerHPEvents()
        {
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

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_FireRaiseHPEvents()
        {
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

        public void lowerHP(int amt, Attack attacker = null)
        {
            if (!Object.HasStateAuthority)
            {
                return;
            }

            HP = HP - amt;

            if (attacker)
            {
                onAttacked.Invoke(attacker);
            }

            RPC_FireLowerHPEvents();
        }

        public void raiseHP(int amt)
        {
            HP = HP + amt;

            RPC_FireRaiseHPEvents();
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

        public void kill()
        {
            lowerHP(this.maxHP);
        }
    }
}
