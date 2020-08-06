using System;
using BepInEx;
using HarmonyLib;

namespace UnderMineControl
{
    using API;
    using Thor;
    using Utility;

    [BepInPlugin("org.underminecontrol.api.entryplugin", "Entry Plugin", "1.0.0.0")]
    [BepInProcess("UnderMine.exe")]
    public class EntryPlugin : BaseUnityPlugin
    {
        private static EntryPlugin _instance;

        private Harmony _harmony;
        private IEvents _events;
        private IGame _game;
        private ILogger _logger;
        private IPatcher _patcher;
        private IPlayer _player;
        private IModLoader _loader;
        private IConfigReader _config;

        public EntryPlugin()
        {
            _instance = this;
            _harmony = new Harmony("org.undermine.api.entryplugin");
            _logger = new Logger();
            _patcher = new Patcher(_harmony, _logger);
        }

        public void Awake()
        {
            try
            {
                _patcher.Patch(this, typeof(Game), "Start", null, "GameStart");
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while performing initial patching:\r\n" + ex);
            }
        }

        public void InitializeMods()
        {
            _config = new ConfigReader();
            _loader = new ModLoader(_logger, _config);
            _game = new UnderMineGame(Game.Instance);
            _player = new PlayerWrapper(_game);
            _events = new Events(_game, _logger, _patcher);

            var mods = _loader.LoadMods();

            foreach (var modParent in mods)
            {
                foreach (var assembly in modParent.Mods)
                {
                    foreach (var mod in assembly.Value)
                    {
                        try
                        {
                            mod.Events = _events;
                            mod.GameInstance = _game;
                            mod.Logger = _logger;
                            mod.Patcher = _patcher;
                            mod.Player = _player;

                            mod.Initialize();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Error running mod: {mod.ModData.Data.Name}\r\n{ex}");
                        }
                    }
                }
            }

            ((Events)_events).Patch();
        }

        public static void GameStart()
        {
            _instance.InitializeMods();
        }
    }
}
