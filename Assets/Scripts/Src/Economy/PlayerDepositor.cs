using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets;
using game.assets.utilities.resources;

namespace game.assets {
    public interface IPlayerTransaction
    {
        bool canAfford(ResourceSet resourceSet);
        void giveResources(ResourceSet resourceSet);
        void takeResources(ResourceSet resourceSet);
        ResourceSet resources();
    }

    public class PlayerDepositor : Depositor, IPlayerTransaction
    {
        public void giveResources(ResourceSet resourceSet) { this.store = this.store + resourceSet; }
        public void takeResources(ResourceSet resourceSet) { this.store = this.store - resourceSet; }
        public bool canAfford(ResourceSet resourceSet) { return (this.store >= resourceSet); }
        public ResourceSet resources() { return this.store; }
    }
}
