using HarmonyLib;
using System;
using System.Linq;
using BepInEx;
using Thor;

namespace UnderMineControl.API
{
    using Utility;

    [BepInPlugin("org.underminecontrol.api.entryplugin", "Entry Plugin", "1.0.0.0")]
    [BepInProcess("UnderMine.exe")]
    public class EntryPlugin : BaseUnityPlugin
    {
        private static Harmony _harmony; 
        private static ILogger _logger;
        private static IPatcher _patcher;
        private static EntryPlugin _entry;

        public void Awake()
        {
            _harmony = new Harmony("org.underminecontrol.api");
            _logger = new Logger();
            _patcher = new Patcher(_harmony, _logger);
            _entry = this;

            _patcher.Patch(this, typeof(Game), "Start", null, "Init");
        }

        public static void Init()
        {
            try
            {
                var refl = GetDI();

                var loader = refl.GetInstance<IModLoader>();
                var events = (Events)refl.GetInstance<IEvents>();
                var logger = refl.GetInstance<ILogger>();
                
                events.Patch();

                logger.Debug("Starting mod loading...");
                var mods = loader.LoadMods();
                foreach (var mod in mods)
                {
                    foreach (var entry in mod.Mods)
                    {
                        foreach (var m in entry.Value)
                        {
                            try
                            {
                                m.Initialize();
                                logger.Debug($"Mod Intialized: {mod.Data.Name}::{m.GetType().Name}");
                            }
                            catch (Exception ex)
                            {
                                logger.Error($"Error occurred while triggering mod: {mod.Data.Name}::{m.GetType().Name}\r\n{ex}");
                            }
                        }
                    }
                }

                logger.Debug("Finsihing mod loading...");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }

        private static IReflectionUtility GetDI()
        {
            var reflection = new ReflectionUtility();
            reflection.RegisterObject(reflection);

            reflection.RegisterObject(Game.Instance);
            reflection.RegisterObject(GameData.Instance);
            reflection.RegisterObject(_harmony);
            reflection.RegisterObject(_entry);

            reflection.RegisterObject(_logger);
            reflection.RegisterObject(_patcher);
            reflection.RegisterObject<ModLoader>();
            reflection.RegisterObject<UnderMineGame>();
            reflection.RegisterObject<Events>();
            reflection.RegisterObject<PlayerWrapper>();

            return reflection;
        }
    }
}
