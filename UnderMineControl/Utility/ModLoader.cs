using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UnderMineControl.Utility
{
    using API;
    using API.Models;
    using Models;
    using System.Linq;

    public interface IModLoader
    {
        IEnumerable<ModImplementation> LoadMods();
    }

    public class ModLoader : IModLoader
    {
        private const string FILE_EXT = "*.umc";

        private readonly ILogger _logger;
        private readonly IConfigReader _config;
        private Dictionary<Type, object> _injectables = new Dictionary<Type, object>();

        public ModLoader(ILogger logger, IConfigReader config)
        {
            _logger = logger;
            _config = config;
        }

        public IEnumerable<ModImplementation> LoadMods()
        {
            var curDir = Environment.CurrentDirectory;
            var files = Directory.GetFiles(curDir, FILE_EXT, SearchOption.AllDirectories);
            _logger.Debug($"Found {files.Length} mods! Starting load.\r\n{curDir}");
            foreach (var file in files)
            {
                var json = _config.LoadModJson(file);
                if (json == null)
                    continue;

                yield return LoadMod(json, file);
            }
        }

        public ModImplementation LoadMod(ModJson json, string jsonFilePath)
        {
            if (json == null ||
                json.EntryFiles == null ||
                json.EntryFiles.Length == 0)
            {
                _logger.Warn("JSON file doesn't contain entry files: " + jsonFilePath);
                return null;
            }

            var imp = new ModImplementation
            {
                Data = json,
                JsonFilePath = jsonFilePath,
                ModDirectory = Path.GetDirectoryName(jsonFilePath),
                ApiVer = new Version(json.Versions.Api),
                GameVer = new Version(json.Versions.Game),
                ModVer = new Version(json.Versions.Mod),
                Mods = new Dictionary<Assembly, Mod[]>()
            };

            if (!ValidateVersion(imp.ModVer, imp.GameVer, imp.ApiVer))
            {
                _logger.Warn($"Skipping {imp.Data.Name} (v{imp.ModVer}) - Invalid game version.");
                return null;
            }

            var baseDir = Path.GetDirectoryName(jsonFilePath);

            foreach (var file in json.EntryFiles)
            {
                _logger.Debug($"Start loading entry file: {file}");
                var path = Path.Combine(baseDir, file);
                if (!File.Exists(path))
                {
                    _logger.Warn($"Couldn't find entry file for mod: {jsonFilePath}\r\nMissing file path: {path}");
                    continue;
                }

                try
                {
                    var assm = Assembly.LoadFrom(path);
                    var mods = LoadMod(assm, imp).ToArray();

                    imp.Mods.Add(assm, mods);
                    _logger.Debug($"Loaded entry file for: {jsonFilePath}\r\nEntry File: {path}");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to load assembly for mod: {jsonFilePath}\r\nAttempted to load: {path}\r\n{ex}");
                    continue;
                }
            }

            if (imp.Mods.Count <= 0)
            {
                _logger.Warn($"No mods found for file: {jsonFilePath}");
                return null;
            }

            return imp;
        }

        //TODO: Complete this
        public bool ValidateVersion(IVersion mod, IVersion game, IVersion api)
        {
            return true;
        }

        public IEnumerable<Mod> LoadMod(Assembly assm, ModImplementation bas)
        {
            var t = typeof(Mod);
            var types = assm.GetTypes();
            foreach (var type in types)
            {
                if (type.IsInterface ||
                    type.IsAbstract ||
                    !t.IsAssignableFrom(type))
                    continue;

                var instance = Activator.CreateInstance(type);
                if (instance == null)
                    continue;

                var mod = (Mod)instance;

                mod.ModData = bas;

                yield return mod;
            }
        }
    }
}
