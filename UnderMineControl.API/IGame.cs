using Thor;
using UnityEngine;

namespace UnderMineControl.API
{
    public interface IGame
    {
        Game Game { get; }
        SimulationPlayer Player { get; }
        Simulation Simulation { get; }
        GameData Data { get; }

        Entity SpawnEntity(Entity item, Vector3 position, Quaternion rotation, int playerId, Entity owner = null, Transform parent = null);
        Entity SpawnEntityOnPlayer(Entity entity);
        EntityList GetAllGameEntities();

        bool KeyDown(KeyCode key);

        bool KeyUp(KeyCode key);
    }
}
