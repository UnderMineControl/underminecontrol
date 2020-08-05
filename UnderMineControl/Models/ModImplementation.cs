using System.Collections.Generic;
using System.Reflection;

namespace UnderMineControl.Models
{
    using API;

    public class ModImplementation
    {
        public string JsonFilePath { get; set; }
        public ModJson Data { get; set; }
        public Dictionary<Assembly, IMod[]> Mods { get; set; }
        public Version ModVer { get; set; }
        public Version GameVer { get; set; }
        public Version ApiVer { get; set; }
    }
}
