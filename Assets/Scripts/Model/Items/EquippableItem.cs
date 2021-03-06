﻿using Scripts.Model.Spells;
using System.Collections;
using System.Collections.Generic;
using Scripts.Model.Stats;
using System;
using UnityEngine;
using Scripts.Model.Buffs;
using Scripts.Game.Defined.Spells;
using Scripts.Game.Defined.Serialized.Spells;
using Scripts.Model.SaveLoad;
using Scripts.Model.SaveLoad.SaveObjects;
using Scripts.Model.Characters;

namespace Scripts.Model.Items {

    /// <summary>
    /// Equippable items can be worn in a particular slot. Denoted by EquipType
    /// </summary>
    /// <seealso cref="Scripts.Model.Items.UseableItem" />
    /// <seealso cref="Scripts.Model.SaveLoad.ISaveable{Scripts.Model.SaveLoad.SaveObjects.EquipItemSave}" />
    public abstract class EquippableItem : UseableItem, ISaveable<EquipItemSave> {

        /// <summary>
        /// The equipment slot this item is equipped in.
        /// </summary>
        public readonly EquipType Type;

        /// <summary>
        /// The stat bonuses of this item.
        /// </summary>
        private readonly IDictionary<StatType, int> statBonuses;

        private readonly SpellBook book;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquippableItem"/> class.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="type">The type.</param>
        /// <param name="basePrice">The base price.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public EquippableItem(Sprite sprite, EquipType type, int basePrice, string name, string description)
            : base(sprite, basePrice, TargetType.SINGLE_ALLY, name, description) {
            this.Type = type;
            this.statBonuses = new SortedDictionary<StatType, int>();
            this.book = new CastEquipItem(this);
        }

        /// <summary>
        /// Spriteless constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="basePrice">The base price.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public EquippableItem(EquipType type, int basePrice, string name, string description)
            : this(GetDefaultSprite(type), type, basePrice, name, description) { }

        public ReadOnlyDictionary<StatType, int> StatBonuses {
            get {
                return new ReadOnlyDictionary<StatType, int>(statBonuses);
            }
        }

        protected sealed override string DescriptionHelper {
            get {
                string[] arr = new string[statBonuses.Count];

                int index = 0;
                foreach (KeyValuePair<StatType, int> pair in statBonuses) {
                    arr[index++] = string.Format("{0} {1}", StatUtil.ShowSigns(pair.Value), pair.Key.ColoredName);
                }
                return string.Format("{0}\n{1}\n{2}", Type.Name, string.Join("\n", arr), Util.ColorString(Flavor, Color.grey));
            }
        }

        /// <summary>
        /// Creates the buff associated with this equippable item.
        /// </summary>
        /// <returns>A buff, possibly.</returns>
        public virtual Buff CreateBuff() {
            return null;
        }

        /// <summary>
        /// Gets the spell book associated with equipping this item.
        /// </summary>
        /// <returns>A spellbook associated with equipping this item.</returns>
        public sealed override SpellBook GetSpellBook() {
            return book;
        }

        /// <summary>
        /// Determines whether the other requirements for caster to equip this item on target are met.
        /// </summary>
        /// <param name="caster">The character equipping the item onto someone.</param>
        /// <param name="target">The character who is getting an item equipped onto them..</param>
        /// <returns>
        ///   <c>true</c> if requirements are met; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsMeetOtherRequirements(Character caster, Character target) {
            return caster.Stats.State == Characters.State.ALIVE && target.Stats.State == Characters.State.ALIVE;
        }

        /// <summary>
        /// Adds a stat bonus to the equipment.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="amount">The amount.</param>
        protected void AddStatBonus(StatType type, int amount) {
            Util.Assert(StatType.ASSIGNABLES.Contains(type), "StatType must be assignable.");
            Util.Assert(amount != 0, "Amount must be nonzero.");
            statBonuses[type] = amount;
        }

        /// <summary>
        /// Gets the default sprite.
        /// </summary>
        /// <param name="type">The type to get the default sprite of.</param>
        /// <returns></returns>
        private static Sprite GetDefaultSprite(EquipType type) {
            return type.Sprite;
        }

        /// <summary>
        /// Gets the save object.
        /// </summary>
        /// <returns></returns>
        EquipItemSave ISaveable<EquipItemSave>.GetSaveObject() {
            return new EquipItemSave(Type.GetSaveObject(), GetType());
        }

        /// <summary>
        /// Initializes from save object.
        /// </summary>
        /// <param name="saveObject">The save object.</param>
        public void InitFromSaveObject(EquipItemSave saveObject) {
            // Nothing
        }
    }
}