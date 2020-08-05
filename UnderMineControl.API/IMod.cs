namespace UnderMineControl.API
{
    /// <summary>
    /// Used to create mods in the world
    /// </summary>
    public interface IMod
    {
        /// <summary>
        /// Fired whenever the game starts
        /// </summary>
        void Initialize();
    }
}
