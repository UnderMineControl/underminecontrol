namespace UnderMineControl
{
    using System.Collections.Generic;
    using API;
    using Thor;

    public class PlayerWrapper : IPlayer
    {
        #region Extensions
        public HealthExt Health => _game.Player.Avatar.GetExtension<HealthExt>();
        public DamageExt Damage => _game.Player.Avatar.GetExtension<DamageExt>();
        public InventoryExt Inventory => _game.Player.Avatar.GetExtension<InventoryExt>();
        #endregion

        #region Health
        public int MaxHP
        {
            get => Health.MaxHP;
            set => SetValue<HealthExt>("MaxHP", value);
        }
        public int CurrentHP
        {
            get => Health.CurrentHP;
            set => Health.SetCurrentHP(value);
        }
        public bool Invulnerable
        {
            get => Health.Invulnerable;
            set => Health.Invulnerable = value;
        }

        public List<ItemData> StatusEffects
        {
            get
            {
                List<ItemData> statusEffects = new List<ItemData>();
                Health.GetStatusEffectData(statusEffects, true);
                return statusEffects;
            }
        }
        #endregion

        #region Inventory
        public int Bombs
        {
            get => Inventory.GetResource(GameData.Instance.BombResource);
            set => Inventory.ChangeResource(GameData.Instance.BombResource, value);
        }
        public int Keys
        {
            get => Inventory.GetResource(GameData.Instance.KeyResource);
            set => Inventory.ChangeResource(GameData.Instance.KeyResource, value);
        }
        public int Gold
        {
            get => Inventory.GetResource(GameData.Instance.GoldResource);
            set => Inventory.ChangeResource(GameData.Instance.GoldResource, value);
        }
        public int Thorium
        {
            get => Inventory.GetResource(GameData.Instance.ThoriumResource);
            set => Inventory.ChangeResource(GameData.Instance.ThoriumResource, value);
        }

        public List<ItemData> Equipment
        {
            get
            {
                List<ItemData> equipment = new List<ItemData>();
                Inventory.GetEquipmentData(equipment);
                return equipment;
            }
        }

        public ItemData BombType => Equipment.Find(item => item.Hint.HasFlag(ItemData.ItemHint.Relic) && item.Slot == "bomb");

        public ItemData ThrowType => Equipment.Find(item => item.Hint.HasFlag(ItemData.ItemHint.Relic) && item.Slot == "gloves");

        public int PotionSlots => throw new System.NotImplementedException();

        #endregion

        private readonly IGame _game;

        public PlayerWrapper(IGame game)
        {
            _game = game;
        }

        public void SetValue<T>(string member, float value, Modifier.Operator op = null)
        {
            op = op ?? Modifier.Assign;
            _game.Player.Avatar.AddModifier(Modifier.Create<T>(member, op, value));
        }

        public void SetValue<T>(string member, bool value, Modifier.Operator op = null)
        {
            op = op ?? Modifier.Assign;
            _game.Player.Avatar.AddModifier(Modifier.Create<T>(member, op, value));
        }

        public void AddRandomBlessing()
        {
            Health.AddRandomBlessing(_game.Player.Avatar);
        }

        public void AddRandomCurse(HealthExt.CurseType type = HealthExt.CurseType.Minor)
        {
            Health.AddRandomCurse(type, _game.Player.Avatar);
        }

        public void RemoveRandomCurse(out Entity curse, HealthExt.CurseType type = HealthExt.CurseType.Minor)
        {
            Health.RemoveRandomCurse(type, out curse);
        }

        public void ChangeHP(int amount, HealthExt.DamageType type = HealthExt.DamageType.Physical, bool ignoreGrace = true)
        {

            var avatar = _game.Player.Avatar;
            var args = new HealthExt.ChangeHPArgs()
            {
                origin = avatar,
                source = avatar,
                damageType = type,
                ignoreGrace = ignoreGrace,
                delta = amount
            };

            Health.ChangeHP(args, out int _);
        }

        public List<ItemData> GetRelics(bool? disabled = null, bool? unique = null)
        {
            // TODO: Implement filtering
            return StatusEffects.FindAll(item => item.Hint.HasFlag(ItemData.ItemHint.Relic));
        }

        public List<ItemData> GetBlessings()
        {
            return StatusEffects.FindAll(item => item.Hint.HasFlag(ItemData.ItemHint.Blessing));
        }

        public List<ItemData> GetCurses(bool? permanent = null)
        {
            // TODO: Implement filtering
            return StatusEffects.FindAll(item => item.Hint.HasFlag(ItemData.ItemHint.Curse));
        }

        public List<ItemData> GetHexes()
        {
            return StatusEffects.FindAll(item => item.Hint.HasFlag(ItemData.ItemHint.Hex));
        }

        public List<ItemData> GetPotions(bool? carried = null)
        {
            if (carried == true)
            {
                return Equipment.FindAll(item => item.Hint.HasFlag(ItemData.ItemHint.Potion));
            }
            
            if (carried == false)
            {
                return StatusEffects.FindAll(item => item.Hint.HasFlag(ItemData.ItemHint.Potion));
            }

            List<ItemData> items = new List<ItemData>();
            Health.GetStatusEffectData(items, true);
            Inventory.GetEquipmentData(items);
            return items.FindAll(item => item.Hint.HasFlag(ItemData.ItemHint.Potion));
        }
    }
}
