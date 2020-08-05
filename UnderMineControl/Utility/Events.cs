using System;
using Thor;

namespace UnderMineControl.Utility
{
    using API;

    public class Events : IEvents
    {
        private static Events _instance;

        private readonly IGame _game;
        private readonly ILogger _logger;
        private readonly IPatcher _patcher;

        public Events(IGame game, ILogger logger, IPatcher patcher)
        {
            _game = game ?? throw new NullReferenceException("game");
            _logger = logger ?? throw new NullReferenceException("logger");
            _patcher = patcher ?? throw new NullReferenceException("patcher");

            _instance = this;
        }

        public event EventHandler<SimulationPlayer> OnAvatarSpawned = delegate { };
        public event EventHandler<SimulationPlayer> OnAvatarDestroyed = delegate { };
        public event EventHandler<IGame> OnGameUpdated = delegate { };
        public event EventHandler<SimulationEvent> OnSimulationEvent = delegate { };

        public static void AvatarSpawned(SimulationPlayer player)
        {
            _instance._logger.Debug($"Avatar Spawned: {player?.Avatar?.name}");
            _instance.OnAvatarSpawned?.Invoke(_instance._game, player);
        }

        public static void AvatarDestroyed(SimulationPlayer player)
        {
            _instance._logger.Debug($"Avatar Destroyed: {player?.Avatar?.name}");
            _instance.OnAvatarDestroyed?.Invoke(_instance._game, player);
        }

        public static void GameUpdated()
        {
            _instance.OnGameUpdated?.Invoke(_instance._game, _instance._game);
        }

        public static void SimulationEvent(SimulationEvent simEvent)
        {
            _instance.OnSimulationEvent?.Invoke(_instance._game, simEvent);
        }

        public static void PlayerEvent(PlayerEvent playerEvent)
        {
            switch(playerEvent.Type)
            {
                case Thor.PlayerEvent.EventType.DestroysAvatar:
                    _instance.OnAvatarSpawned(_instance._game, playerEvent.Player);
                    break;
                case Thor.PlayerEvent.EventType.SpawnsAvatar:
                    _instance.OnAvatarDestroyed(_instance._game, playerEvent.Player);
                    break;                    
            }
        }

        public void Patch()
        {

            _logger.Info("Starting Patching...");

            _patcher.Patch(this, typeof(Game), "Update", null, "GameUpdated");
            _patcher.Patch(this, typeof(SimulationPlayer), "FireEvent", null, "PlayerEvent", typeof(PlayerEvent));
            _patcher.Patch(this, typeof(Simulation), "FireEvent", null, "SimulationEvent", typeof(SimulationEvent));

            _logger.Info("Patching Finished");
        }
    }
}
// SteamPath
// 32bit HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam
// 64bit HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam
