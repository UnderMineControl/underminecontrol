using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace UnderMineControl.Utility
{
    using API;
    using Models;
    using System.Linq;

    public interface IModLoader
    {
        IEnumerable<ModImplementation> LoadMods();
    }

    public class ModLoader : IModLoader
    {
        private const string FILE_EXT = "*.umcmod.json";

        private readonly IReflectionUtility _reflection;
        private readonly ILogger _logger;

        public ModLoader(IReflectionUtility reflection, ILogger logger)
        {
            _reflection = reflection;
            _logger = logger;
        }

        public IEnumerable<ModImplementation> LoadMods()
        {
            var curDir = Environment.CurrentDirectory;
            var files = Directory.GetFiles(curDir, FILE_EXT, SearchOption.AllDirectories);
            _logger.Debug($"Found {files.Length} mods! Starting load.\r\n{curDir}");
            foreach(var file in files)
            {
                var json = LoadJson(file);
                if (json == null)
                    continue;

                yield return LoadMod(json, file);
            }
        }

        public ModJson LoadJson(string file)
        {
            _logger.Debug("Loading mod file: " + file);

            if (!File.Exists(file))
            {
                _logger.Warn("Json file doesn't exist: " + file);
                return null;
            }

            try
            {
                using (var io = File.OpenRead(file))
                {
                    var sr = new DataContractJsonSerializer(typeof(ModJson));
                    return (ModJson)sr.ReadObject(io);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Json file failed to parse: {file}\r\n{ex}");
                return null;
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
                ApiVer = new Version(json.Versions.Api),
                GameVer = new Version(json.Versions.Game),
                ModVer = new Version(json.Versions.Mod),
                Mods = new Dictionary<Assembly, IMod[]>()
            };

            if (!ValidateVersion(imp.ModVer, imp.GameVer, imp.ApiVer))
            {
                _logger.Warn($"Skipping {imp.Data.Name} (v{imp.ModVer}) - Invalid game version.");
                return null;
            }

            var baseDir = Path.GetDirectoryName(jsonFilePath);

            foreach(var file in json.EntryFiles)
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
        public bool ValidateVersion(Version mod, Version game, Version api)
        {
            return true;
        }

        public IEnumerable<IMod> LoadMod(Assembly assm, ModImplementation bas)
        {
            var types = _reflection.GetTypes(typeof(IMod), assm);
            foreach (var type in types)
            {
                yield return (IMod)_reflection.GetInstance(type);
            }
        }
    }
}
