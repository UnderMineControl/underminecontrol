using System;
using System.Linq;
using System.Reflection;
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

        public UnderMineGame(Game game)
        {
            Game = game;
        }

        public Entity SpawnEntity(Entity item, Vector3 position, Quaternion rotation, int playerId, Entity owner = null, Transform parent = null)
        {
            parent = parent ?? Game.Simulation.SpawnParent;
            var entity = UnityEngine.Object.Instantiate(item, position, rotation, parent);
            entity.name = item.name;
            entity.Prefab = item;

            Array.ForEach(entity.GetComponentsInChildren<Grid>(true), (e) =>
            {
                e.Initialize();
                e.Merge();
            });
            Array.ForEach(entity.GetComponentsInChildren<TextureLoader>(true), e => e.Initialize());
            Array.ForEach(entity.GetComponentsInChildren<AnimationLoader>(true), e => e.Initialize());

            entity.Initialize();
            entity.PreSetup(Game.Simulation.GetPlayerByID(playerId), owner);
            entity.Setup();

            GetAllGameEntities().Add(entity);
            Game.Simulation.FireEvent(SimulationEvent.EntitySpawned.Create(entity, position));
            return entity;
        }

        public Entity SpawnEntityOnPlayer(Entity entity)
        {
            var player = Player;
            var avatar = player.Avatar;
            var position = avatar.transform.position;
            var rotation = avatar.transform.rotation;

            var placement = Simulation.GetNearestWalkablePosition(position + Rand.OnUnitSphere * 5f, avatar.AgentTypeID);
            return SpawnEntity(entity, placement, rotation, player.ID);
        }

        public bool KeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        public bool KeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        public EntityList GetAllGameEntities()
        {
            var type = Simulation.GetType().GetField("mAllEntities", BindingFlags.Instance | BindingFlags.NonPublic);
            return (EntityList)type.GetValue(Game.Instance.Simulation);
        }
    }
}
