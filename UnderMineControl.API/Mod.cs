namespace UnderMineControl.API
{
    using Models;

    /// <summary>
    /// Used to create mods in the world
    /// </summary>
    public abstract class Mod
    {
        /// <summary>
        /// A collection of useful events that fire in UnderMine
        /// </summary>
        public IEvents Events { get; set; }
        /// <summary>
        /// This represents the instance of the game
        /// </summary>
        public IGame GameInstance { get; set; }
        /// <summary>
        /// Provides access to logging functions
        /// </summary>
        public ILogger Logger { get; set; }
        /// <summary>
        /// Used to patch game methods using Harmony
        /// </summary>
        public IPatcher Patcher { get; set; }
        /// <summary>
        /// This represents the instance of the player
        /// </summary>
        public IPlayer Player { get; set; }
        /// <summary>
        /// Gets the mod's information
        /// </summary>
        public IMod ModData { get; set; }
        /// <summary>
        /// Allows access to resources embedded within your mod
        /// </summary>
        public IResourceUtility Resources { get; set; }
        /// <summary>
        /// Allows your application to render it's own menu!
        /// </summary>
        public IMenu MenuRenderer { get; set; }
        /// <summary>
        /// Allows access to configuration files
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Fired whenever the game starts
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Allows for drawing things to the UI
        /// </summary>
        public virtual void OnGUI()
        {
            MenuRenderer?.Render();
        }
    }
}