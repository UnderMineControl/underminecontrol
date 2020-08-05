using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderMineControl
{
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
            Health.ChangeHP(avatar, avatar, type, ignoreGrace, amount, out int _);
        }
    }
}
