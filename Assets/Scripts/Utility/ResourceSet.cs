﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.utilities.resources {
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

        public void setEmpty() {
            this.wood = 0;
            this.food = 0;
            this.gold = 0;
            this.stone = 0;
            this.iron = 0;
            this.horse = 0;
        }

        public bool empty() {
            return false;
        }
    }
}
