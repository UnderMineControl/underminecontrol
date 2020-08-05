using Thor;

namespace UnderMineControl.API
{
    /// <summary>
    /// This represents the instance of the player
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Direct access to the players health instance
        /// </summary>
        HealthExt Health { get; }
        /// <summary>
        /// Direct access to the players damage instance
        /// </summary>
        DamageExt Damage { get; }
        /// <summary>
        /// Direct access to the players inventory instance
        /// </summary>
        InventoryExt Inventory { get; }

        /// <summary>
        /// The players max HP
        /// </summary>
        int MaxHP { get; set; }
        /// <summary>
        /// The players current HP
        /// </summary>
        int CurrentHP { get; set; }
        /// <summary>
        /// Whether or not the player is invulnerable (god mode)
        /// </summary>
        bool Invulnerable { get; set; }

        /// <summary>
        /// The number of bombs the player has
        /// </summary>
        int Bombs { get; set; }
        /// <summary>
        /// The number of keys the player has
        /// </summary>
        int Keys { get; set; }
        /// <summary>
        /// How much gold the player has
        /// </summary>
        int Gold { get; set; }
        /// <summary>
        /// How much Thorium the player has
        /// </summary>
        int Thorium { get; set; }

        /// <summary>
        /// Allows for accessing properties on the different extension classes
        /// </summary>
        /// <typeparam name="T">The type of extension you're modifying</typeparam>
        /// <param name="member">The name of the property you're modifying</param>
        /// <param name="value">The value you're modifying</param>
        /// <param name="op">The operation you're using to modify. Defaults to <see cref="Modifier.Assign"/></param>
        void SetValue<T>(string member, float value, Modifier.Operator op = null);
        /// <summary>
        /// Allows for accessing properties on the different extension classes
        /// </summary>
        /// <typeparam name="T">The type of extension you're modifying</typeparam>
        /// <param name="member">The name of the property you're modifying</param>
        /// <param name="value">The value you're modifying</param>
        /// <param name="op">The operation you're using to modify. Defaults to <see cref="Modifier.Assign"/></param>
        void SetValue<T>(string member, bool value, Modifier.Operator op = null);

        /// <summary>
        /// Adds a random blessing to your character
        /// </summary>
        void AddRandomBlessing();
        /// <summary>
        /// Adds a random curse to your character
        /// </summary>
        /// <param name="type">The type of curse to add (Major or Minor)</param>
        void AddRandomCurse(HealthExt.CurseType type = HealthExt.CurseType.Minor);
        /// <summary>
        /// Removes a random curse from your character
        /// </summary>
        /// <param name="curse">The curse that was removed</param>
        /// <param name="type">The type of curse to remove (Major or Minor)</param>
        void RemoveRandomCurse(out Entity curse, HealthExt.CurseType type = HealthExt.CurseType.Minor);
        /// <summary>
        /// Changes the HP values of your character
        /// </summary>
        /// <param name="amount">How much to change it by</param>
        /// <param name="type">The type of damage that is done</param>
        /// <param name="ignoreGrace">Whether or not to ignore Grace</param>
        void ChangeHP(int amount, HealthExt.DamageType type = HealthExt.DamageType.Physical, bool ignoreGrace = true);
    }
}
