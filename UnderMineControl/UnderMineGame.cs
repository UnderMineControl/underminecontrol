using System;
using System.Collections.Generic;
using System.Linq;
using Thor;
using UnityEngine;

namespace UnderMineControl
{
    using API;

    public class UnderMineGame : IGame
    {
        #region Properties
        public Game Game { get; }
        public Simulation Simulation => Game.Simulation;
        public SimulationPlayer Player => Game.Simulation.Players.FirstOrDefault();
        public GameData Data => GameData.Instance;
        #endregion

        private readonly ILogger _logger;
        private Entity[] _enemyPrefabs;

        public UnderMineGame(Game game, ILogger logger)
        {
            Game = game;
            _logger = logger;
        }

        #region Exposed API methods

        /// <summary>
        /// Gets all enemy entities in the game
        /// </summary>
        /// <returns></returns>
        public Entity[] GetAllEnemyEntities()
        {
            if (_enemyPrefabs != null &&
                _enemyPrefabs.Length > 0)
                return _enemyPrefabs;

            return _enemyPrefabs = Resources.LoadAll<Entity>("Entities/Dynamic");
        }

        /// <summary>
        /// Gets all of the currently active entities in the game.
        /// </summary>
        /// <returns>The collection of entities</returns>
        public EntityList GetAllGameEntities()
        {
            return Simulation?.Entities;
        }

        /// <summary>
        /// Finds an enemy by name or guid
        /// </summary>
        /// <param name="nameOrGuid">The name or the guid of the enemy</param>
        /// <returns>The enemy data or null if the enemy wasn't found</returns>
        public EntityData GetEnemy(string nameOrGuid)
        {
            foreach(EnemyData entity in Data.EnemyCollection)
            {
                if (entity.name.ToLower() == nameOrGuid.ToLower())
                    return entity;

                if (entity.guid.ToLower() == nameOrGuid.ToLower())
                    return entity;
            }

            foreach(var entity in GetAllEnemyEntities())
            {
                if (entity.Data == null)
                    continue;

                if (entity.Data.guid.ToLower() == nameOrGuid.ToLower())
                    return entity.Data;

                if (entity.Data.name.ToLower() == nameOrGuid.ToLower())
                    return entity.Data;
            }

            return null;
        }

        /// <summary>
        /// Scans the enemy collection and finds any enemy where the name matches
        /// </summary>
        /// <param name="name">The name to check for</param>
        /// <returns>A collection of the names</returns>
        public IEnumerable<string> GetEnemyLike(string name)
        {
            foreach (EnemyData entity in Data.EnemyCollection)
                if (entity.name.ToLower().Contains(name.ToLower()))
                    yield return entity.name;
        }

        /// <summary>
        /// Gets a relic by name or guid
        /// </summary>
        /// <param name="nameOrGuid">The name or the guid of the relic</param>
        /// <returns>The relic data or null if it wasn't found</returns>
        public ItemData GetRelic(string nameOrGuid)
        {
            try
            {
                foreach(ItemData relic in Data.RelicCollection)
                {
                    if (relic.name.ToLower() == nameOrGuid.ToLower())
                        return relic;

                    if (relic.guid.ToLower() == nameOrGuid.ToLower())
                        return relic;
                }

                _logger.Warn("Couldn't find relic: " + nameOrGuid);
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while attempting to find {nameOrGuid} relic: " + ex);
                return null;
            }
        }

        /// <summary>
        /// Attempts to spawn an enemy
        /// </summary>
        /// <param name="data">The entity data for the enemy to spawn</param>
        /// <returns>The entity if it was spawned or null if something didn't work</returns>
        public Entity SpawnEnemy(EntityData data)
        {
            try
            {
                if (data == null)
                {
                    _logger.Warn("EntityData cannot be null!");
                    return null;
                }

                var prefab = FindEnemyPrefab(data);
                if (prefab == null)
                {
                    _logger.Warn("Entity prefab was not found.");
                    return null;
                }

                var position = Simulation.Zone.CurrentRoom.GetRandomPosition(true, prefab.AgentTypeID);
                return Simulation.SpawnEntity(prefab, position, Quaternion.identity, -1, null, null);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while spawning enemy: " + ex);
                return null;
            }
        }

