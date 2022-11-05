using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.utilities.resources {
    [System.Serializable]
    public struct ResourceSet {
        public int wood;
        public int food;
        public int gold;
        public int stone;
        public int iron;
        public int horse;

        public ResourceSet(int wood = 0, int food = 0, int gold = 0, int stone = 0, int iron = 0, int horse = 0) {
            this.wood = wood;
            this.food = food;
            this.gold = gold;
            this.stone = stone;
            this.iron = iron;
            this.horse = horse;
        }

        public bool anyValOver(int val = 0) {
            if (this.wood > val || this.food > val || this.gold > val || this.stone > val || this.iron > val || this.horse > val) {
                return true;
            }

            return false;
        }

        public static bool operator >=(ResourceSet left, ResourceSet right)
        {
            return (
                left.wood >= right.wood &&
                left.food >= right.food &&
                left.gold >= right.gold &&
                left.stone >= right.stone &&
                left.iron >= right.iron &&
                left.horse >= right.horse
            );
        }

        public static bool operator <=(ResourceSet left, ResourceSet right)
        {
            return (
                left.wood <= right.wood &&
                left.food <= right.food &&
                left.gold <= right.gold &&
                left.stone <= right.stone &&
                left.iron <= right.iron &&
                left.horse <= right.horse
            );
        }

        public static ResourceSet operator +(ResourceSet left, ResourceSet right) {
            ResourceSet ret = new ResourceSet();
            ret.wood = left.wood + right.wood;
            ret.food = left.food + right.food;
            ret.gold = left.gold + right.gold;
            ret.stone = left.stone + right.stone;
            ret.iron = left.iron + right.iron;
            ret.horse = left.horse + right.horse;
            return ret;
        }
        public static ResourceSet operator -(ResourceSet left, ResourceSet right) {
            return new ResourceSet(
                left.wood - right.wood,
                left.food - right.food,
                left.gold - right.gold,
                left.stone - right.stone,
                left.iron - right.iron,
                left.horse - right.horse
            );
        }

        public void setEmpty() {
            this.wood = 0;
            this.food = 0;
            this.gold = 0;
            this.stone = 0;
            this.iron = 0;
            this.horse = 0;
        }

        public bool empty() {
            return anyValOver(-1);
        }

        public void giveResources(ResourceSet resourceSet)
        {
            wood = wood + resourceSet.wood;
            food = food + resourceSet.food;
            gold = gold + resourceSet.gold;
            stone = stone + resourceSet.stone;
            iron = iron + resourceSet.iron;
            horse = horse + resourceSet.horse;
        }
        public void takeResources(ResourceSet resourceSet)
        {
            wood = wood - resourceSet.wood;
            food = food - resourceSet.food;
            gold = gold - resourceSet.gold;
            stone = stone - resourceSet.stone;
            iron = iron - resourceSet.iron;
            horse = horse - resourceSet.horse;
        }
        public bool canAfford(ResourceSet resourceSet) { return (this >= resourceSet); }

        public static object Deserialize(byte[] data)
        {
            var result = new ResourceSet();
            result.food = BitConverter.ToInt32(data, 0);
            result.wood = BitConverter.ToInt32(data, 4);
            result.gold = BitConverter.ToInt32(data, 8);
            return result;
        }

        public const int SIZE_IN_BYTES = 20;

        public static byte[] Serialize(object customType)
        {
            var c = (ResourceSet)customType;
            var foodBits = BitConverter.GetBytes(c.food);
            var woodBits = BitConverter.GetBytes(c.wood);
            var goldBits = BitConverter.GetBytes(c.gold);
            var stoneBits = BitConverter.GetBytes(c.stone);
            var ironBits = BitConverter.GetBytes(c.iron);
            return new byte[] { 
                foodBits[0],
                foodBits[1],
                foodBits[2],
                foodBits[3],
                woodBits[0],
                woodBits[1],
                woodBits[2],
                woodBits[3],
                goldBits[0],
                goldBits[1],
                goldBits[2],
                goldBits[3],
                stoneBits[0],
                stoneBits[1],
                stoneBits[2],
                stoneBits[3],
                ironBits[0],
                ironBits[1],
                ironBits[2],
                ironBits[3],
            };
        }
    }
}
