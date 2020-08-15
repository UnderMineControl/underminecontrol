using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnderMineControl.Utility
{
    using API;

    public class ConfigurationUtility : IConfiguration
    {
        private const string CONFIG_FILENAME_DEFAULT = "config.json";

        public string WorkingDirectory { get; private set; }

        private readonly ILogger _logger;

        public ConfigurationUtility(ILogger logger, string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            _logger = logger;
        }

        public string ResolvePath(string filename)
        {
            var workingPath = Path.Combine(WorkingDirectory, filename);
            if (File.Exists(workingPath))
                return workingPath;

            if (filename == CONFIG_FILENAME_DEFAULT)
            {
                _logger.Error("Cannot resolve a path outside of the mod directory for default filenames (config.json)");
                return null;
            }

            var envWorking = Environment.CurrentDirectory;
            var files = Directory.GetFiles(envWorking, filename, SearchOption.AllDirectories);
            if (files.Length <= 0)
            {
                _logger.Error($"Couldn't find a config file with the given name in the working directory: {filename} >> {envWorking}");
                return null;
            }

            if (files.Length > 1)
            {
                _logger.Error($"Found more than one config file named with the same name: {filename} >> {envWorking}");
                return null;
            }

            return files.First();
        }

        public T Get<T>(string filename = null)
        {
            filename = filename ?? CONFIG_FILENAME_DEFAULT;
            try
            {
                var path = ResolvePath(filename);
                if (path == null)
                    return default;

                if (!File.Exists(path))
                {
                    _logger.Warn("Config file doesn't exist: " + filename);
                    return default;
                }

                var data = File.ReadAllText(path);
                return JsonUtility.FromJson<T>(data);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while fetching config file {filename}: {ex}");
                return default;
            }
        }

        public bool Set<T>(T data, string filename = null)
        {
            filename = filename ?? CONFIG_FILENAME_DEFAULT;

            try
            {
                var path = ResolvePath(filename);
                if (path == null)
                    path = Path.Combine(WorkingDirectory, filename);

                var jsonData = JsonUtility.ToJson(data);
                File.WriteAllText(path, jsonData);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred while writing config file {filename}: {ex}");
                return false;
            }
        }
    }
}
