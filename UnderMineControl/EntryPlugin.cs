using System;
using BepInEx;
using HarmonyLib;

namespace UnderMineControl
{
    using API;
    using System.Runtime.InteropServices;
    using Thor;
    using Utility;

    [BepInPlugin("org.underminecontrol.api.entryplugin", "Entry Plugin", "1.0.0.0")]
    [BepInProcess("UnderMine.exe")]
    public class EntryPlugin : BaseUnityPlugin
    {
        private static EntryPlugin _instance;
        private static int _modCount = 0;

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
                _patcher.Patch(this, typeof(Game), "Start", null, "GameStart_Bind");
                _patcher.Patch(this, typeof(ResourcePanel), "Initialize", null, "PatchVersion_Bind");
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while performing initial patching:\r\n" + ex);
            }
        }

        public void InitializeMods()
        {
            try
            {
                _config = new ConfigReader();
                _loader = new ModLoader(_logger, _config);
                _game = new UnderMineGame(Game.Instance);
                _player = new PlayerWrapper(_game);
                _events = new Events(_game, _logger, _patcher);

                var mods = _loader?.LoadMods();
                if (mods == null)
                {
                    _logger.Warn("No mods were found!");
                    return;
                }

                foreach (var modParent in mods)
                {
                    if (modParent == null)
                    {
                        _logger.Warn("Mod Parent missing.");
                        continue;
                    }

                    if (modParent.Mods == null)
                    {
                        _logger.Warn("Mod Parent Mods are missing.");
                        continue;
                    }

                    foreach (var assembly in modParent?.Mods)
                    {
                        foreach (var mod in assembly.Value)
                        {
                            if (mod == null)
                            {
                                _logger.Warn("Mod is null.");
                                continue;
                            }

                            try
                            {
                                mod.Events = _events;
                                mod.GameInstance = _game;
                                mod.Logger = _logger;
                                mod.Patcher = _patcher;
                                mod.Player = _player;

                                mod.Initialize();
                                _modCount += 1;
                            }
                            catch (Exception ex)
                            {
                                _logger.Error($"Error running mod: {mod.ModData.Data.Name}\r\n{ex}");
                            }
                        }
                    }
                }

                ((Events)_events).Patch();
                _logger.Debug("Events have been patched");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while setting up mods: \r\n{ex}");
            }
        }

        public static void PatchVersion_Bind(ResourcePanel __instance)
        {
            try
            {
                var patcher = _instance._patcher;
                var versionText = patcher.GetField<LocalizedText>(__instance, "m_versionText");
                versionText.Text += " (UMC)";
                _instance._logger.Debug("Patched version text!");
            }
            catch (Exception ex)
            {
                _instance._logger.Error("Error patching version text: " + ex);
            }
        }

        public static void GameStart_Bind()
        {
            _instance.InitializeMods();
        }
    }
}
