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
            _game = game;
            _logger = logger;
            _patcher = patcher;
            _instance = this;
        }

        public event EventHandler<SimulationPlayer> OnAvatarSpawned = delegate { };
        public event EventHandler<SimulationPlayer> OnAvatarDestroyed = delegate { };
        public event EventHandler<IGame> OnGameUpdated = delegate { };
        public event EventHandler<SimulationEvent> OnSimulationEvent = delegate { };

        public static void AvatarSpawned(SimulationPlayer player)
        {
            try
            {
                _instance._logger.Debug($"Avatar Spawned: {player?.Avatar?.name}");
                _instance.OnAvatarSpawned?.Invoke(_instance._game, player);
            }
            catch (Exception ex)
            {
                _instance._logger.Error(nameof(AvatarSpawned) + "GAME UPDATED: Error occurred: \r\n" + ex.ToString());
            }
        }

        public static void AvatarDestroyed(SimulationPlayer player)
        {

            try
            {
                _instance._logger.Debug($"Avatar Destroyed: {player?.Avatar?.name}");
                _instance.OnAvatarDestroyed?.Invoke(_instance._game, player);
            }
            catch (Exception ex)
            {
                _instance._logger.Error(nameof(AvatarDestroyed) + "GAME UPDATED: Error occurred: \r\n" + ex.ToString());
            }
        }

        public static void GameUpdated()
        {
            try
            {
                _instance.OnGameUpdated?.Invoke(_instance._game, _instance._game);
            }
            catch (Exception ex)
            {
                _instance._logger.Error(nameof(GameUpdated) + "GAME UPDATED: Error occurred: \r\n" + ex.ToString());
            }
        }

        public static void SimulationEvent(SimulationEvent simEvent)
        {
            try
            {
                _instance.OnSimulationEvent?.Invoke(_instance._game, simEvent);
            }
            catch (Exception ex)
            {
                _instance._logger.Error(nameof(SimulationEvent) + "SIMULATION EVENT: Error occurred: \r\n" + ex.ToString());
            }
        } 

        public static void PlayerEvent(PlayerEvent playerEvent)
        {
            try
            {
                switch (playerEvent.Type)
                {
                    case Thor.PlayerEvent.EventType.DestroysAvatar:
                        _instance.OnAvatarSpawned(_instance._game, playerEvent?.Player);
                        break;
                    case Thor.PlayerEvent.EventType.SpawnsAvatar:
                        _instance.OnAvatarDestroyed(_instance._game, playerEvent?.Player);
                        break;
                }
            }
            catch (Exception ex)
            {
                _instance._logger.Error(nameof(PlayerEvent) + ": Error occurred: \r\n" + ex.ToString());
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
