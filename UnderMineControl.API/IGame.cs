using Thor;
using UnityEngine;

namespace UnderMineControl.API
{
    /// <summary>
    /// This represents the instance of the game
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Direct access to the game instance
        /// </summary>
        Game Game { get; }
        /// <summary>
        /// Direct access to the player instance
        /// </summary>
        SimulationPlayer Player { get; }
        /// <summary>
        /// Direct access to the simulation instance
        /// </summary>
        Simulation Simulation { get; }
        /// <summary>
        /// Direct access to the game data instance
        /// </summary>
        GameData Data { get; }

        /// <summary>
        /// Spawns an entity in the world
        /// </summary>
        /// <param name="item">The entity to spawn</param>
        /// <param name="position">The position to spawn at</param>
        /// <param name="rotation">What rotation the entity should spawn at</param>
        /// <param name="playerId">The player who spawned the entity</param>
        /// <param name="owner">The owner of the entity (optional)</param>
        /// <param name="parent">The parent of the entity (optional)</param>
        /// <returns>The entity instance that was spawned</returns>
        Entity SpawnEntity(Entity item, Vector3 position, Quaternion rotation, int playerId, Entity owner = null, Transform parent = null);
        /// <summary>
        /// Spawns an entity on the player
        /// </summary>
        /// <param name="entity">The entity to spawn</param>
        /// <returns>The entity instance that was spawned</returns>
        Entity SpawnEntityOnPlayer(Entity entity);
        /// <summary>
        /// Fetches a collection of all of the entities in the game
        /// </summary>
        /// <returns>The collection of entities</returns>
        EntityList GetAllGameEntities();

        /// <summary>
        /// Checks if a key is currently being held down
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Whether or not the key is held down</returns>
        bool KeyDown(KeyCode key);

        /// <summary>
        /// Checks if the key is not being held
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Whether or not the key is not being held</returns>
        bool KeyUp(KeyCode key);
    }
}
