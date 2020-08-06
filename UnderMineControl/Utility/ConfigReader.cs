using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnderMineControl.Utility
{
    using Models;

    public interface IConfigReader
    {
        IEnumerable<KeyValuePair<string, string>> ReadConfig(string data);
        IEnumerable<KeyValuePair<string, string>> ReadConfigFile(string filepath);
        ModJson LoadModJson(string filepath);
    }

    public class ConfigReader : IConfigReader
    {
        private const string KEY_SPLITTER = ":";
        private const string ENTRY_SPLITTER = "\r\n";
        private const string COMMENT_MARKER = "#";

        public IEnumerable<KeyValuePair<string, string>> ReadConfig(string data)
        {
            var lines = data.Split(new[] { ENTRY_SPLITTER }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith(COMMENT_MARKER) ||
                    !line.Contains(KEY_SPLITTER))
                    continue;

                var args = line.Split(new[] { KEY_SPLITTER }, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length <= 1)
                    continue;

                yield return new KeyValuePair<string, string>(args[0].Trim(), string.Join(KEY_SPLITTER, args.Skip(1)).Trim());
            }
        }

        public IEnumerable<KeyValuePair<string, string>> ReadConfigFile(string filepath)
        {
            if (!File.Exists(filepath))
                return new KeyValuePair<string, string>[0];

            var data = File.ReadAllText(filepath);
            return ReadConfig(data);
        }

        public ModJson LoadModJson(string filepath)
        {
            var mj = new ModJson
            {
                Versions = new ModJsonVersions
                {

                }
            };

            foreach(var c in ReadConfigFile(filepath))
            {
                switch(c.Key)
                {
                    case "Name": mj.Name = c.Value; break;
                    case "TagLine": mj.TagLine = c.Value; break;
                    case "Author": mj.Author = c.Value; break;
                    case "GameVersion": mj.Versions.Game = c.Value; break;
                    case "ApiVersion": mj.Versions.Api = c.Value; break;
                    case "Version": mj.Versions.Mod = c.Value; break;
                    case "EntryFiles": mj.EntryFiles = c.Value.Split(',').Select(t => t.Trim()).ToArray(); break;
                    case "Url": mj.Url = c.Value; break;
                }
            }

            return mj;
        }
    }
}
