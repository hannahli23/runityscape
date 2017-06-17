﻿using Scripts.Model.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Scripts.Model.Stats;
using Scripts.Model.Buffs;
using Scripts.Model.Spells;
using Scripts.Presenter;
using Scripts.Model.Interfaces;

namespace Scripts.Model.Characters {

    public class Equipment : IEnumerable<EquippableItem>, IEnumerable<ISpellable> {
        public readonly IDictionary<EquipType, EquippableItem> Dict;
        public Buffs Buffs;
        public SpellParams Owner;
        public Action<SplatDetails> AddSplat;

        private readonly IDictionary<EquippableItem, Buff> itemBuffs;
        private readonly IDictionary<StatType, int> statBonuses;

        public Equipment() {
            this.Dict = new Dictionary<EquipType, EquippableItem>();
            this.statBonuses = new Dictionary<StatType, int>();
            foreach (StatType s in StatType.ASSIGNABLES) {
                statBonuses.Add(s, 0);
            }
            this.itemBuffs = new Dictionary<EquippableItem, Buff>();
            this.AddSplat = ((p) => { });
        }

        public void AddEquip(Inventory inv, EquippableItem e) {
            inv.Remove(e);
            if (Contains(e.Type)) {
                RemoveEquip(inv, e.Type);
            }

            Buff buff = e.CreateBuff(Owner);
            if (buff != null) {
                itemBuffs.Add(e, buff);
                Buffs.AddBuff(buff);
            }

            Dict.Add(e.Type, e);
            foreach (KeyValuePair<StatType, int> pair in e.Stats) {
                Util.Assert(StatType.ASSIGNABLES.Contains(pair.Key), "Invalid stat type on equipment.");
                this.statBonuses[pair.Key] += pair.Value;
            }

            AddSplat(new SplatDetails(Color.green, "+", e.Icon));
        }

        public void RemoveEquip(Inventory inv, EquipType type) {
            Util.Assert(Dict.ContainsKey(type), "No equipment in slot.");
            EquippableItem itemToRemove = Dict[type];

            Buff buffToRemove = itemBuffs[itemToRemove];
            itemBuffs.Remove(itemToRemove);

            inv.Add(itemToRemove);
            Dict.Remove(itemToRemove.Type);
            Buffs.RemoveBuff(RemovalType.TIMED_OUT, buffToRemove);

            foreach (KeyValuePair<StatType, int> pair in itemToRemove.Stats) {
                Util.Assert(StatType.ASSIGNABLES.Contains(pair.Key), "Invalid stat type on equipment.");
                this.statBonuses[pair.Key] -= pair.Value;
            }

            AddSplat(new SplatDetails(Color.red, "-", itemToRemove.Icon));
        }

        public EquippableItem PeekItem(EquipType type) {
            return Dict[type];
        }

        public int GetBonus(StatType type) {
            return statBonuses[type];
        }

        public bool Contains(EquipType type) {
            return Dict.ContainsKey(type);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Dict.Values.GetEnumerator();
        }

        IEnumerator<EquippableItem> IEnumerable<EquippableItem>.GetEnumerator() {
            return Dict.Values.GetEnumerator();
        }

        IEnumerator<ISpellable> IEnumerable<ISpellable>.GetEnumerator() {
            return Dict.Values.Select(e => e.GetSpellBook()).Cast<ISpellable>().ToList().GetEnumerator();
        }
    }
}
