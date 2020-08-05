using System;
using Thor;

namespace UnderMineControl.API
{
    public interface IEvents
    {
        event EventHandler<SimulationPlayer> OnAvatarSpawned;
        event EventHandler<SimulationPlayer> OnAvatarDestroyed;
        event EventHandler<IGame> OnGameUpdated;
        event EventHandler<SimulationEvent> OnSimulationEvent;
    }
}
