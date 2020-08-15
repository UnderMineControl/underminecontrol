using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using Thor;

namespace UnderMineControl
{
    using API;
    using Menu;
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
        private IResourceUtility _resources;

        private List<Mod> _loadedMods = new List<Mod>();

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
                _patcher.Patch(this, typeof(PlayerChapter), "Initialize", null, "PatchGameTime_Bind");
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
                _game = new UnderMineGame(Game.Instance, _logger);
                _player = new PlayerWrapper(_game);
                _events = new Events(_game, _logger, _patcher);
                _resources = new ResourceUtility();

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
                                mod.Resources = _resources;
                                mod.MenuRenderer = new MenuUtility(_resources);
                                mod.Configuration = new ConfigurationUtility(_logger, mod.ModData.ModDirectory);

                                mod.Initialize();
                                _loadedMods.Add(mod);
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

        private void OnGUI()
        {
            foreach(var mod in _loadedMods)
            {
                try
                {
                    mod.OnGUI();
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error occurred while running OnGUI for " + mod.ModData.Data.Name + "\r\n" + ex);
                }
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

        public static void PatchGameTime_Bind(PlayerChapter __instance)
        {
            try
            {
                PatchGoldAmount(__instance);
            }
            catch (Exception ex)
            {
                _instance._logger.Error("Error patching gold amount: " + ex);
            }
        }

        private static void PatchGoldAmount(PlayerChapter chapter)
        {
            var patcher = _instance._patcher;
            InventoryExt extension1 = chapter.Owner.GetExtension<InventoryExt>();

            var goldText = patcher.GetField<LocalizedText>(chapter, "m_goldText");

            int resource1 = extension1.GetResource(GameData.Instance.GoldResource);
            int goldRetainAmount = Game.Instance.ResourceManager.GetGoldRetainAmount(resource1);
            goldText.Text = resource1 < 0 || Game.Instance.Mode != Game.GameMode.Story ? 
                    "{" + resource1.ToString() + "}" : 
                    string.Format("{0} {{{1}}}", resource1, goldRetainAmount);
        }
    }
}
