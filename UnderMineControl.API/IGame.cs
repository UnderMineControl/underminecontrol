using System.Collections.Generic;
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
        /// Gets all enemy entities
        /// </summary>
        /// <returns>The collection of enemy entities</returns>
        Entity[] GetAllEnemyEntities();

        /// <summary>
        /// Gets all of the currently active entities in the game.
        /// </summary>
        /// <returns>The collection of entities</returns>
        EntityList GetAllGameEntities();

        /// <summary>
        /// Finds an enemy by name or guid
        /// </summary>
        /// <param name="nameOrGuid">The name or the guid of the enemy</param>
        /// <returns>The enemy data or null if the enemy wasn't found</returns>
        EntityData GetEnemy(string nameOrGuid);

        /// <summary>
        /// Scans the enemy collection and finds any enemy where the name matches
        /// </summary>
        /// <param name="name">The name to check for</param>
        /// <returns>A collection of the names</returns>
        IEnumerable<string> GetEnemyLike(string name);

        /// <summary>
        /// Gets a relic by name or guid
        /// </summary>
        /// <param name="nameOrGuid">The name or the guid of the relic</param>
        /// <returns>The relic data or null if it wasn't found</returns>
        ItemData GetRelic(string nameOrGuid);

        /// <summary>
        /// Attempts to spawn an enemy
        /// </summary>
        /// <param name="data">The entity data for the enemy to spawn</param>
        /// <returns>The entity if it was spawned or null if something didn't work</returns>
        Entity SpawnEnemy(EntityData data);

        /// <summary>
        /// Attempts to spawn a relic by the given name or GUID
        /// </summary>
        /// <param name="nameOrGuid">The name or guid of the relic</param>
        /// <param name="entity">The entity that is returned, or null if something goes wrong during spawning</param>
        /// <param name="forceDiscover">Whether or not to force the relic to be discovered if it's not</param>
        /// <param name="forceUnlock">Whether or not to force the relic to be unlocked if it's not</param>
        /// <returns>true if the relic was found, false if it was not. The result can be true and the Entity can be null if the spawning failed, but the relic was found.</returns>
        bool SpawnRelic(string nameOrGuid, out Entity entity, bool forceDiscover = false, bool forceUnlock = false);

        /// <summary>
        /// Spawns the specified relic
        /// </summary>
        /// <param name="data">The item data for the relic</param>
        /// <param name="forceDiscover">Whether or not to force the game to discover the item if it isn't discovered</param>
        /// <param name="forceUnlock">Whether or not to force the game to unlock the item if it isn't unlocked</param>
        /// <returns>The entity if it was spawned or null if something didn't work</returns>
        Entity SpawnRelic(ItemData data, bool forceDiscover = false, bool forceUnlock = false);

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