        /// <summary>
        /// Attempts to spawn a relic by the given name or GUID
        /// </summary>
        /// <param name="nameOrGuid">The name or guid of the relic</param>
        /// <param name="entity">The entity that is returned, or null if something goes wrong during spawning</param>
        /// <param name="forceDiscover">Whether or not to force the relic to be discovered if it's not</param>
        /// <param name="forceUnlock">Whether or not to force the relic to be unlocked if it's not</param>
        /// <returns>true if the relic was found, false if it was not. The result can be true and the Entity can be null if the spawning failed, but the relic was found.</returns>
        public bool SpawnRelic(string nameOrGuid, out Entity entity, bool forceDiscover = false, bool forceUnlock = false)
        {
            entity = null;

            try
            {
                if (string.IsNullOrEmpty(nameOrGuid))
                {
                    _logger.Warn("Please specify the name or guid of the relic you want to spawn");
                    return false;
                }

                var relic = GetRelic(nameOrGuid);

                if (relic == null)
                    return false;

                if (forceDiscover && !relic.IsDiscovered)
                    Data.Discover(relic);

                if (!forceDiscover && !relic.IsDiscovered)
                    return false;

                if (forceUnlock && !relic.IsUnlocked)
                    Data.Unlock(relic);

                if (!forceUnlock && !relic.IsUnlocked)
                    return false;

                entity = SpawnRelic(relic);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while spawning {nameOrGuid}: " + ex);
                return false;
            }
        }

        /// <summary>
        /// Spawns the specified relic
        /// </summary>
        /// <param name="data">The item data for the relic</param>
        /// <param name="forceDiscover">Whether or not to force the game to discover the item if it isn't discovered</param>
        /// <param name="forceUnlock">Whether or not to force the game to unlock the item if it isn't unlocked</param>
        /// <returns>The entity if it was spawned or null if something didn't work</returns>
        public Entity SpawnRelic(ItemData data, bool forceDiscover = false, bool forceUnlock = false)
        {
            try
            {
                if (data == null)
                {
                    _logger.Warn("ItemData cannot be null!");
                    return null;
                }

                if (forceDiscover && !data.IsDiscovered)
                    Data.Discover(data);

                if (!forceDiscover && !data.IsDiscovered)
                    return null;

                if (forceUnlock && !data.IsUnlocked)
                    Data.Unlock(data);

                if (!forceUnlock && !data.IsUnlocked)
                    return null;

                var prefab = Data?.GetItemTemplate(data);
                if (prefab == null)
                {
                    _logger.Warn("Could not find item template for: " + data?.name);
                    return null;
                }

                using (new ItemExt.ItemDataScope(data))
                {
                    var position = Player.Avatar.Position + Vector3.right * 3f;
                    var entity = Simulation.SpawnEntity(prefab, position, Quaternion.identity, -1, null, null);
                    var mover = entity.GetExtension<MoverExt>();
                    if (mover == null)
                        return entity;

                    entity.transform.position += Vector3.up;
                    var normalized = Rand.InsideUnitCircle.normalized;
                    var point = new Vector3(normalized.x * Rand.Range(2f, 6f), 0.0f, normalized.y * Rand.Range(2f, 4f));
                    var walkable = Simulation.GetNearestWalkablePosition(entity.transform.LocalToWorldPoint(point), entity.AgentTypeID);
                    mover.Launch(walkable, Rand.Range(4, 5), 75f, false);
                    return entity;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while spawning relic: " + ex);
                return null;
            }
        }

        /// <summary>
        /// Checks if a key is currently being held down
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Whether or not the key is held down</returns>
        public bool KeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        /// <summary>
        /// Checks if the key is not being held
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Whether or not the key is not being held</returns>
        public bool KeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        #endregion

        #region None exposed API methods

        /// <summary>
        /// Attempts to find the spawn template for the given enemy
        /// </summary>
        /// <param name="data">The enemy to find the template for</param>
        /// <returns>The spawn template</returns>
        public Entity FindEnemyPrefab(EntityData data)
        {
            foreach (Entity entity in GetAllEnemyEntities())
            {
                if (entity?.Data?.guid == data?.guid)
                    return entity;
            }

            return null;
        }

        #endregion
    }
}
