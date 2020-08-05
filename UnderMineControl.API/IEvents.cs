using System;
using Thor;

namespace UnderMineControl.API
{
    /// <summary>
    /// A collection of useful events that fire in UnderMine
    /// </summary>
    public interface IEvents
    {
        /// <summary>
        /// This gets fired whenever the player entity spawns in the world
        /// </summary>
        event EventHandler<SimulationPlayer> OnAvatarSpawned;
        /// <summary>
        /// This gets fired whenever the player entity despawns
        /// </summary>
        event EventHandler<SimulationPlayer> OnAvatarDestroyed;
        /// <summary>
        /// This fires on a loop and handles most of the games mechanics
        /// </summary>
        event EventHandler<IGame> OnGameUpdated;
        /// <summary>
        /// This fires whenever an event happens on the simulation. You can check out the list of them <see cref="SimulationEvent.EventType"/>
        /// </summary>
        event EventHandler<SimulationEvent> OnSimulationEvent;
    }
}
