using System.Collections.Generic;
using System.Reflection;

namespace UnderMineControl.Models
{
    using API;
    using API.Models;

    public class ModImplementation : IMod
    {
        public string JsonFilePath { get; set; }
        public string ModDirectory { get; set; }
        public IModSettings Data { get; set; }
        public Dictionary<Assembly, Mod[]> Mods { get; set; }
        public IVersion ModVer { get; set; }
        public IVersion GameVer { get; set; }
        public IVersion ApiVer { get; set; }

    }
}
